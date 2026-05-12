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
        ServerSession serverSession = session as ServerSession;

        NetworkManager.Instance.Spawn(EntityType.MyPlayer, 0, new Vector3(0, 0, 0));
    }

    public static void S_CreateOtherCharacterHandler(PacketSession session, IPacket packet)
    {
        S_CreateOtherCharacter pkt = packet as S_CreateOtherCharacter;
        ServerSession serverSession = session as ServerSession;

        NetworkManager.Instance.Spawn(EntityType.OtherPlayer, 0, new Vector3(0, 0, 0));
    }

    public static void S_DeleteCharacterHandler(PacketSession session, IPacket packet)
    {
        S_DeleteCharacter pkt = packet as S_DeleteCharacter;
        ServerSession serverSession = session as ServerSession;

        NetworkManager.Instance.Despawn(EntityType.OtherPlayer, 0);
    }

    public static void S_MoveStartHandler(PacketSession session, IPacket packet)
    {
        S_MoveStart pkt = packet as S_MoveStart;
        ServerSession serverSession = session as ServerSession;

        NetEntity entity = NetworkManager.Instance.entitySystem.Get((int)pkt.accountId);
        entity.DispatchPacket(NetBehaviourType.Controller, packet);
    }

    public static void S_TickSyncHandler(PacketSession session, IPacket packet)
    {
        S_TickSync pkt = packet as S_TickSync;
        ServerSession serverSession = session as ServerSession;

        NetworkManager.Instance.tickScheduler.UpdateTick(pkt.serverTick);
    }

    public static void S_RotateStartHandler(PacketSession session, IPacket packet)
    {
        S_RotateStart pkt = packet as S_RotateStart;
        ServerSession serverSession = session as ServerSession;

        NetEntity entity = NetworkManager.Instance.entitySystem.Get((int)pkt.accountId);
        entity.DispatchPacket(NetBehaviourType.Aim, packet);
    }

    internal static void S_MoveStateHandler(PacketSession session, IPacket packet)
    {
        S_MoveState pkt = packet as S_MoveState;
        ServerSession serverSession = session as ServerSession;

        NetEntity entity = NetworkManager.Instance.entitySystem.Get((int)pkt.accountId);
        entity.DispatchPacket(NetBehaviourType.Controller, packet);
    }

    internal static void S_RotateStateHandler(PacketSession session, IPacket packet)
    {
        S_RotateStart pkt = packet as S_RotateStart;
        ServerSession serverSession = session as ServerSession;

        NetEntity entity = NetworkManager.Instance.entitySystem.Get((int)pkt.accountId);
        entity.DispatchPacket(NetBehaviourType.Controller, packet);
    }

    internal static void S_ProjectileShootStartHandler(PacketSession session, IPacket packet)
    {
        S_ProjectileShootStart pkt = packet as S_ProjectileShootStart;
        ServerSession serverSession = session as ServerSession;

        NetEntity entity = NetworkManager.Instance.entitySystem.Get((int)pkt.accountId);
        entity.DispatchPacket(NetBehaviourType.Projectile, packet);
    }

    internal static void S_ProjectileShootStateHandler(PacketSession session, IPacket packet)
    {
        S_ProjectileShootState pkt = packet as S_ProjectileShootState;
        ServerSession serverSession = session as ServerSession;

        NetEntity entity = NetworkManager.Instance.entitySystem.Get((int)pkt.accountId);
        entity.DispatchPacket(NetBehaviourType.Controller, packet);
    }
}
