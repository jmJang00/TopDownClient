using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Purchasing;

public class ServerSession : PacketSession
{
    private int _state = (int)NetworkState.None;
    public NetworkState State { get { return (NetworkState)Volatile.Read(ref _state); } }

    public Action ConnectHandler;
    public Action DisconnectHandler;

    public void AssignState(NetworkState desired)
    {
        Interlocked.Exchange(ref _state, (int)desired);
    }

    public bool ChangeState(NetworkState expected, NetworkState desired)
    {
        int value = Interlocked.CompareExchange(ref _state, (int)desired, (int)expected);
        return value == (int)expected;
    }
    
    public async Task TryConnectAndAuthorize(string sessionKey, string ip, int port, CancellationToken token)
    {
        while (true)
        {
            while (State != NetworkState.Disconnected && State != NetworkState.None)
            {
                await Task.Delay(500, token);
            }

            if (State == NetworkState.Disconnected)
            {
                DisconnectHandler.Invoke();
                return;
            }

            while (true)
            {
                NetworkState state = State;
                if (state != NetworkState.None && state != NetworkState.ConnectRequested)
                {
                    break;
                }

                if (state == NetworkState.None && ChangeState(NetworkState.None, NetworkState.ConnectRequested))
                {
                    IPAddress ipAddr = IPAddress.Parse(ip);
                    IPEndPoint endPoint = new IPEndPoint(ipAddr, port);
                    Connector connector = new Connector();
                    connector.Connect(endPoint, () => { return this; }, 1);
                }

                await Task.Delay(500, token);
            }

            if (State == NetworkState.Connected)
            {
                ConnectHandler.Invoke();
            }
        }
    }

    public async Task<T> SendRequest<T>(IPacket packet, CancellationToken token = default) where T : IPacket
    {
        PacketID protocol = Enum.Parse<PacketID>(typeof(T).Name);
        IPacket result = await SendRequest(protocol, packet, token);
        return (T)result;
    }

    public Task<IPacket> SendRequest(PacketID protocol, IPacket packet, CancellationToken token)
    {
        var pending = new NetworkManager.PendingRequest();

        if (token.CanBeCanceled)
        {
            pending.Registration = token.Register(() =>
            {
                pending.TryCancel(token);
            });
        }

        NetworkManager mng = NetworkManager.Instance;
        if (!mng.pendingRequests.TryGetValue((ushort)protocol, out var queue))
        {
            queue = new Queue<NetworkManager.PendingRequest>();
            mng.pendingRequests.Add((ushort)protocol, queue);
        }

        queue.Enqueue(pending);
        Send(packet.Write());

        return pending.Source.Task;
    }

    public override void OnConnected(EndPoint endPoint)
    {
        Console.WriteLine($"OnConnected: {endPoint}");
        AssignState(NetworkState.Connected);
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        Console.WriteLine($"OnDisconnected: {endPoint}");
        AssignState(NetworkState.Disconnected);
    }

    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        PacketManager.Instance.OnRecvPacket(this, buffer, (s, p) => PacketQueue.Instance.Push(p));
    }

    public override void OnSend(int numOfBytes)
    {
        // Console.WriteLine($"Transffered bytes: {numOfBytes}");
    }
}
