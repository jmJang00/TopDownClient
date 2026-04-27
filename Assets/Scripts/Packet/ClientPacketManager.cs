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

        _makeFunc.Add((ushort)PacketID.S_CreateMyCharacter, MakePacket<S_CreateMyCharacter>);
        _handler.Add((ushort)PacketID.S_CreateMyCharacter, PacketHandler.S_CreateMyCharacterHandler);

        _makeFunc.Add((ushort)PacketID.S_CreateOtherCharacter, MakePacket<S_CreateOtherCharacter>);
        _handler.Add((ushort)PacketID.S_CreateOtherCharacter, PacketHandler.S_CreateOtherCharacterHandler);

        _makeFunc.Add((ushort)PacketID.S_DeleteCharacter, MakePacket<S_DeleteCharacter>);
        _handler.Add((ushort)PacketID.S_DeleteCharacter, PacketHandler.S_DeleteCharacterHandler);

        _makeFunc.Add((ushort)PacketID.S_MoveStart, MakePacket<S_MoveStart>);
        _handler.Add((ushort)PacketID.S_MoveStart, PacketHandler.S_MoveStartHandler);

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