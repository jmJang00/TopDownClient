using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class PacketHandler
{
    public static void S_CreateMyCharacterHandler(PacketSession session, IPacket packet)
    {
        S_CreateMyCharacter pkt = packet as S_CreateMyCharacter;

        NetworkManager.Instance.spawnManager.SpawnAt(pkt.serverTick, EntityType.MyPlayer, pkt.entityId, new Vector3(0, 0, 0));
    }

    public static void S_CreateOtherCharacterHandler(PacketSession session, IPacket packet)
    {
        S_CreateOtherCharacter pkt = packet as S_CreateOtherCharacter;

        NetworkManager.Instance.spawnManager.SpawnAt(pkt.serverTick, EntityType.OtherPlayer, pkt.entityId, new Vector3(0, 0, 0));
    }

    public static void S_DeleteCharacterHandler(PacketSession session, IPacket packet)
    {
        S_DeleteCharacter pkt = packet as S_DeleteCharacter;

        if (NetworkManager.Instance.entitySystem.MyCharacter?.entityId == pkt.entityId)
        {
            NetworkManager.Instance.spawnManager.DespawnAt(pkt.serverTick, EntityType.MyPlayer, pkt.entityId);
        }
        else
        {
            NetworkManager.Instance.spawnManager.DespawnAt(pkt.serverTick, EntityType.OtherPlayer, pkt.entityId);
        }
    }

    public static void S_MoveStartHandler(PacketSession session, IPacket packet)
    {
        S_MoveStart pkt = packet as S_MoveStart;

        NetEntity entity = NetworkManager.Instance.entitySystem.Get(pkt.entityId);
        entity.DispatchPacket(NetBehaviourType.Controller, packet);
    }

    public static void S_TickSyncHandler(PacketSession session, IPacket packet)
    {
        S_TickSync pkt = packet as S_TickSync;

        NetworkManager.Instance.tickScheduler.UpdateTick(pkt.serverTick);
    }

    public static void S_RotateStartHandler(PacketSession session, IPacket packet)
    {
        S_RotateStart pkt = packet as S_RotateStart;

        NetEntity entity = NetworkManager.Instance.entitySystem.Get(pkt.entityId);
        entity.DispatchPacket(NetBehaviourType.Aim, packet);
    }

    internal static void S_MoveStateHandler(PacketSession session, IPacket packet)
    {
        S_MoveState pkt = packet as S_MoveState;

        NetEntity entity = NetworkManager.Instance.entitySystem.Get(pkt.entityId);
        entity.DispatchPacket(NetBehaviourType.Controller, packet);
    }

    internal static void S_RotateStateHandler(PacketSession session, IPacket packet)
    {
        S_RotateState pkt = packet as S_RotateState;

        NetEntity entity = NetworkManager.Instance.entitySystem.Get(pkt.entityId);
        entity.DispatchPacket(NetBehaviourType.Aim, packet);
    }

    internal static void S_ProjectileShootStartHandler(PacketSession session, IPacket packet)
    {
        S_ProjectileShootStart pkt = packet as S_ProjectileShootStart;

        NetEntity entity = NetworkManager.Instance.entitySystem.Get(pkt.entityId);
        entity.DispatchPacket(NetBehaviourType.Projectile, packet);
    }

    internal static void S_SpawnProjectileHandler(PacketSession session, IPacket packet)
    {
        S_SpawnProjectile pkt = packet as S_SpawnProjectile;

        NetworkManager.Instance.spawnManager.SpawnAt(
            pkt.currentTick, EntityType.Projectile, pkt.entityId, Vector2.zero);
        NetEntity entity = NetworkManager.Instance.entitySystem.Get(pkt.entityId);
        entity.DispatchPacket(NetBehaviourType.BulletMovement, packet);
    }

    internal static void S_DespawnProjectileHandler(PacketSession session, IPacket packet)
    {
        S_DespawnProjectile pkt = packet as S_DespawnProjectile;

        NetworkManager.Instance.spawnManager.DespawnAt(pkt.currentTick, EntityType.Projectile, pkt.entityId);
    }

    internal static void S_ProjectileHitHandler(PacketSession session, IPacket packet)
    {
        S_ProjectileHit pkt = packet as S_ProjectileHit;

        NetEntity bullet = NetworkManager.Instance.entitySystem.Get(pkt.bulletId);
        bullet.DispatchPacket(NetBehaviourType.BulletMovement, packet);

        NetEntity collision = NetworkManager.Instance.entitySystem.Get(pkt.collisionId);
        collision.DispatchPacket(NetBehaviourType.Health, packet);
    }

    internal static void S_GameStartWaitHandler(PacketSession session, IPacket packet)
    {
        NetworkManager.Instance.OnGameFound();
    }

    internal static void S_GameStartHandler(PacketSession session, IPacket packet)
    {
        S_GameStart pkt = packet as S_GameStart;
        NetworkManager.Instance.tickScheduler.UpdateTick(pkt.tick);
        NetworkManager.Instance.OnGameStart();
    }

    internal static void S_GameEndHandler(PacketSession session, IPacket packet)
    {
        NetworkManager.Instance.OnGameEnd();
    }
}
