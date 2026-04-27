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
	
}

public interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}

public class S_CreateMyCharacter : IPacket
{
    public uint accountId;
	public byte dir;
	public ushort x;
	public ushort y;
	public byte hp;

    public ushort Protocol { get { return (ushort)PacketID.S_CreateMyCharacter; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.accountId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
		count += sizeof(uint);
		this.dir = (byte)segment.Array[segment.Offset + count];
		count += sizeof(byte);
		this.x = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.y = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.hp = (byte)segment.Array[segment.Offset + count];
		count += sizeof(byte);
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
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), accountId);
		count += sizeof(uint);
		segment.Array[segment.Offset + count] = (byte)this.dir;
		count += sizeof(byte);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), x);
		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), y);
		count += sizeof(ushort);
		segment.Array[segment.Offset + count] = (byte)this.hp;
		count += sizeof(byte);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_CreateOtherCharacter : IPacket
{
    public uint accountId;
	public byte dir;
	public ushort x;
	public ushort y;
	public byte hp;

    public ushort Protocol { get { return (ushort)PacketID.S_CreateOtherCharacter; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.accountId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
		count += sizeof(uint);
		this.dir = (byte)segment.Array[segment.Offset + count];
		count += sizeof(byte);
		this.x = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.y = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.hp = (byte)segment.Array[segment.Offset + count];
		count += sizeof(byte);
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
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), accountId);
		count += sizeof(uint);
		segment.Array[segment.Offset + count] = (byte)this.dir;
		count += sizeof(byte);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), x);
		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), y);
		count += sizeof(ushort);
		segment.Array[segment.Offset + count] = (byte)this.hp;
		count += sizeof(byte);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_DeleteCharacter : IPacket
{
    public uint accountId;

    public ushort Protocol { get { return (ushort)PacketID.S_DeleteCharacter; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
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
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_DeleteCharacter);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), accountId);
		count += sizeof(uint);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class C_MoveStart : IPacket
{
    public uint accountId;
	public byte dir;
	public ushort x;
	public ushort y;

    public ushort Protocol { get { return (ushort)PacketID.C_MoveStart; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.accountId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
		count += sizeof(uint);
		this.dir = (byte)segment.Array[segment.Offset + count];
		count += sizeof(byte);
		this.x = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.y = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
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
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), accountId);
		count += sizeof(uint);
		segment.Array[segment.Offset + count] = (byte)this.dir;
		count += sizeof(byte);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), x);
		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), y);
		count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_MoveStart : IPacket
{
    public uint accountId;
	public byte dir;
	public ushort x;
	public ushort y;

    public ushort Protocol { get { return (ushort)PacketID.S_MoveStart; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.accountId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
		count += sizeof(uint);
		this.dir = (byte)segment.Array[segment.Offset + count];
		count += sizeof(byte);
		this.x = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.y = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
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
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), accountId);
		count += sizeof(uint);
		segment.Array[segment.Offset + count] = (byte)this.dir;
		count += sizeof(byte);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), x);
		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), y);
		count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}

