using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using NUnit;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Purchasing;

public class HitscanWeaponRevised : HitscanWeapon
{       
    public override void SpawnProjectile(Vector3 spawnPosition, bool triggerObjectActivation = true)
    {
        //LineRendering
        base.SpawnProjectile(spawnPosition, triggerObjectActivation);
        SendShootStartPacket();
        return;
    }

    protected override void HandleDamage()
    {
        //슈터가 플레이어블 캐릭터일때만 체크한다.
        NetEntity entity = this.Owner.gameObject.GetComponent<NetEntity>();
        if (entity.type != EntityType.MyPlayerH)
        {
            return;
        }

        if (_hitObject == null)
        {
            return;
        }

        //히트대상이 플레이어면.
        //히트요청 보내자.
        if(_hitObject.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Player shooter = Owner.gameObject.GetComponent<Player>();
            Player target = _hitObject.gameObject.GetComponent<Player>();
            int tick = NetworkManager.Instance.tickScheduler.GetCurrentTick();            

            C_HitscanHit pkt2 = new C_HitscanHit();
            pkt2.currentTick = tick;            
            pkt2.shooterId = shooter.entityId;
            pkt2.targetId = target.entityId;
            NetworkManager.Instance.GameSend(pkt2.Write());
        }
    }

    private void SendShootStartPacket()
    {
        //슈터가 플레이어블 캐릭터일때만 보낸다.
        NetEntity entity = this.Owner.gameObject.GetComponent<NetEntity>();
        if (entity.type != EntityType.MyPlayerH)
        {
            return;
        }

        //사격 방향 계산
        Vector3 v1, v2;
        GetLastShootData(out v1, out v2);

        LaserManager.Instance.DrawLaser(v1, v2);

        C_HitscanShootStart pkt = new C_HitscanShootStart();
        pkt.clientTick = NetworkManager.Instance.tickScheduler.GetCurrentTick();
        pkt.startX = v1.x;
        pkt.startY = v1.z;
        pkt.endX = v2.x;
        pkt.endY = v2.z;
        NetworkManager.Instance.GameSend(pkt.Write());

    }

    public void GetLastShootData(out Vector3 originPos, out Vector3 hitPos)
    {
        originPos = _origin;
        if (_hitObject != null)
        {
            hitPos = _hitPoint;
        }
        else
        {
            hitPos = _origin
                + (_randomSpreadDirection.normalized
                * HitscanMaxDistance);
        }
    }   
}
