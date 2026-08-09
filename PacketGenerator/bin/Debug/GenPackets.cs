using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ServerCore;

public enum PacketID
{
    C_RegLoginGameServer = 1,
	S_ResLoginGameServer = 2,
	C_SendFriendReq = 200,
	S_SendFriendReqResult = 201,
	S_FriendReqFromOther = 202,
	C_FriendReqRespons = 203,
	S_FriendReqResFromTarget = 204,
	C_RemoveFriend = 205,
	S_RemoveFriendResult = 206,
	C_FriendList = 207,
	S_FriendList = 208,
	C_FriendReqList = 209,
	S_FriendReqList = 210,
	
}

public interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}



public class C_SendFriendReq : IPacket
{
    public string friendname;

    public ushort Protocol { get { return (ushort)PacketID.C_SendFriendReq; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        ushort friendnameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.friendname = Encoding.Unicode.GetString(s.Slice(count, friendnameLen));
		count += friendnameLen;
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_SendFriendReq);
        count += sizeof(ushort);
        ushort friendnameLen = (ushort)Encoding.Unicode.GetBytes(this.friendname, 0, this.friendname.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), friendnameLen);
		count += sizeof(ushort);
		count += friendnameLen;
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_SendFriendReqResult : IPacket
{
    public int result;

    public ushort Protocol { get { return (ushort)PacketID.S_SendFriendReqResult; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.result = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_SendFriendReqResult);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), result);
		count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_FriendReqFromOther : IPacket
{
    public string friendname;

    public ushort Protocol { get { return (ushort)PacketID.S_FriendReqFromOther; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        ushort friendnameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.friendname = Encoding.Unicode.GetString(s.Slice(count, friendnameLen));
		count += friendnameLen;
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_FriendReqFromOther);
        count += sizeof(ushort);
        ushort friendnameLen = (ushort)Encoding.Unicode.GetBytes(this.friendname, 0, this.friendname.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), friendnameLen);
		count += sizeof(ushort);
		count += friendnameLen;
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class C_FriendReqRespons : IPacket
{
    public int result;

    public ushort Protocol { get { return (ushort)PacketID.C_FriendReqRespons; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.result = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_FriendReqRespons);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), result);
		count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_FriendReqResFromTarget : IPacket
{
    public int result;

    public ushort Protocol { get { return (ushort)PacketID.S_FriendReqResFromTarget; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.result = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_FriendReqResFromTarget);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), result);
		count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class C_RemoveFriend : IPacket
{
    public string friendname;

    public ushort Protocol { get { return (ushort)PacketID.C_RemoveFriend; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        ushort friendnameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.friendname = Encoding.Unicode.GetString(s.Slice(count, friendnameLen));
		count += friendnameLen;
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_RemoveFriend);
        count += sizeof(ushort);
        ushort friendnameLen = (ushort)Encoding.Unicode.GetBytes(this.friendname, 0, this.friendname.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), friendnameLen);
		count += sizeof(ushort);
		count += friendnameLen;
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_RemoveFriendResult : IPacket
{
    public int result;

    public ushort Protocol { get { return (ushort)PacketID.S_RemoveFriendResult; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.result = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_RemoveFriendResult);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), result);
		count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class C_FriendList : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.C_FriendList; } }

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
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_FriendList);
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_FriendList : IPacket
{
    public int friendCount;
	

    public ushort Protocol { get { return (ushort)PacketID.S_FriendList; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.friendCount = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_FriendList);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), friendCount);
		count += sizeof(int);
		
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class C_FriendReqList : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.C_FriendReqList; } }

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
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_FriendReqList);
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S_FriendReqList : IPacket
{
    public int requestCount;
	

    public ushort Protocol { get { return (ushort)PacketID.S_FriendReqList; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.requestCount = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_FriendReqList);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), requestCount);
		count += sizeof(int);
		
        success &= BitConverter.TryWriteBytes(s, (ushort)(count - 2));
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}

