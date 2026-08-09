using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ServerCore;

public enum PacketID
{
    // 100 - 계정
    C_ReqLoginGameServer = 100,
	S_ResLoginGameServer = 101,
	
    // 200 - 커뮤니티 (친구)


    // 300 - 파티


    // 400 - 플레이
	C_MatchStart = 400,
	C_MatchCancel = 401,

    // 500 - 스폰
    S_CreateMyCharacter = 500,
	S_CreateOtherCharacter = 501,
	S_DeleteCharacter = 502,

    // 600 - 게임 진행
	S_MatchFound = 600,
	S_GameStart = 601,
	S_GameEnd = 602,
	C_SceneReady = 603,
	C_WeaponSelect = 604,
	C_ReturnToLobby = 605,
	S_ReturnToLobby = 606,
	C_AccountInfoDebug = 607,
    C_QuitGame = 608,

    // 700 - 무기
	C_ProjectileShootStart = 700,
	S_ProjectileShootStart = 701,
	S_SpawnProjectile = 702,
	S_DespawnProjectile = 703,
	S_ProjectileHit = 704,
    C_HitscanShootStart = 720,
    S_HitscanShootStart = 721,
    S_NtfHitscanShootStart = 722,
    S_NtfHitscanShootState = 723,
    C_HitscanHit = 724,    
    S_HitscanHit = 725,
    C_ReqReloadBullet = 726,
    S_NtfUpdateBullet = 727,

    // 800 - 경험치

    // 900 - 아이템
    S_NtfSpawnItemPicker = 900,
    S_NtfDespawnItemPicker = 901,
    C_ReqPickupItemPicker = 902,


    // 1000 - 상자
    S_NtfCreateChest = 1000,
    S_NtfDestroyChest = 1001,    
    C_OpenChest = 1002, 
    C_CloseChest = 1003, // 체스트에서 멀리떨어지거나, 직접 체스트를 닫을때 송신
    S_OpenChest = 1004,
    S_CloseChest = 1005, // ->C에대한 응답은 아님, 서버쪽에서 능동적으로 닫을수도있어서 우선 만들어둠.
    S_NtfChestInfo = 1006, 
    C_ReqInventoryToChest = 1007,
    S_ResInventoryToChest = 1008,
    C_ReqChestToInventory = 1009,
    S_ResChestToInventory = 1010,
    C_OpenInventory = 1011,
    S_NtfInventoryInfo = 1012,
    C_ReqUseItem = 1013,
    S_ResUseItem = 1014,
    C_ReqDropItem = 1015,
    S_ResDropItem = 1016,
    

    // 1100 - 이동
    C_MoveStart = 1100,
	S_MoveStart = 1101,
    S_MoveState = 1102,
	S_TickSync = 1103,
	C_RotateStart = 1104,
	S_RotateStart = 1105,
    S_RotateState = 1106,

    //1200 - 채팅
    C_ChatMessage = 1200,
}

public interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}

public class C_ReqLoginGameServer : IPacket
{
    public string sessionKey;

    public ushort Protocol { get { return (ushort)PacketID.C_ReqLoginGameServer; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        ushort sessionKeyLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.sessionKey = Encoding.Unicode.GetString(s.Slice(count, sessionKeyLen));
		count += sessionKeyLen;
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_ReqLoginGameServer);
        count += sizeof(ushort);
        ushort sessionKeyLen = (ushort)Encoding.Unicode.GetBytes(this.sessionKey, 0, this.sessionKey.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), sessionKeyLen);
		count += sizeof(ushort);
		count += sessionKeyLen;
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_ResLoginGameServer : IPacket
{
    public bool loginOk;

    public ushort Protocol { get { return (ushort)PacketID.S_ResLoginGameServer; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.loginOk = BitConverter.ToBoolean(s.Slice(count, s.Length - count));
		count += sizeof(bool);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_ResLoginGameServer);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), loginOk);
		count += sizeof(bool);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}


public class S_NtfCreateMyCharacter : IPacket
{
    public int serverTick;
	public uint entityId;
	public ushort hp;
	public ushort weaponId;

