using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.PlayerLoop;

public class PlayerHealth : NetBehaviour 
{
    public override int RenderingOrder => 4;

    public override ITickRunner Runner => null;

    public override NetBehaviourType Type => NetBehaviourType.Health;

    private HealthRevised _health;

    public ushort currentHealth = 150;
    public float maximumHealth = 150;

    public float InvincibilityDuration = 0.0f;

    public override void Init()
    {
        base.Init();
        _health = gameObject.MMGetComponentNoAlloc<HealthRevised>();
    }

    public override void OnSpawn(int tick)
    {
        base.OnSpawn(tick);
        maximumHealth = _health.MaximumHealth;
        _health.SetHealth(currentHealth);
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
                var p = packet as S_NtfProjectileHit;
                Damage(p.currentTick, p.damage);
                break;
            }
            case (ushort)PacketID.S_HitscanHit:
            {
                var p = packet as S_HitscanHit;
                Damage(p.serverTick, p.damage);
                break;
            }
        }
    }

    public void Damage(int serverTick, ushort damage)
    {
        Player player = (Player)Entity;
        _tickScheduler.ScheduleAt(serverTick, () =>
        {
            _health.Damage(
                damage,
                gameObject,
                InvincibilityDuration,
                InvincibilityDuration,
                Vector3.up);

            currentHealth -= damage;

            player.NamePlate.SetHP(currentHealth / maximumHealth);
        });
    }

    public void SetHealth(ushort health)
    {
        currentHealth = health;
    }

    void Update()
    {
        if (InputModeManager.Instance.CurrentMode != InputMode.Game)
            return;
            

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
