using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.UI;
using System;
using MoreMountains.Feedbacks;
using System.Collections.Generic;
using MoreMountains.Tools;
using System.Collections;

public struct BulletMask128
{
    public ulong low;   // [0..63]
    public ulong high;  // [64..127]
    public int baseTick; // bit 0이 의미하는 틱
}

public struct ProjectileState
{
    public BulletMask128 mask;
}

public struct ProjectileInput
{
    public int fireTick;
}

public class PlayerProjectileSync : NetBehaviour, ITickable<ProjectileState, ProjectileInput> 
{
    // De Brujin

    public static readonly int[] index64 = new int[64]
    {
         0, 1, 48, 2, 57, 49, 28, 3,
        61, 58, 50, 42, 38, 29, 17, 4,
        62, 55, 59, 36, 53, 51, 43, 22,
        45, 39, 33, 30, 24, 18, 12, 5,
        63, 47, 56, 27, 60, 41, 37, 16,
        54, 35, 52, 21, 44, 32, 23, 11,
        46, 26, 40, 15, 34, 20, 31, 10,
        25, 14, 19, 9, 13, 8, 7, 6
    };

    public override int RenderingOrder => 2;

    public override ITickRunner Runner => _runner;

    public override NetBehaviourType Type => NetBehaviourType.Projectile;

    public Vector3 projectileSpawnOffset;
    public bool hasAuthority;
    public float bulletSpeed;
    public float updateInterval = 1.0f;
    private float _updateTimer;

    public CharacterHandleWeapon TargetHandleWeaponAbility;
    public ProjectileWeaponRevised CurrentWeapon;
    public MMObjectPooler ObjectPooler;

    private Dictionary<int, GameObject> _spawnedProjectile;
    private ProjectileState _spawnedState;
    private ProjectileState _state;
    protected Character _character;
    private bool _wasInput;
    private ReplayerRunner<ProjectileState, ProjectileInput> _runner;

    private PlayerAimController _playerAimController;
    private PlayerController _playerController;

    public override void Init()
    {
        base.Init();
        _spawnedProjectile = new Dictionary<int, GameObject>();
        _runner = new ReplayerRunner<ProjectileState, ProjectileInput>(this, hasAuthority,
            Entity.renderDelay, (int tick, ProjectileInput input) =>
            {
                _tickScheduler.ScheduleAfter(2, () =>
                {
                    S_ProjectileShootStart pkt = new S_ProjectileShootStart();
                    pkt.accpetTick = tick;
                    DispatchPacket(pkt);
                });

                _tickScheduler.ScheduleAfter(2, () =>
                {
                    NetEntity entity = NetworkManager.Instance.entitySystem.Get(1);
                    S_ProjectileShootStart pkt = new S_ProjectileShootStart();
                    pkt.accpetTick = tick;
                    entity.DispatchPacket(NetBehaviourType.Projectile, pkt);
                });

            });
        _playerAimController = GetComponent<PlayerAimController>();
        _playerController = GetComponent<PlayerController>();
        _character = GetComponentInParent<Character>();
        TargetHandleWeaponAbility = _character?.FindAbility<CharacterHandleWeapon>();
        _updateTimer = updateInterval;
    }

    public void Update()
    {
        if (TargetHandleWeaponAbility?.CurrentWeapon != null)
        {
            if (TargetHandleWeaponAbility.CurrentWeapon is ProjectileWeaponRevised p)
            {
                CurrentWeapon = p;
                //p.OnSpawnProjectile += ShootTrigger;
            }
        }

        if (ObjectPooler == null)
        {
            ObjectPooler = GetComponent<MMObjectPooler>();
        }

        bool hasInput = Input.GetMouseButton(0);

        if (CurrentWeapon)
        {
            if (hasAuthority)
            {
                _updateTimer += Time.deltaTime;

                if (hasInput)
                {
                    if (_updateTimer >= updateInterval)
                    {
                        StartCoroutine(ShootTrigger());
                        _updateTimer = 0.0f;
                        Debug.Log("Shoot");
                    }
                }
            }
        }
    }

    public override void OnSpawn(int tick)
    {
        base.OnSpawn(tick);
        _tickScheduler.Register(_runner);
        _state.mask.baseTick = tick;
        _state.mask.low = 0;
        _state.mask.high = 0;
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        _tickScheduler.Unregister(_runner);
    }

