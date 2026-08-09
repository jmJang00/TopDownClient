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


public class PlayerHitscanSync : NetBehaviour 
{   

    public override int RenderingOrder => 2;

    public override ITickRunner Runner => null;

    public override NetBehaviourType Type => NetBehaviourType.Hitscan;

    public Vector3 projectileSpawnOffset;
    public bool hasAuthority;
    public float bulletSpeed;
    public float updateInterval = 1.0f;
    private float _updateTimer;

    public CharacterHandleWeapon TargetHandleWeaponAbility;
    public HitscanWeaponRevised CurrentWeapon;
    public MMObjectPooler ObjectPooler;
    
    private HitscanState _state;
    protected Character _character;
    private bool _wasInput;    

    private PlayerAimController _playerAimController;
    private PlayerController _playerController;

    public override void Init()
    {
        base.Init();
        _character = GetComponentInParent<Character>();
        TargetHandleWeaponAbility = _character?.FindAbility<CharacterHandleWeapon>();
        _updateTimer = updateInterval;
    }

    public void Update()
    {
        if (InputModeManager.Instance.CurrentMode != InputMode.Game)
            return;

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

                            NetworkManager.Instance.Send(pkt.Write());

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
        _state.mask.baseTick = tick;
        _state.mask.low = 0;
        _state.mask.high = 0;
    }

    public override void OnDespawn()
    {
        base.OnDespawn();        
    }

    public override void OnRender(float alpha)
    {       


    }

    public override void DispatchPacket(IPacket packet)
    {
        switch (packet.Protocol)
        {
            case (ushort)PacketID.S_HitscanShootStart:
            {
                if (!hasAuthority)
                {
                    StartCoroutine(ShootTrigger());
                    S_HitscanShootStart pkt = packet as S_HitscanShootStart;
                    LaserManager.Instance.DrawLaser(new Vector3(pkt.startX, 2.23f, pkt.startY), new Vector3(pkt.endX, 2.23f, pkt.endY));
                }
                break;
            }
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
        TargetHandleWeaponAbility.ShootStart();

        yield return new WaitForSeconds(0.05f);
        TargetHandleWeaponAbility.ForceStop();
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
