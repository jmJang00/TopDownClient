using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ServerCore;

public enum PacketID
{
    S_CreateMyCharacter = 1,
	S_CreateOtherCharacter = 2,
	S_DeleteCharacter = 3,
	C_MoveStart = 4,
	S_MoveStart = 5,
	S_MoveState = 6,
	S_TickSync = 7,
	C_RotateStart = 8,
	S_RotateStart = 9,
	S_RotateState = 10,
	C_ProjectileShootStart = 11,
	S_ProjectileShootStart = 12,
	S_SpawnProjectile = 13,
	S_DespawnProjectile = 14,
	S_ProjectileHit = 15,
	C_MatchStart = 16,
	C_MatchCancel = 17,
	C_GameEnd = 18,
	S_GameStartWait = 19,
	S_GameStart = 20,
	S_GameEnd = 21,
	C_SceneReady = 22,
	C_WeaponSelect = 23,
	
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
	public byte dir;
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
		this.dir = (byte)segment.Array[segment.Offset + count];
		count += sizeof(byte);
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
		segment.Array[segment.Offset + count] = (byte)this.dir;
		count += sizeof(byte);
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
	public byte dir;
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
		this.dir = (byte)segment.Array[segment.Offset + count];
		count += sizeof(byte);
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
		segment.Array[segment.Offset + count] = (byte)this.dir;
		count += sizeof(byte);
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
	public byte dir;
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
		this.dir = (byte)segment.Array[segment.Offset + count];
		count += sizeof(byte);
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
		segment.Array[segment.Offset + count] = (byte)this.dir;
		count += sizeof(byte);
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
public class C_GameEnd : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.C_GameEnd; } }

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
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_GameEnd);
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_GameStartWait : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.S_GameStartWait; } }

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
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_GameStartWait);
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_GameStart : IPacket
{
    public int tick;
	public ushort weaponId;

    public ushort Protocol { get { return (ushort)PacketID.S_GameStart; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
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
    

    public ushort Protocol { get { return (ushort)PacketID.S_GameEnd; } }

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
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_GameEnd);
        count += sizeof(ushort);
        
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