    public override void OnRender(float alpha)
    {
        BulletMask128 added, removed;
        _runner.TryGetRenderPair(out ProjectileState _, out ProjectileState curr);
        DiffDualBase(_spawnedState.mask, curr.mask, out added, out removed);

        // 생성 (curr 기준 tick)
        ForEach(in added, (int tick) =>
        {
            Vector2 spawnPos2D = _playerController.GetPositionAt(tick);
            SpawnBulletGo(tick, new Vector3(spawnPos2D.x, transform.position.y, spawnPos2D.y));
        });

        // 제거 (prev 기준 tick)
        ForEach(in removed, DespawnBulletGo);

        _spawnedState = curr;

        int renderTick = _tickScheduler.GetCurrentTick() - Entity.renderDelay;
        float deltaTime = _tickScheduler.GetDeltaTime();

        foreach (var kv in _spawnedProjectile)
        {
            int tick = kv.Key;
            Vector2 spawnPos2D = _playerController.GetPositionAt(tick);
            float spawnAngle = _playerAimController.GetAngleAt(tick);

            float rad = spawnAngle * Mathf.Deg2Rad;
            float vx = Mathf.Cos(rad);
            float vz = Mathf.Sin(rad);
            Vector2 dir = new Vector2(vx, vz);

            Vector3 spawnPos3D = ComputeSpawnPos(new Vector3(spawnPos2D.x, transform.position.y, spawnPos2D.y), dir);

            //Vector3 spawnPos3D = new Vector3(spawnPos2D.x, transform.position.y, spawnPos2D.y);
            //spawnPos3D += Quaternion.Euler(0, spawnAngle, 0) * projectileSpawnDir;

            GameObject go = kv.Value;
            go.transform.position = ComputePosition(kv.Key, renderTick, deltaTime, bulletSpeed, 
                spawnPos3D, dir);

            if (!go.activeInHierarchy)
            {
                go.SetActive(true);
            }
        }
    }

    public override void DispatchPacket(IPacket packet)
    {
        if (packet is S_ProjectileShootStart p)
        {
            var input = new ProjectileInput
            {
                fireTick = p.accpetTick
            };

            if (!hasAuthority)
            {
                StartCoroutine(ShootTrigger());
            }
            _runner.EnqueueServerInput(p.accpetTick, input);
        }
    }

    public void ApplyInput(in ProjectileInput input)
    {
        Set(ref _state.mask, input.fireTick);
    }

    public ProjectileState CaptureState()
    {
        return _state;
    }

    public void RestoreState(in ProjectileState state)
    {
        _state = state;
    }

    public IEnumerator ShootTrigger()
    {
        TargetHandleWeaponAbility.ShootStart();

        if (hasAuthority)
        {
            int tick = _tickScheduler.GetCurrentTick();
            ProjectileInput input;
            input.fireTick = tick;
            _runner.EnqueueClientInput(tick, input);
        }

        yield return new WaitForSeconds(0.05f);

        TargetHandleWeaponAbility.ForceStop();
    }

    public void SpawnBulletGo(int tick, Vector3 position)
    {
        GameObject go = ObjectPooler.GetPooledGameObject();
        if (go == null) { return; }
        if (go.GetComponent<MMPoolableObject>() == null)
        {
            throw new Exception(gameObject.name + " is trying to spawn objects that don't have a PoolableObject component.");
        }

        go.transform.position = position;
        _spawnedProjectile.Add(tick, go);
    }

    public void DespawnBulletGo(int tick)
    {
        GameObject go = _spawnedProjectile[tick];
        _spawnedProjectile.Remove(tick);
        go.SetActive(false);
    }

    public static void Set(ref BulletMask128 m, int tick)
    {
        int d = tick - m.baseTick;
        if ((uint)d >= 128u) return; // 범위 밖

        if (d < 64) m.low |= 1UL << d;
        else m.high |= 1UL << (d - 64);
    }

    public static bool Get(in BulletMask128 m, int tick)
    {
        int d = tick - m.baseTick;
        if ((uint)d >= 128u) return false;

        if (d < 64) return ((m.low >> d) & 1UL) != 0;
        else return ((m.high >> (d - 64)) & 1UL) != 0;
    }

    public static void Slide(ref BulletMask128 m, int newBaseTick)
    {
        int shift = newBaseTick - m.baseTick;
        if (shift <= 0) return;

        if (shift >= 128)
        {
            m.low = 0;
            m.high = 0;
            m.baseTick = newBaseTick;
            return;
        }

        if (shift >= 64)
        {
            int s = shift - 64;
            m.low = (s < 64) ? (m.high >> s) : 0;
            m.high = 0;
        }
        else
        {
            ulong newLow = (m.low >> shift) | (m.high << (64 - shift));
            ulong newHigh = (m.high >> shift);

            m.low = newLow;
            m.high = newHigh;
        }

        m.baseTick = newBaseTick;
    }

