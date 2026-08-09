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

        _makeFunc.Add((ushort)PacketID.S_ResLoginGameServer, MakePacket<S_ResLoginGameServer>);
        _handler.Add((ushort)PacketID.S_ResLoginGameServer, PacketHandler.S_ResLoginGameServerHandler);

        _makeFunc.Add((ushort)PacketID.S_SendFriendReqResult, MakePacket<S_SendFriendReqResult>);
        _handler.Add((ushort)PacketID.S_SendFriendReqResult, PacketHandler.S_SendFriendReqResultHandler);

        _makeFunc.Add((ushort)PacketID.S_FriendReqFromOther, MakePacket<S_FriendReqFromOther>);
        _handler.Add((ushort)PacketID.S_FriendReqFromOther, PacketHandler.S_FriendReqFromOtherHandler);

        _makeFunc.Add((ushort)PacketID.S_FriendReqResFromTarget, MakePacket<S_FriendReqResFromTarget>);
        _handler.Add((ushort)PacketID.S_FriendReqResFromTarget, PacketHandler.S_FriendReqResFromTargetHandler);

        _makeFunc.Add((ushort)PacketID.S_RemoveFriendResult, MakePacket<S_RemoveFriendResult>);
        _handler.Add((ushort)PacketID.S_RemoveFriendResult, PacketHandler.S_RemoveFriendResultHandler);

        _makeFunc.Add((ushort)PacketID.S_FriendList, MakePacket<S_FriendList>);
        _handler.Add((ushort)PacketID.S_FriendList, PacketHandler.S_FriendListHandler);

        _makeFunc.Add((ushort)PacketID.S_FriendReqList, MakePacket<S_FriendReqList>);
        _handler.Add((ushort)PacketID.S_FriendReqList, PacketHandler.S_FriendReqListHandler);

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