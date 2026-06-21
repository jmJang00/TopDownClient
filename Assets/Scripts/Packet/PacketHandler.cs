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

        if (NetworkManager.Instance.game)
        {
            NetworkManager.Instance.spawnManager.SpawnAt(pkt.serverTick, EntityType.MyPlayer, pkt.entityId, new Vector3(0, 0, 0));
        }
    }

    public static void S_CreateOtherCharacterHandler(PacketSession session, IPacket packet)
    {
        S_CreateOtherCharacter pkt = packet as S_CreateOtherCharacter;

        if (NetworkManager.Instance.game)
        {
            NetworkManager.Instance.spawnManager.SpawnAt(pkt.serverTick, EntityType.OtherPlayer, pkt.entityId, new Vector3(0, 0, 0));
        }
    }

    public static void S_DeleteCharacterHandler(PacketSession session, IPacket packet)
    {
        S_DeleteCharacter pkt = packet as S_DeleteCharacter;

        if (NetworkManager.Instance.game)
        {
            if (NetworkManager.Instance.entitySystem.MyCharacter.entityId == pkt.entityId)
            {
                NetworkManager.Instance.spawnManager.DespawnAt(pkt.serverTick, EntityType.MyPlayer, pkt.entityId);
            }
            else
            {
                NetworkManager.Instance.spawnManager.DespawnAt(pkt.serverTick, EntityType.OtherPlayer, pkt.entityId);
            }
        }
    }

    public static void S_MoveStartHandler(PacketSession session, IPacket packet)
    {
        S_MoveStart pkt = packet as S_MoveStart;

        if (NetworkManager.Instance.game)
        {
            NetEntity entity = NetworkManager.Instance.entitySystem.Get(pkt.entityId);
            entity.DispatchPacket(NetBehaviourType.Controller, packet);
        }
    }

    public static void S_TickSyncHandler(PacketSession session, IPacket packet)
    {
        S_TickSync pkt = packet as S_TickSync;

        if (NetworkManager.Instance.game)
        {
            NetworkManager.Instance.tickScheduler.UpdateTick(pkt.serverTick);
        }
    }

    public static void S_RotateStartHandler(PacketSession session, IPacket packet)
    {
        S_RotateStart pkt = packet as S_RotateStart;

        if (NetworkManager.Instance.game)
        {
            NetEntity entity = NetworkManager.Instance.entitySystem.Get(pkt.entityId);
            entity.DispatchPacket(NetBehaviourType.Aim, packet);
        }
    }

    internal static void S_MoveStateHandler(PacketSession session, IPacket packet)
    {
        S_MoveState pkt = packet as S_MoveState;

        if (NetworkManager.Instance.game)
        {
            NetEntity entity = NetworkManager.Instance.entitySystem.Get(pkt.entityId);
            entity.DispatchPacket(NetBehaviourType.Controller, packet);
        }
    }

    internal static void S_RotateStateHandler(PacketSession session, IPacket packet)
    {
        S_RotateState pkt = packet as S_RotateState;

        if (NetworkManager.Instance.game)
        {
            NetEntity entity = NetworkManager.Instance.entitySystem.Get(pkt.entityId);
            entity.DispatchPacket(NetBehaviourType.Aim, packet);
        }
    }

    internal static void S_ProjectileShootStartHandler(PacketSession session, IPacket packet)
    {
        S_ProjectileShootStart pkt = packet as S_ProjectileShootStart;

        if (NetworkManager.Instance.game)
        {
            NetEntity entity = NetworkManager.Instance.entitySystem.Get(pkt.entityId);
            entity.DispatchPacket(NetBehaviourType.Projectile, packet);
        }
    }

    internal static void S_SpawnProjectileHandler(PacketSession session, IPacket packet)
    {
        S_SpawnProjectile pkt = packet as S_SpawnProjectile;

        if (NetworkManager.Instance.game)
        {
            NetworkManager.Instance.spawnManager.SpawnAt(
                pkt.currentTick, EntityType.Projectile, pkt.entityId, Vector2.zero);
            NetEntity entity = NetworkManager.Instance.entitySystem.Get(pkt.entityId);
            entity.DispatchPacket(NetBehaviourType.BulletMovement, packet);
        }
    }

    internal static void S_DespawnProjectileHandler(PacketSession session, IPacket packet)
    {
        S_DespawnProjectile pkt = packet as S_DespawnProjectile;

        if (NetworkManager.Instance.game)
        {
            NetworkManager.Instance.spawnManager.DespawnAt(pkt.currentTick, EntityType.Projectile, pkt.entityId);
        }
    }

    internal static void S_ProjectileHitHandler(PacketSession session, IPacket packet)
    {
        S_ProjectileHit pkt = packet as S_ProjectileHit;

        if (NetworkManager.Instance.game)
        {
            NetEntity bullet = NetworkManager.Instance.entitySystem.Get(pkt.bulletId);
            bullet.DispatchPacket(NetBehaviourType.BulletMovement, packet);

            NetEntity collision = NetworkManager.Instance.entitySystem.Get(pkt.collisionId);
            collision.DispatchPacket(NetBehaviourType.Health, packet);
        }
    }

    internal static void S_GameStartHandler(PacketSession session, IPacket packet)
    {
        S_GameStart pkt = packet as S_GameStart;
        if (NetworkManager.Instance.game)
        {
            NetworkManager.Instance.tickScheduler.UpdateTick(pkt.tick);
            NetworkManager.Instance.OnGameStart(pkt.success);
        }
    }

    internal static void S_GameEndHandler(PacketSession session, IPacket packet)
    {
        S_GameEnd pkt = packet as S_GameEnd;
        NetworkManager.Instance.OnGameEnd(pkt.win);
    }

    internal static void S_MatchFoundHandler(PacketSession session, IPacket packet)
    {
        S_MatchFound pkt = packet as S_MatchFound;
        UIEventBus.Publish(packet);
        NetworkManager.Instance.OnGameFound(pkt.success);
    }

    internal static void S_ReturnToLobbyHandler(PacketSession session, IPacket packet)
    {
        NetworkManager.Instance.OnReturnToLobby();
    }
}
