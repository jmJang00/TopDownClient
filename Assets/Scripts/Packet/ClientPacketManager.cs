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

        _makeFunc.Add((ushort)PacketID.S_MoveState, MakePacket<S_MoveState>);
        _handler.Add((ushort)PacketID.S_MoveState, PacketHandler.S_MoveStateHandler);

        _makeFunc.Add((ushort)PacketID.S_TickSync, MakePacket<S_TickSync>);
        _handler.Add((ushort)PacketID.S_TickSync, PacketHandler.S_TickSyncHandler);

        _makeFunc.Add((ushort)PacketID.S_RotateStart, MakePacket<S_RotateStart>);
        _handler.Add((ushort)PacketID.S_RotateStart, PacketHandler.S_RotateStartHandler);

        _makeFunc.Add((ushort)PacketID.S_RotateState, MakePacket<S_RotateState>);
        _handler.Add((ushort)PacketID.S_RotateState, PacketHandler.S_RotateStateHandler);

        _makeFunc.Add((ushort)PacketID.S_ProjectileShootStart, MakePacket<S_ProjectileShootStart>);
        _handler.Add((ushort)PacketID.S_ProjectileShootStart, PacketHandler.S_ProjectileShootStartHandler);

        _makeFunc.Add((ushort)PacketID.S_SpawnProjectile, MakePacket<S_SpawnProjectile>);
        _handler.Add((ushort)PacketID.S_SpawnProjectile, PacketHandler.S_SpawnProjectileHandler);

        _makeFunc.Add((ushort)PacketID.S_DespawnProjectile, MakePacket<S_DespawnProjectile>);
        _handler.Add((ushort)PacketID.S_DespawnProjectile, PacketHandler.S_DespawnProjectileHandler);

        _makeFunc.Add((ushort)PacketID.S_ProjectileHit, MakePacket<S_ProjectileHit>);
        _handler.Add((ushort)PacketID.S_ProjectileHit, PacketHandler.S_ProjectileHitHandler);

        _makeFunc.Add((ushort)PacketID.S_NtfSpawnItemPicker, MakePacket<S_NtfSpawnItemPicker>);
        _handler.Add((ushort)PacketID.S_NtfSpawnItemPicker, PacketHandler.S_NtfSpawnItemPickerHandler);

        _makeFunc.Add((ushort)PacketID.S_NtfDespawnItemPicker, MakePacket<S_NtfDespawnItemPicker>);
        _handler.Add((ushort)PacketID.S_NtfDespawnItemPicker, PacketHandler.S_NtfDespawnItemPickerHandler);

        _makeFunc.Add((ushort)PacketID.C_ReqPickupItemPicker, MakePacket<C_ReqPickupItemPicker>);
        _handler.Add((ushort)PacketID.C_ReqPickupItemPicker, PacketHandler.C_ReqPickupItemPickerHandler);

        _makeFunc.Add((ushort)PacketID.S_MatchFound, MakePacket<S_MatchFound>);
        _handler.Add((ushort)PacketID.S_MatchFound, PacketHandler.S_MatchFoundHandler);

        _makeFunc.Add((ushort)PacketID.S_GameStart, MakePacket<S_GameStart>);
        _handler.Add((ushort)PacketID.S_GameStart, PacketHandler.S_GameStartHandler);

        _makeFunc.Add((ushort)PacketID.S_GameEnd, MakePacket<S_GameEnd>);
        _handler.Add((ushort)PacketID.S_GameEnd, PacketHandler.S_GameEndHandler);

        _makeFunc.Add((ushort)PacketID.S_ReturnToLobby, MakePacket<S_ReturnToLobby>);
        _handler.Add((ushort)PacketID.S_ReturnToLobby, PacketHandler.S_ReturnToLobbyHandler);

        _makeFunc.Add((ushort)PacketID.S_CreateChest, MakePacket<S_CreateChest>);
        _handler.Add((ushort)PacketID.S_CreateChest, PacketHandler.S_CreateChestHandler);

        _makeFunc.Add((ushort)PacketID.S_ResChestInfo, MakePacket<S_ResChestInfo>);
        _handler.Add((ushort)PacketID.S_ResChestInfo, PacketHandler.S_ResChestInfoHandler);

        _makeFunc.Add((ushort)PacketID.S_ResInventoryToChest, MakePacket<S_ResInventoryToChest>);
        _handler.Add((ushort)PacketID.S_ResInventoryToChest, PacketHandler.S_ResInventoryToChestHandler);

        _makeFunc.Add((ushort)PacketID.S_ResChestToInventory, MakePacket<S_ResChestToInventory>);
        _handler.Add((ushort)PacketID.S_ResChestToInventory, PacketHandler.S_ResChestToInventoryHandler);

        _makeFunc.Add((ushort)PacketID.S_ResInventoryInfo, MakePacket<S_ResInventoryInfo>);
        _handler.Add((ushort)PacketID.S_ResInventoryInfo, PacketHandler.S_ResInventoryInfoHandler);
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