    public ushort Protocol { get { return (ushort)PacketID.S_CreateMyCharacter; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.serverTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		this.entityId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
		count += sizeof(uint);
		this.hp = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.weaponId = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_CreateMyCharacter);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), serverTick);
		count += sizeof(int);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), entityId);
		count += sizeof(uint);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), hp);
		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), weaponId);
		count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_NtfCreateOtherCharacter : IPacket
{
    public int serverTick;
	public uint entityId;
	public ushort hp;
	public ushort weaponId;

    public ushort Protocol { get { return (ushort)PacketID.S_CreateOtherCharacter; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.serverTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		this.entityId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
		count += sizeof(uint);
		this.hp = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.weaponId = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_CreateOtherCharacter);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), serverTick);
		count += sizeof(int);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), entityId);
		count += sizeof(uint);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), hp);
		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), weaponId);
		count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_NtfDeleteCharacter : IPacket
{
    public int serverTick;
	public uint entityId;

    public ushort Protocol { get { return (ushort)PacketID.S_DeleteCharacter; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.serverTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		this.entityId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
		count += sizeof(uint);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_DeleteCharacter);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), serverTick);
		count += sizeof(int);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), entityId);
		count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class C_MoveStart : IPacket
{
    public int clientTick;
	public float targetX;
	public float targetY;

    public ushort Protocol { get { return (ushort)PacketID.C_MoveStart; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.clientTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		this.targetX = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
		this.targetY = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_MoveStart);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), clientTick);
		count += sizeof(int);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), targetX);
		count += sizeof(float);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), targetY);
		count += sizeof(float);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_MoveStart : IPacket
{
    public int acceptTick;
	public uint entityId;
	public float targetX;
	public float targetY;

    public ushort Protocol { get { return (ushort)PacketID.S_MoveStart; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.acceptTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		this.entityId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
		count += sizeof(uint);
		this.targetX = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
		this.targetY = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_MoveStart);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), acceptTick);
		count += sizeof(int);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), entityId);
		count += sizeof(uint);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), targetX);
		count += sizeof(float);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), targetY);
		count += sizeof(float);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_NtfMoveState : IPacket
{
    public int currentTick;
	public uint entityId;
	public float serverX; 
	public float serverY;
	public float targetX;
	public float targetY;

    public ushort Protocol { get { return (ushort)PacketID.S_MoveState; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.currentTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		this.entityId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
		count += sizeof(uint);
		this.serverX = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
		this.serverY = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
		this.targetX = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
		this.targetY = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_MoveState);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), currentTick);
		count += sizeof(int);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), entityId);
		count += sizeof(uint);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), serverX);
		count += sizeof(float);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), serverY);
		count += sizeof(float);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), targetX);
		count += sizeof(float);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), targetY);
		count += sizeof(float);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_NtfTickSync : IPacket
{
    public int serverTick;

    public ushort Protocol { get { return (ushort)PacketID.S_TickSync; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.serverTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_TickSync);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), serverTick);
		count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class C_RotateStart : IPacket
{
    public int clientTick;
	public float targetAngle;

    public ushort Protocol { get { return (ushort)PacketID.C_RotateStart; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.clientTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		this.targetAngle = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_RotateStart);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), clientTick);
		count += sizeof(int);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), targetAngle);
		count += sizeof(float);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_RotateStart : IPacket
{
    public int accpetTick;
	public uint entityId;
	public float targetAngle;

    public ushort Protocol { get { return (ushort)PacketID.S_RotateStart; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.accpetTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		this.entityId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
		count += sizeof(uint);
		this.targetAngle = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_RotateStart);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), accpetTick);
		count += sizeof(int);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), entityId);
		count += sizeof(uint);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), targetAngle);
		count += sizeof(float);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_NtfRotateState : IPacket
{
    public int currentTick;
	public uint entityId;
	public float currentAngle;
	public float targetAngle;

    public ushort Protocol { get { return (ushort)PacketID.S_RotateState; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.currentTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		this.entityId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
		count += sizeof(uint);
		this.currentAngle = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
		this.targetAngle = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_RotateState);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), currentTick);
		count += sizeof(int);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), entityId);
		count += sizeof(uint);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), currentAngle);
		count += sizeof(float);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), targetAngle);
		count += sizeof(float);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class C_ProjectileShootStart : IPacket
{
    public int clientTick;

    public ushort Protocol { get { return (ushort)PacketID.C_ProjectileShootStart; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.clientTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_ProjectileShootStart);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), clientTick);
		count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_ShootStart : IPacket
{
    public int accpetTick;
	public uint entityId;

    public ushort Protocol { get { return (ushort)PacketID.S_ProjectileShootStart; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.accpetTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		this.entityId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
		count += sizeof(uint);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_ProjectileShootStart);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), accpetTick);
		count += sizeof(int);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), entityId);
		count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_NtfSpawnProjectile : IPacket
{
    public int currentTick;
	public uint entityId;
	public float spawnPosX;
	public float spawnPosY;
	public float targetPosX;
	public float targetPosY;

    public ushort Protocol { get { return (ushort)PacketID.S_SpawnProjectile; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.currentTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		this.entityId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
		count += sizeof(uint);
		this.spawnPosX = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
		this.spawnPosY = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
		this.targetPosX = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
		this.targetPosY = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_SpawnProjectile);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), currentTick);
		count += sizeof(int);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), entityId);
		count += sizeof(uint);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), spawnPosX);
		count += sizeof(float);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), spawnPosY);
		count += sizeof(float);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), targetPosX);
		count += sizeof(float);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), targetPosY);
		count += sizeof(float);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_DespawnProjectile : IPacket
{
    public int currentTick;
	public uint entityId;

    public ushort Protocol { get { return (ushort)PacketID.S_DespawnProjectile; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.currentTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		this.entityId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
		count += sizeof(uint);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_DespawnProjectile);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), currentTick);
		count += sizeof(int);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), entityId);
		count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_NtfProjectileHit : IPacket
{
    public int currentTick;
	public ushort damage;
	public uint bulletId;
	public uint collisionId;

    public ushort Protocol { get { return (ushort)PacketID.S_ProjectileHit; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.currentTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		this.damage = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.bulletId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
		count += sizeof(uint);
		this.collisionId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
		count += sizeof(uint);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_ProjectileHit);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), currentTick);
		count += sizeof(int);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), damage);
		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), bulletId);
		count += sizeof(uint);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), collisionId);
		count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class C_HitscanShootStart : IPacket
{
    public int clientTick;
    public float startX;
    public float startY;
    public float endX;
    public float endY;

    public ushort Protocol { get { return (ushort)PacketID.C_HitscanShootStart; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.clientTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.startX = BitConverter.ToSingle(s.Slice(count, s.Length - count));
        count += sizeof(float);
        this.startY = BitConverter.ToSingle(s.Slice(count, s.Length - count));
        count += sizeof(float);
        this.endX = BitConverter.ToSingle(s.Slice(count, s.Length - count));
        count += sizeof(float);
        this.endY = BitConverter.ToSingle(s.Slice(count, s.Length - count));
        count += sizeof(float);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_HitscanShootStart);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), clientTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), startX);
        count += sizeof(float);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), startY);
        count += sizeof(float);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), endX);
        count += sizeof(float);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), endY);
        count += sizeof(float);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_HitscanShootStart : IPacket
{
    public int acceptTick;    
    public uint entityId;
    public float startX;
    public float startY;
    public float endX;
    public float endY;

    public ushort Protocol { get { return (ushort)PacketID.S_HitscanShootStart; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.acceptTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.entityId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
        count += sizeof(uint);
        this.startX = BitConverter.ToSingle(s.Slice(count, s.Length - count));
        count += sizeof(float);
        this.startY = BitConverter.ToSingle(s.Slice(count, s.Length - count));
        count += sizeof(float);
        this.endX = BitConverter.ToSingle(s.Slice(count, s.Length - count));
        count += sizeof(float);
        this.endY = BitConverter.ToSingle(s.Slice(count, s.Length - count));
        count += sizeof(float);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_HitscanShootStart);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), acceptTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), entityId);
        count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), startX);
        count += sizeof(float);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), startY);
        count += sizeof(float);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), endX);
        count += sizeof(float);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), endY);
        count += sizeof(float);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_NtfHitscanShootStart : IPacket
{
    public int accpetTick;
    public uint accountId;

    public ushort Protocol { get { return (ushort)PacketID.S_NtfHitscanShootStart; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.accpetTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.accountId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
        count += sizeof(uint);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_NtfHitscanShootStart);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), accpetTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), accountId);
        count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_NtfHitscanShootState : IPacket
{
    public int serverTick;
    public uint accountId;
    public ulong bulletMaskLow;
    public ulong bulletMaskHigh;
    public int baseTick;

    public ushort Protocol { get { return (ushort)PacketID.S_NtfHitscanShootState; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.serverTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.accountId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
        count += sizeof(uint);
        this.bulletMaskLow = BitConverter.ToUInt64(s.Slice(count, s.Length - count));
        count += sizeof(ulong);
        this.bulletMaskHigh = BitConverter.ToUInt64(s.Slice(count, s.Length - count));
        count += sizeof(ulong);
        this.baseTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_NtfHitscanShootState);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), serverTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), accountId);
        count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), bulletMaskLow);
        count += sizeof(ulong);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), bulletMaskHigh);
        count += sizeof(ulong);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), baseTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}

