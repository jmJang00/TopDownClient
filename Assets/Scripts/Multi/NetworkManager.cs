using MoreMountains.TopDownEngine;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    ServerSession _session = new ServerSession();

    public Transform[] spawnPositions;

    public TickScheduler tickScheduler;
    public EntitySystem entitySystem;

    public string ipStr = "127.0.0.1";
    public short port = 6000;

    public void Send(ArraySegment<byte> sendBuff)
    {
        _session.Send(sendBuff);
    }

    void Start()
    {
        // string host = Dns.GetHostName();
        // IPHostEntry ipHost = Dns.GetHostEntry(host);
        // IPAddress ipAddr = ipHost.AddressList[0];

        tickScheduler = GetComponent<TickScheduler>();

        IPAddress ipAddr = IPAddress.Parse(ipStr);
        IPEndPoint endPoint = new IPEndPoint(ipAddr, port);

        Connector connector = new Connector();

        connector.Connect(endPoint, () => { return _session; }, 1);

        Spawn(EntityType.Player, spawnPositions[0].position);
    }

    void Update()
    {
        List<IPacket> list = PacketQueue.Instance.PopAll();
        foreach (IPacket packet in list)
            PacketManager.Instance.HandlePacket(_session, packet);
    }

    public void Spawn(EntityType type, Vector3 position)
    {
        if (type == EntityType.Player)
        { 
            GameObject prefab = Resources.Load<GameObject>("Prefabs/LoftSuit");
            if (prefab == null)
            {
                Debug.Log("Can't find " + type.ToString());
                return;
            }

            GameObject obj = Instantiate(prefab);
            position.y += 5;
            MyPlayer myPlayer = obj.GetComponent<MyPlayer>();
            obj.transform.position = position;
            myPlayer.OnSpawn();
        }
    }
}
