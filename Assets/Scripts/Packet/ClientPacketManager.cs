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
        _makeFunc.Add((ushort)PacketID.S_NtfCreateMyCharacter, MakePacket<S_NtfCreateMyCharacter>);
        _handler.Add((ushort)PacketID.S_NtfCreateMyCharacter, PacketHandler.S_NtfCreateMyCharacterHandler);

        _makeFunc.Add((ushort)PacketID.S_ResLoginChatServer, MakePacket<S_ResLoginChatServer>);
        _handler.Add((ushort)PacketID.S_ResLoginChatServer, PacketHandler.S_ResLoginChatServerHandler);

        _makeFunc.Add((ushort)PacketID.S_NtfAccountInfo, MakePacket<S_NtfAccountInfo>);
        _handler.Add((ushort)PacketID.S_NtfAccountInfo, PacketHandler.S_NtfAccountInfoHandler);

        _makeFunc.Add((ushort)PacketID.S_NtfCreateOtherCharacter, MakePacket<S_NtfCreateOtherCharacter>);
        _handler.Add((ushort)PacketID.S_NtfCreateOtherCharacter, PacketHandler.S_NtfCreateOtherCharacterHandler);

        _makeFunc.Add((ushort)PacketID.S_DeleteCharacter, MakePacket<S_NtfDeleteCharacter>);
        _handler.Add((ushort)PacketID.S_DeleteCharacter, PacketHandler.S_NtfDeleteCharacterHandler);

        _makeFunc.Add((ushort)PacketID.S_NtfSpectateUser, MakePacket<S_NtfSpectateUser>);
        _handler.Add((ushort)PacketID.S_NtfSpectateUser, PacketHandler.S_NtfSpectateUserHandler);

        _makeFunc.Add((ushort)PacketID.S_MoveStart, MakePacket<S_MoveStart>);
        _handler.Add((ushort)PacketID.S_MoveStart, PacketHandler.S_MoveStartHandler);

        _makeFunc.Add((ushort)PacketID.S_MoveState, MakePacket<S_NtfMoveState>);
        _handler.Add((ushort)PacketID.S_MoveState, PacketHandler.S_NtfMoveStateHandler);

        _makeFunc.Add((ushort)PacketID.S_TickSync, MakePacket<S_NtfTickSync>);
        _handler.Add((ushort)PacketID.S_TickSync, PacketHandler.S_NtfTickSyncHandler);

        _makeFunc.Add((ushort)PacketID.S_RotateStart, MakePacket<S_RotateStart>);
        _handler.Add((ushort)PacketID.S_RotateStart, PacketHandler.S_RotateStartHandler);

        _makeFunc.Add((ushort)PacketID.S_RotateState, MakePacket<S_NtfRotateState>);
        _handler.Add((ushort)PacketID.S_RotateState, PacketHandler.S_NtfRotateStateHandler);

        _makeFunc.Add((ushort)PacketID.S_ProjectileShootStart, MakePacket<S_ShootStart>);
        _handler.Add((ushort)PacketID.S_ProjectileShootStart, PacketHandler.S_ProjectileShootStartHandler);

        _makeFunc.Add((ushort)PacketID.S_SpawnProjectile, MakePacket<S_NtfSpawnProjectile>);
        _handler.Add((ushort)PacketID.S_SpawnProjectile, PacketHandler.S_NtfSpawnProjectileHandler);

        _makeFunc.Add((ushort)PacketID.S_DespawnProjectile, MakePacket<S_DespawnProjectile>);
        _handler.Add((ushort)PacketID.S_DespawnProjectile, PacketHandler.S_NtfDespawnProjectileHandler);

        _makeFunc.Add((ushort)PacketID.S_ProjectileHit, MakePacket<S_NtfProjectileHit>);
        _handler.Add((ushort)PacketID.S_ProjectileHit, PacketHandler.S_NtfProjectileHitHandler);

        _makeFunc.Add((ushort)PacketID.S_HitscanShootStart, MakePacket<S_HitscanShootStart>);
        _handler.Add((ushort)PacketID.S_HitscanShootStart, PacketHandler.S_HitscanShootStartHandler);

        _makeFunc.Add((ushort)PacketID.S_HitscanHit, MakePacket<S_HitscanHit>);
        _handler.Add((ushort)PacketID.S_HitscanHit, PacketHandler.S_HitscanHitHandler);

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

        _makeFunc.Add((ushort)PacketID.S_NtfCreateChest, MakePacket<S_NtfCreateChest>);
        _handler.Add((ushort)PacketID.S_NtfCreateChest, PacketHandler.S_NtfCreateChestHandler);

        _makeFunc.Add((ushort)PacketID.S_NtfDestroyChest, MakePacket<S_NtfDestroyChest>);
        _handler.Add((ushort)PacketID.S_NtfDestroyChest, PacketHandler.S_NtfDestroyChestHandler);

        _makeFunc.Add((ushort)PacketID.S_OpenChest, MakePacket<S_OpenChest>);
        _handler.Add((ushort)PacketID.S_OpenChest, PacketHandler.S_OpenChestHandler);

        _makeFunc.Add((ushort)PacketID.S_CloseChest, MakePacket<S_CloseChest>);
        _handler.Add((ushort)PacketID.S_CloseChest, PacketHandler.S_CloseChestHandler);
        
        _makeFunc.Add((ushort)PacketID.S_NtfChestInfo, MakePacket<S_NtfChestInfo>);
        _handler.Add((ushort)PacketID.S_NtfChestInfo, PacketHandler.S_NtfChestInfoHandler);        

        _makeFunc.Add((ushort)PacketID.S_ResInventoryToChest, MakePacket<S_ResInventoryToChest>);
        _handler.Add((ushort)PacketID.S_ResInventoryToChest, PacketHandler.S_ResInventoryToChestHandler);

        _makeFunc.Add((ushort)PacketID.S_ResChestToInventory, MakePacket<S_ResChestToInventory>);
        _handler.Add((ushort)PacketID.S_ResChestToInventory, PacketHandler.S_ResChestToInventoryHandler);

        _makeFunc.Add((ushort)PacketID.S_NtfInventoryInfo, MakePacket<S_NtfInventoryInfo>);
        _handler.Add((ushort)PacketID.S_NtfInventoryInfo, PacketHandler.S_ResInventoryInfoHandler);

        _makeFunc.Add((ushort)PacketID.S_ResLoginGameServer, MakePacket<S_ResLoginGameServer>);
        _handler.Add((ushort)PacketID.S_ResLoginGameServer, PacketHandler.S_ResLoginGameServerHandler);

        _makeFunc.Add((ushort)PacketID.S_NtfUpdateBullet, MakePacket<S_NtfUpdateBullet>);
        _handler.Add((ushort)PacketID.S_NtfUpdateBullet, PacketHandler.S_NtfUpdateBulletHandler);

        //friend
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