public class C_HitscanHit : IPacket
{
    public int currentTick;
    public uint shooterId;
    public uint targetId;    

    public ushort Protocol { get { return (ushort)PacketID.C_HitscanHit; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.currentTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.shooterId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
        count += sizeof(uint);
        this.targetId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
        count += sizeof(uint);     
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_HitscanHit);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), currentTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), shooterId);
        count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), targetId);
        count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}

public class S_HitscanHit : IPacket
{
    public int serverTick;
    public ushort damage;
    public uint shooterId;
    public uint targetId;

    public ushort Protocol { get { return (ushort)PacketID.S_HitscanHit; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.serverTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.damage = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
        count += sizeof(ushort);
        this.shooterId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
        count += sizeof(uint);
        this.targetId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
        count += sizeof(uint);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_HitscanHit);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), serverTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), damage);
        count += sizeof(short);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), shooterId);
        count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), targetId);
        count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}

public class S_NtfSpawnItemPicker : IPacket
{
    public int serverTick;
    public uint entityId;
    public uint itemType;    
    public float targetX;
    public float targetY;    

    public ushort Protocol { get { return (ushort)PacketID.S_NtfSpawnItemPicker; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.serverTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.entityId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
        count += sizeof(uint);
        this.itemType = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
        count += sizeof(uint);
        this.targetX = BitConverter.ToSingle(s.Slice(count, s.Length - count));
        count += sizeof(float);
        this.targetY = BitConverter.ToSingle(s.Slice(count, s.Length - count));
        count += sizeof(float);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_NtfSpawnItemPicker);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), serverTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), entityId);
        count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), itemType);
        count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), targetX);
        count += sizeof(float);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), targetY);
        count += sizeof(float);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_NtfDespawnItemPicker : IPacket
{
    public int serverTick;
    public uint entityId;    
    public uint itemType;
    public bool isObtained;
    public uint obtainEntityId;


    public ushort Protocol { get { return (ushort)PacketID.S_NtfDespawnItemPicker; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.serverTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.entityId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
        count += sizeof(uint);
        this.itemType = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
        count += sizeof(uint);
        this.isObtained = BitConverter.ToBoolean(s.Slice(count, s.Length - count));
        count += sizeof(bool);
        this.obtainEntityId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
        count += sizeof(uint);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_NtfDespawnItemPicker);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), serverTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), entityId);
        count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), itemType);
        count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), isObtained);
        count += sizeof(bool);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), obtainEntityId);
        count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class C_ReqPickupItemPicker : IPacket
{
    public int clientTick;
    public uint entityId;

    public ushort Protocol { get { return (ushort)PacketID.C_ReqPickupItemPicker; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.clientTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.entityId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
        count += sizeof(uint);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_ReqPickupItemPicker);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), clientTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), entityId);
        count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}



