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

        _makeFunc.Add((ushort)PacketID.C_RegLoginGameServer, MakePacket<C_RegLoginGameServer>);
        _handler.Add((ushort)PacketID.C_RegLoginGameServer, PacketHandler.C_RegLoginGameServerHandler);

        _makeFunc.Add((ushort)PacketID.C_SendFriendReq, MakePacket<C_SendFriendReq>);
        _handler.Add((ushort)PacketID.C_SendFriendReq, PacketHandler.C_SendFriendReqHandler);

        _makeFunc.Add((ushort)PacketID.C_FriendReqRespons, MakePacket<C_FriendReqRespons>);
        _handler.Add((ushort)PacketID.C_FriendReqRespons, PacketHandler.C_FriendReqResponsHandler);

        _makeFunc.Add((ushort)PacketID.C_RemoveFriend, MakePacket<C_RemoveFriend>);
        _handler.Add((ushort)PacketID.C_RemoveFriend, PacketHandler.C_RemoveFriendHandler);

        _makeFunc.Add((ushort)PacketID.C_FriendList, MakePacket<C_FriendList>);
        _handler.Add((ushort)PacketID.C_FriendList, PacketHandler.C_FriendListHandler);

        _makeFunc.Add((ushort)PacketID.C_FriendReqList, MakePacket<C_FriendReqList>);
        _handler.Add((ushort)PacketID.C_FriendReqList, PacketHandler.C_FriendReqListHandler);

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