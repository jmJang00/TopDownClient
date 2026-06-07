using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.PlayerLoop;

public class PlayerHealth : NetBehaviour 
{
    public override int RenderingOrder => 4;

    public override ITickRunner Runner => null;

    public override NetBehaviourType Type => NetBehaviourType.Health;

    private Health _health;

    public float InvincibilityDuration = 0.0f;

    public override void Init()
    {
        base.Init();
        _health = gameObject.MMGetComponentNoAlloc<Health>();
    }

    public override void OnSpawn(int tick)
    {
        base.OnSpawn(tick);
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
            case (ushort)PacketID.S_ProjectileHit:
            {
                var p = packet as S_ProjectileHit;
                _tickScheduler.ScheduleAt(p.currentTick, () =>
                {
                    _health.Damage(
                        p.damage, 
                        gameObject, 
                        InvincibilityDuration, 
                        InvincibilityDuration,
                        Vector3.up);
                });

                break;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (_health != null)
            {
                _health.Damage(
                    150,
                    gameObject,
                    InvincibilityDuration,
                    InvincibilityDuration,
                    Vector3.up);

            }
        }
    }
}