public class C_MatchStart : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.C_MatchStart; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_MatchStart);
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class C_MatchCancel : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.C_MatchCancel; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_MatchCancel);
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_MatchFound : IPacket
{
    public bool success;

    public ushort Protocol { get { return (ushort)PacketID.S_MatchFound; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.success = BitConverter.ToBoolean(s.Slice(count, s.Length - count));
		count += sizeof(bool);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_MatchFound);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), success);
		count += sizeof(bool);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_GameStart : IPacket
{
    public bool success;
	public int tick;

    public ushort Protocol { get { return (ushort)PacketID.S_GameStart; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.success = BitConverter.ToBoolean(s.Slice(count, s.Length - count));
		count += sizeof(bool);
		this.tick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_GameStart);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), success);
		count += sizeof(bool);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), tick);
		count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_GameEnd : IPacket
{
    public bool win;

    public ushort Protocol { get { return (ushort)PacketID.S_GameEnd; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.win = BitConverter.ToBoolean(s.Slice(count, s.Length - count));
		count += sizeof(bool);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_GameEnd);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), win);
		count += sizeof(bool);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class C_SceneReady : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.C_SceneReady; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_SceneReady);
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class C_WeaponSelect : IPacket
{
    public ushort weaponId;

    public ushort Protocol { get { return (ushort)PacketID.C_WeaponSelect; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.weaponId = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_WeaponSelect);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), weaponId);
		count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class C_AccountInfoDebug : IPacket
{
    public long accountId;

    public ushort Protocol { get { return (ushort)PacketID.C_AccountInfoDebug; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.accountId = BitConverter.ToInt64(s.Slice(count, s.Length - count));
		count += sizeof(long);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_AccountInfoDebug);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), accountId);
		count += sizeof(long);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}

