using System;
using System.Collections.Generic;
using ServerCore;

public class PacketManager
{
    #region Singleton
    static PacketManager _instance = new PacketManager();
    public static PacketManager Instance { get { return _instance; } }
    #endregion

    PacketManager() 
    {
        Register();
    }

    Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> _makeFunc = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();

    public void Register()
    {

        _makeFunc.Add((ushort)PacketID.C_MoveStart, MakePacket<C_MoveStart>);
        _handler.Add((ushort)PacketID.C_MoveStart, PacketHandler.C_MoveStartHandler);

        _makeFunc.Add((ushort)PacketID.C_RotateStart, MakePacket<C_RotateStart>);
        _handler.Add((ushort)PacketID.C_RotateStart, PacketHandler.C_RotateStartHandler);

        _makeFunc.Add((ushort)PacketID.C_ProjectileShootStart, MakePacket<C_ProjectileShootStart>);
        _handler.Add((ushort)PacketID.C_ProjectileShootStart, PacketHandler.C_ProjectileShootStartHandler);

        _makeFunc.Add((ushort)PacketID.C_MatchStart, MakePacket<C_MatchStart>);
        _handler.Add((ushort)PacketID.C_MatchStart, PacketHandler.C_MatchStartHandler);

        _makeFunc.Add((ushort)PacketID.C_MatchCancel, MakePacket<C_MatchCancel>);
        _handler.Add((ushort)PacketID.C_MatchCancel, PacketHandler.C_MatchCancelHandler);

        _makeFunc.Add((ushort)PacketID.C_SceneReady, MakePacket<C_SceneReady>);
        _handler.Add((ushort)PacketID.C_SceneReady, PacketHandler.C_SceneReadyHandler);

        _makeFunc.Add((ushort)PacketID.C_WeaponSelect, MakePacket<C_WeaponSelect>);
        _handler.Add((ushort)PacketID.C_WeaponSelect, PacketHandler.C_WeaponSelectHandler);

        _makeFunc.Add((ushort)PacketID.C_AccountInfoDebug, MakePacket<C_AccountInfoDebug>);
        _handler.Add((ushort)PacketID.C_AccountInfoDebug, PacketHandler.C_AccountInfoDebugHandler);

        _makeFunc.Add((ushort)PacketID.C_ReturnToLobby, MakePacket<C_ReturnToLobby>);
        _handler.Add((ushort)PacketID.C_ReturnToLobby, PacketHandler.C_ReturnToLobbyHandler);

    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> onRecvCallback = null)
    {
        ushort count = 0;

        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        Func<PacketSession, ArraySegment<byte>, IPacket> func = null;
        if (_makeFunc.TryGetValue(id, out func))
        {
            IPacket packet = func.Invoke(session, buffer);
            if (onRecvCallback != null)
                onRecvCallback.Invoke(session, packet);
            else
                HandlePacket(session, packet);
        }
    }

    T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {
        T pkt = new T();
        pkt.Read(buffer);
        return pkt;
    }

    public void HandlePacket(PacketSession session, IPacket packet)
    {
        Action<PacketSession, IPacket> action = null;
        if (_handler.TryGetValue(packet.Protocol, out action))
            action.Invoke(session, packet);
    }
}