using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ServerCore;

public enum PacketID
{
    // 100 - 계정
	
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

    // 700 - 무기
	C_ProjectileShootStart = 700,
	S_ProjectileShootStart = 701,
	S_SpawnProjectile = 702,
	S_DespawnProjectile = 703,
	S_ProjectileHit = 704,
    C_HitscanShootStart = 720,
    S_HitscanShootStart = 721,
    S_HitscanShootState = 722,

    // 800 - 경험치

    // 900 - 아이템
    S_NtfSpawnItemPicker = 900,
    S_NtfDespawnItemPicker = 901,
    C_ReqPickupItemPicker = 902,


    // 1000 - 상자
    S_NtfCreateChest = 1000,
    S_NtfDestroyChest = 1001,
    C_ReqChestInfo = 1002,
    S_ResChestInfo = 1003,
    C_ReqInventoryToChest = 1004,
    S_ResInventoryToChest = 1005,
    C_ReqChestToInventory = 1006,
    S_ResChestToInventory = 1007,
    C_ReqInventoryInfo = 1008,
    S_ResInventoryInfo = 1009,

    // 1100 - 이동
    C_MoveStart = 1100,
	S_MoveStart = 1101,
    S_MoveState = 1102,
	S_TickSync = 1103,
	C_RotateStart = 1104,
	S_RotateStart = 1105,
    S_RotateState = 1106,
}

public interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}

public class S_CreateMyCharacter : IPacket
{
    public int serverTick;
	public uint entityId;

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
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_CreateOtherCharacter : IPacket
{
    public int serverTick;
	public uint entityId;

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
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_DeleteCharacter : IPacket
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
public class S_MoveState : IPacket
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
public class S_TickSync : IPacket
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
public class S_RotateState : IPacket
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
public class S_ProjectileShootStart : IPacket
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
public class S_SpawnProjectile : IPacket
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
public class S_ProjectileHit : IPacket
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

    public ushort Protocol { get { return (ushort)PacketID.C_HitscanShootStart; } }

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
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_HitscanShootStart);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), clientTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_HitscanShootStart : IPacket
{
    public int accpetTick;
    public uint accountId;

    public ushort Protocol { get { return (ushort)PacketID.S_HitscanShootStart; } }

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
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_HitscanShootStart);
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
public class S_HitscanShootState : IPacket
{
    public int currentTick;
    public uint accountId;
    public ulong bulletMaskLow;
    public ulong bulletMaskHigh;
    public int baseTick;

    public ushort Protocol { get { return (ushort)PacketID.S_HitscanShootState; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.currentTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
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
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_HitscanShootState);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), currentTick);
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
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class C_ReqPickupItemPicker : IPacket
{
    public int clientTick;
    public uint ItemUniqueId;

    public ushort Protocol { get { return (ushort)PacketID.C_ReqPickupItemPicker; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.clientTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.ItemUniqueId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
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
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), ItemUniqueId);
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
	public ushort weaponId;

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
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_GameStart);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), success);
		count += sizeof(bool);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), tick);
		count += sizeof(int);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), weaponId);
		count += sizeof(ushort);
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
public class C_ReqChestInfo : IPacket
{
    public int clientTick;
    public uint chestId;

    public ushort Protocol { get { return (ushort)PacketID.C_ReqChestInfo; } }

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
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_ReqChestInfo);
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
public class S_ResChestInfo : IPacket
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


    public ushort Protocol { get { return (ushort)PacketID.S_ResChestInfo; } }

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
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_ResChestInfo);
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
    public uint accountId;
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
        this.accountId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
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
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), accountId);
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
    public uint resultCode;

    public ushort Protocol { get { return (ushort)PacketID.S_ResInventoryToChest; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.serverTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.resultCode = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
        count += sizeof(uint);
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
        count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class C_ReqChestToInventory : IPacket
{
    public int clientTick;
    public uint accountId;
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
        this.accountId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
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
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), accountId);
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
    public uint resultCode;

    public ushort Protocol { get { return (ushort)PacketID.S_ResChestToInventory; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.serverTick = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.resultCode = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
        count += sizeof(uint);
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
        count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class C_ReqInventoryInfo : IPacket
{
    public int clientTick;

    public ushort Protocol { get { return (ushort)PacketID.C_ReqInventoryInfo; } }

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
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_ReqInventoryInfo);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), clientTick);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_ResInventoryInfo : IPacket
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


    public ushort Protocol { get { return (ushort)PacketID.S_ResInventoryInfo; } }

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
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_ResInventoryInfo);
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