public class C_QuitGame : IPacket
{

    public ushort Protocol { get { return (ushort)PacketID.C_QuitGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_QuitGame);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}

public class C_ReturnToLobby : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.C_ReturnToLobby; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_ReturnToLobby);
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_ReturnToLobby : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.S_ReturnToLobby; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_ReturnToLobby);
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}

public class S_NtfCreateChest : IPacket
{
    public int serverTick;
    public uint entityId;
    public float targetX;
    public float targetY;

    public ushort Protocol { get { return (ushort)PacketID.S_NtfCreateChest; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.serverTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.entityId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
        count += sizeof(uint);
        this.targetX = BitConverter.ToSingle(s.Slice(count, s.Length - count));
        count += sizeof(float);
        this.targetY = BitConverter.ToSingle(s.Slice(count, s.Length - count));
        count += sizeof(float);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_NtfCreateChest);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), serverTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), entityId);
        count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), targetX);
        count += sizeof(float);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), targetY);
        count += sizeof(float);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}

public class S_NtfDestroyChest : IPacket
{
    public int serverTick;
    public uint entityId;    

    public ushort Protocol { get { return (ushort)PacketID.S_NtfDestroyChest; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.serverTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.entityId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
        count += sizeof(uint);        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_NtfDestroyChest);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), serverTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), entityId);
        count += sizeof(uint);        
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class C_OpenChest : IPacket
{
    public int clientTick;
    public uint chestId;

    public ushort Protocol { get { return (ushort)PacketID.C_OpenChest; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.clientTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.chestId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
        count += sizeof(uint);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_OpenChest);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), clientTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), chestId);
        count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}

public class C_CloseChest : IPacket
{
    public int clientTick;
    public uint chestId;

