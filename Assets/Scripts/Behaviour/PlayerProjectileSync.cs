using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.UI;
using System;
using MoreMountains.Feedbacks;
using System.Collections.Generic;
using MoreMountains.Tools;
using System.Collections;
using UnityEngine.UIElements;

public class PlayerProjectileSync : NetBehaviour
{
    public override int RenderingOrder => 2;

    public override ITickRunner Runner => null;

    public override NetBehaviourType Type => NetBehaviourType.Projectile;

    public Vector3 projectileSpawnOffset;
    public bool hasAuthority;
    public float bulletSpeed;
    public float updateInterval = 1.0f;
    private float _updateTimer;

    public CharacterHandleWeapon TargetHandleWeaponAbility;
    public ProjectileWeaponRevised CurrentWeapon;

    protected Character _character;
    private bool _wasInput;

    public override void Init()
    {
        base.Init();
        _character = GetComponent<Character>();
        TargetHandleWeaponAbility = _character.FindAbility<CharacterHandleWeapon>();
        _updateTimer = updateInterval;
    }

    public void Update()
    {
        if (!Ready)
        {
            return;
        }

        if (TargetHandleWeaponAbility?.CurrentWeapon != null)
        {
            if (TargetHandleWeaponAbility.CurrentWeapon is ProjectileWeaponRevised p)
            {
                CurrentWeapon = p;
                //p.OnSpawnProjectile += ShootTrigger;
            }
        }

        if (InputModeManager.Instance.CurrentMode != InputMode.Game)
            return;

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
                        _updateTimer = 0.0f;

                        //잔여총알있을시 감소.
                        if (MyChestInventoryManager.Instance.DecreaseAmmo())
                        {
                            //남은 총알 있을 시
                            StartCoroutine(ShootTrigger());
                            Debug.Log("Shoot");
                        }
                        else
                        {
                            //없다면 리로드 요청하기.
                            C_ReqReloadBullet pkt = new C_ReqReloadBullet();
                            pkt.clientTick = NetworkManager.Instance.tickScheduler.GetCurrentTick();

                            NetworkManager.Instance.GameSend(pkt.Write());

                            Debug.Log("Empty Ammo");
                        }
                        
                    }
                }
            }
        }
    }

    public override void OnSpawn(int tick)
    {
        base.OnSpawn(tick);
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
    }

    public override void DispatchPacket(IPacket packet)
    {

        switch (packet.Protocol)
        {
            case (ushort)PacketID.S_ProjectileShootStart:
            {
                S_ShootStart pkt = packet as S_ShootStart;
                if (!hasAuthority)
                {
                    _tickScheduler.ScheduleAt(pkt.accpetTick, () =>
                    {
                        if (gameObject.activeInHierarchy)
                        {
                            StartCoroutine(ShootTrigger());
                        }
                    });
                }
                break;
            }
        }
    }

    public IEnumerator ShootTrigger()
    {
        TargetHandleWeaponAbility.ShootStart();

        if (hasAuthority)
        {
            C_ProjectileShootStart pkt = new C_ProjectileShootStart();
            pkt.clientTick = _tickScheduler.GetCurrentTick();
            NetworkManager.Instance.GameSend(pkt.Write());
        }

        yield return new WaitForSeconds(0.1f);

        TargetHandleWeaponAbility.ForceStop();
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
}
