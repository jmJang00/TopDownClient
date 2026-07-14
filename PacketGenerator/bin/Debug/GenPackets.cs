using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ServerCore;

public enum PacketID
{
    C_RegLoginGameServer = 1,
	S_ResLoginGameServer = 2,
	
}

public interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}

public class C_RegLoginGameServer : IPacket
{
    public string sessionKey;

    public ushort Protocol { get { return (ushort)PacketID.C_RegLoginGameServer; } }

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
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_RegLoginGameServer);
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