    public ushort Protocol { get { return (ushort)PacketID.C_CloseChest; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.clientTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.chestId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
        count += sizeof(uint);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_CloseChest);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), clientTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), chestId);
        count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_OpenChest : IPacket
{
    public int serverTick;
    public uint chestId;
    public ushort resultCode;   

    public ushort Protocol { get { return (ushort)PacketID.S_OpenChest; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.serverTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.chestId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
        count += sizeof(uint);
        this.resultCode = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
        count += sizeof(ushort);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_OpenChest);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), serverTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), chestId);
        count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), resultCode);
        count += sizeof(ushort);
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}

public class S_CloseChest : IPacket
{
    public int serverTick;
    public uint chestId;
    public ushort resultCode; //필요하면 이유도 전송하게.

    public ushort Protocol { get { return (ushort)PacketID.S_CloseChest; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.serverTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.chestId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
        count += sizeof(uint);
        this.resultCode = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
        count += sizeof(ushort);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_CloseChest);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), serverTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), chestId);
        count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), resultCode);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}

public class S_NtfChestInfo : IPacket
{
    public int serverTick;
    public uint chestId;

    public class ItemList
    {
        public uint cursor;
        public uint itemType;
        public uint quantity;

        public void Read(ReadOnlySpan<byte> s, ref ushort count)
        {
            this.cursor = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
            count += sizeof(uint);
            this.itemType = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
            count += sizeof(uint);
            this.quantity = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
            count += sizeof(uint);
        }

        public bool Write(Span<byte> s, ArraySegment<byte> segment, ref ushort count)
        {
            bool success = true;
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), cursor);
            count += sizeof(uint);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), itemType);
            count += sizeof(uint);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), quantity);
            count += sizeof(uint);
            return success;
        }
    }

    public List<ItemList> itemLists = new List<ItemList>();


    public ushort Protocol { get { return (ushort)PacketID.S_NtfChestInfo; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.serverTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.chestId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
        count += sizeof(uint);
        this.itemLists.Clear();
        ushort itemListLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
        count += sizeof(ushort);
        for (int i = 0; i < itemListLen; i++)
        {
            ItemList itemList = new ItemList();
            itemList.Read(s, ref count);
            itemLists.Add(itemList);
        }
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_NtfChestInfo);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), serverTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), chestId);
        count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)this.itemLists.Count);
        count += sizeof(ushort);
        foreach (ItemList itemList in this.itemLists)
            success &= itemList.Write(s, segment, ref count);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class C_ReqInventoryToChest : IPacket
{
    public int clientTick;
    public int lastChestUpdateTick;
    public int lastInventoryUpdateTick;
    public uint entityId;
    public uint inventoryCursor;
    public uint chestId;

    public ushort Protocol { get { return (ushort)PacketID.C_ReqInventoryToChest; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.clientTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.lastChestUpdateTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.lastInventoryUpdateTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.entityId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
        count += sizeof(uint);
        this.inventoryCursor = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
        count += sizeof(uint);
        this.chestId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
        count += sizeof(uint);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_ReqInventoryToChest);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), clientTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), lastChestUpdateTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), lastInventoryUpdateTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), entityId);
        count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), inventoryCursor);
        count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), chestId);
        count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_ResInventoryToChest : IPacket
{
    public int serverTick;
    public ushort resultCode;

    public ushort Protocol { get { return (ushort)PacketID.S_ResInventoryToChest; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.serverTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.resultCode = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
        count += sizeof(ushort);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_ResInventoryToChest);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), serverTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), resultCode);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class C_ReqChestToInventory : IPacket
{
    public int clientTick;
    public int lastChestUpdateTick;
    public int lastInventoryUpdateTick;
    public uint entityId;
    public uint chestId;
    public uint chestCursor;

    public ushort Protocol { get { return (ushort)PacketID.C_ReqChestToInventory; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.clientTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.lastChestUpdateTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.lastInventoryUpdateTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.entityId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
        count += sizeof(uint);
        this.chestId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
        count += sizeof(uint);
        this.chestCursor = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
        count += sizeof(uint);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_ReqChestToInventory);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), clientTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), lastChestUpdateTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), lastInventoryUpdateTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), entityId);
        count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), chestId);
        count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), chestCursor);
        count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_ResChestToInventory : IPacket
{
    public int serverTick;
    public ushort resultCode;

    public ushort Protocol { get { return (ushort)PacketID.S_ResChestToInventory; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.serverTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.resultCode = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
        count += sizeof(ushort);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_ResChestToInventory);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), serverTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), resultCode);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class C_OpenInventory : IPacket
{
    public int clientTick;

    public ushort Protocol { get { return (ushort)PacketID.C_OpenInventory; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.clientTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_OpenInventory);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), clientTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_NtfInventoryInfo : IPacket
{
    public int serverTick;
    public class ItemList
    {
        public uint cursor;
        public uint itemId;
        public uint quantity;

        public void Read(ReadOnlySpan<byte> s, ref ushort count)
        {
            this.cursor = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
            count += sizeof(uint);
            this.itemId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
            count += sizeof(uint);
            this.quantity = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
            count += sizeof(uint);
        }

        public bool Write(Span<byte> s, ArraySegment<byte> segment, ref ushort count)
        {
            bool success = true;
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), cursor);
            count += sizeof(uint);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), itemId);
            count += sizeof(uint);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), quantity);
            count += sizeof(uint);
            return success;
        }
    }

    public List<ItemList> itemLists = new List<ItemList>();


    public ushort Protocol { get { return (ushort)PacketID.S_NtfInventoryInfo; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.serverTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.itemLists.Clear();
        ushort itemListLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
        count += sizeof(ushort);
        for (int i = 0; i < itemListLen; i++)
        {
            ItemList itemList = new ItemList();
            itemList.Read(s, ref count);
            itemLists.Add(itemList);
        }
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_NtfInventoryInfo);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), serverTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)this.itemLists.Count);
        count += sizeof(ushort);
        foreach (ItemList itemList in this.itemLists)
            success &= itemList.Write(s, segment, ref count);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}

