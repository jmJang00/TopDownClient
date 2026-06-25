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


public struct HitscanState
{
    public BulletMask128 mask;
}

public struct HitscanInput
{
    public int fireTick;
}


public class PlayerHitscanSync : NetBehaviour, ITickable<HitscanState, HitscanInput> 
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

    public override NetBehaviourType Type => NetBehaviourType.Hitscan;

    public Vector3 projectileSpawnOffset;
    public bool hasAuthority;
    public float bulletSpeed;
    public float updateInterval = 1.0f;
    private float _updateTimer;

    public CharacterHandleWeapon TargetHandleWeaponAbility;
    public HitscanWeaponRevised CurrentWeapon;
    public MMObjectPooler ObjectPooler;

    private Dictionary<int, GameObject> _spawnedProjectile;
    private HitscanState _spawnedState;
    private HitscanState _state;
    protected Character _character;
    private bool _wasInput;
    private ReplayerRunner<HitscanState, HitscanInput> _runner;

    private PlayerAimController _playerAimController;
    private PlayerController _playerController;

    public override void Init()
    {
        base.Init();
        _spawnedProjectile = new Dictionary<int, GameObject>();
        _runner = new ReplayerRunner<HitscanState, HitscanInput>(this, hasAuthority,
            Entity.renderDelay, (int tick, HitscanInput input) =>
            {
                _tickScheduler.ScheduleAfter(2, () =>
                {
                    S_HitscanShootStart pkt = new S_HitscanShootStart();
                    pkt.accpetTick = tick;
                    DispatchPacket(pkt);
                });

                _tickScheduler.ScheduleAfter(2, () =>
                {
                    NetEntity entity = NetworkManager.Instance.entitySystem.Get(1);
                    S_HitscanShootStart pkt = new S_HitscanShootStart();
                    pkt.accpetTick = tick;
                    entity.DispatchPacket(NetBehaviourType.Hitscan, pkt);
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
            if (TargetHandleWeaponAbility.CurrentWeapon is HitscanWeaponRevised p)
            {
                CurrentWeapon = p;                
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
    }

    public override void DispatchPacket(IPacket packet)
    {
        if (packet is S_HitscanShootStart p)
        {
            var input = new HitscanInput
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

    public void ApplyInput(in HitscanInput input)
    {
        Set(ref _state.mask, input.fireTick);
    }

    public HitscanState CaptureState()
    {
        return _state;
    }

    public void RestoreState(in HitscanState state)
    {
        _state = state;
    }

    public IEnumerator ShootTrigger()
    {
        //TargetHandleWeaponAbility.PlayAbilityStartFeedbacks();

        if (hasAuthority)
        {
            int tick = _tickScheduler.GetCurrentTick();
            HitscanInput input;
            input.fireTick = tick;
            _runner.EnqueueClientInput(tick, input);
        }        
        else
        {
            TargetHandleWeaponAbility.ShootStart();            
        }

        yield return new WaitForSeconds(0.05f);
        TargetHandleWeaponAbility.ShootStop();
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

    public void Tick(int tick, float dt)
    {
        Slide(ref _state.mask, tick - 64);
    }
}