    static int TrailingZeroCount(ulong x)
    {
        if (x == 0) return 64;

        ulong isolated = x & (~x + 1); // x & -x
        return index64[(isolated * 0x03F79D71B4CB0A89UL) >> 58];
    }

    public static void ForEach(in BulletMask128 m, Action<int> fn)
    {
        ulong lo = m.low;
        ulong hi = m.high;

        while (lo != 0)
        {
            int bit = TrailingZeroCount(lo);
            fn(m.baseTick + bit);
            lo &= lo - 1; // clear lowest set bit
        }

        while (hi != 0)
        {
            int bit = TrailingZeroCount(hi);
            fn(m.baseTick + 64 + bit);
            hi &= hi - 1;
        }
    }

    public static void Clear(ref BulletMask128 m, int tick)
    {
        int d = tick - m.baseTick;
        if ((uint)d >= 128u) return;

        if (d < 64) m.low &= ~(1UL << d);
        else m.high &= ~(1UL << (d - 64));
    }

    public static BulletMask128 AlignTo(in BulletMask128 src, int dstBaseTick)
    {
        int shift = dstBaseTick - src.baseTick;

        BulletMask128 r;
        r.baseTick = dstBaseTick;

        ulong lo = src.low;
        ulong hi = src.high;

        // 완전히 범위 밖
        if (shift >= 128 || shift <= -128)
        {
            r.low = 0;
            r.high = 0;
            return r;
        }

        if (shift > 0)
        {
            // right shift
            if (shift >= 64)
            {
                int s = shift - 64;
                r.low = (s < 64) ? (hi >> s) : 0;
                r.high = 0;
            }
            else
            {
                r.low = (lo >> shift) | (hi << (64 - shift));
                r.high = hi >> shift;
            }
        }
        else if (shift < 0)
        {
            // left shift
            int s = -shift;

            if (s >= 64)
            {
                int k = s - 64;
                r.high = (k < 64) ? (lo << k) : 0;
                r.low = 0;
            }
            else
            {
                r.high = (hi << s) | (lo >> (64 - s));
                r.low = lo << s;
            }
        }
        else
        {
            r.low = lo;
            r.high = hi;
        }

        return r;
    }

    public static void DiffDualBase(
        in BulletMask128 prev,
        in BulletMask128 curr,
        out BulletMask128 added,   // curr.baseTick 기준
        out BulletMask128 removed) // prev.baseTick 기준
    {
        // 1) added: curr 기준
        BulletMask128 prevToCurr = AlignTo(prev, curr.baseTick);

        added.baseTick = curr.baseTick;
        added.low = curr.low & ~prevToCurr.low;
        added.high = curr.high & ~prevToCurr.high;

        // 2) removed: prev 기준
        BulletMask128 currToPrev = AlignTo(curr, prev.baseTick);

        removed.baseTick = prev.baseTick;
        removed.low = prev.low & ~currToPrev.low;
        removed.high = prev.high & ~currToPrev.high;
    }

    public Vector3 ComputeSpawnPos(Vector3 playerPos, Vector2 dir)
    {
        // XZ 평면 회전
        float x = projectileSpawnOffset.x * dir.x - projectileSpawnOffset.z * dir.y;
        float z = projectileSpawnOffset.x * dir.y + projectileSpawnOffset.z * dir.x;

        return new Vector3(
            playerPos.x + x,
            playerPos.y + projectileSpawnOffset.y,
            playerPos.z + z
        );
    }

    public Vector3 ComputePosition(
        int fireTick,
        int renderTick,
        float tickDelta,     // 1틱 시간 (ex: 0.05f)
        float speed,         // units/sec
        Vector3 spawnPos,
        Vector2 dir) // yaw
    {
        int dtTick = renderTick - fireTick;
        if (dtTick <= 0)
            return spawnPos;

        float dt = dtTick * tickDelta;

        return new Vector3(
            spawnPos.x + dir.x * speed * dt,
            spawnPos.y,
            spawnPos.z + dir.y * speed * dt
        );
    }

    public void Tick(int tick, float dt)
    {
        Slide(ref _state.mask, tick - 64);
    }
}