public class C_ReqUseItem : IPacket
{
    public int clientTick;
    public int lastInventoryUpdateTick;    
    public uint inventoryCursor;    

    public ushort Protocol { get { return (ushort)PacketID.C_ReqUseItem; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.clientTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.lastInventoryUpdateTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.inventoryCursor = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
        count += sizeof(uint);        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_ReqUseItem);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), clientTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), lastInventoryUpdateTick);
        count += sizeof(int);        
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), inventoryCursor);
        count += sizeof(uint);        
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}

public class C_ReqDropItem : IPacket
{
    public int clientTick;
    public int lastInventoryUpdateTick;
    public uint inventoryCursor;

    public ushort Protocol { get { return (ushort)PacketID.C_ReqDropItem; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.clientTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.lastInventoryUpdateTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.inventoryCursor = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
        count += sizeof(uint);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_ReqDropItem);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), clientTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), lastInventoryUpdateTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), inventoryCursor);
        count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}

public class C_ChatMessage : IPacket
{
    public string message;

    public ushort Protocol { get { return (ushort)PacketID.C_ChatMessage; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        ushort messageLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
        count += sizeof(ushort);
        this.message = Encoding.Unicode.GetString(s.Slice(count, messageLen));
        count += messageLen;
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_ReqLoginGameServer);
        count += sizeof(ushort);
        ushort messageLen = (ushort)Encoding.Unicode.GetBytes(this.message, 0, this.message.Length, segment.Array, segment.Offset + count + sizeof(ushort));
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), messageLen);
        count += sizeof(ushort);
        count += messageLen;
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}

public class C_ReqReloadBullet : IPacket
{
    public int clientTick;    

    public ushort Protocol { get { return (ushort)PacketID.C_ReqReloadBullet; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.clientTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_ReqReloadBullet);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), clientTick);
        count += sizeof(int);        
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}

public class S_NtfUpdateBullet : IPacket
{
    public int serverTick;
    public ushort bulletCount;


    public ushort Protocol { get { return (ushort)PacketID.S_NtfUpdateBullet; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.serverTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.bulletCount = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
        count += sizeof(ushort);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_NtfUpdateBullet);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), serverTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), bulletCount);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
