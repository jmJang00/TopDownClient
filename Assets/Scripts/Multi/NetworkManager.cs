using MoreMountains.TopDownEngine;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    ServerSession _session;

    static NetworkManager s_instance;
    public static NetworkManager Instance { get { Init(); return s_instance; } }

    public TickScheduler tickScheduler;
    public EntitySystem entitySystem;

    public string ipStr = "127.0.0.1";
    public short port = 6000;

    public void Send(ArraySegment<byte> sendBuff)
    {
        _session.Send(sendBuff);
    }

    public static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("GameManager");
            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<NetworkManager>();
            s_instance.tickScheduler = s_instance.GetComponent<TickScheduler>();
            s_instance.entitySystem = s_instance.GetComponent<EntitySystem>();
            s_instance._session = new ServerSession();
            s_instance.ConnectServer();
        }
    }

    private void ConnectServer()
    {
        // string host = Dns.GetHostName();
        // IPHostEntry ipHost = Dns.GetHostEntry(host);
        // IPAddress ipAddr = ipHost.AddressList[0];

        IPAddress ipAddr = IPAddress.Parse(ipStr);
        IPEndPoint endPoint = new IPEndPoint(ipAddr, port);

        Connector connector = new Connector();

        connector.Connect(endPoint, () => { return _session; }, 1);
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        List<IPacket> list = PacketQueue.Instance.PopAll();
        foreach (IPacket packet in list)
            PacketManager.Instance.HandlePacket(_session, packet);

        tickScheduler.Simulate();

        entitySystem.RunRender(tickScheduler.Alpha);
    }

    public void SpawnAt(int tick, EntityType type, int id, Vector3 position)
    {
        NetEntity entity = Spawn(type, id, position);
        entity.gameObject.SetActive(false);
        tickScheduler.ScheduleAt(tick, () =>
        {
            entity.gameObject.SetActive(true);
            entity.OnSpawn(tick);
        });
    }

    public void DespawnAt(int tick, EntityType type, int id)
    {
        tickScheduler.ScheduleAt(tick, () =>
        {
            NetEntity entity = entitySystem.Get(id);
            entity.OnDespawn();
            Despawn(type, id);
        });
    }

    public NetEntity Spawn(EntityType type, int id, Vector3 position)
    {
        switch (type)
        {
            case EntityType.MyPlayer:
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/Player");
                if (prefab == null)
                {
                    Debug.Log("Can't find " + type.ToString());
                    return null;
                }

                GameObject obj = Instantiate(prefab);
                MyPlayer myPlayer = obj.GetComponent<MyPlayer>();
                myPlayer.entityId = id;
                myPlayer.type = EntityType.MyPlayer;
                myPlayer.transform.position = position;
                myPlayer.Init();
                entitySystem.Register(id, myPlayer);
                return myPlayer;
            }
            case EntityType.OtherPlayer:
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/OtherPlayer");
                if (prefab == null)
                {
                    Debug.Log("Can't find " + type.ToString());
                    return null;
                }

                GameObject obj = Instantiate(prefab);
                Player player = obj.GetComponent<Player>();
                player.entityId = id;
                player.transform.position = position;
                player.type = EntityType.OtherPlayer;
                player.Init();
                entitySystem.Register(id, player);
                return player;
            }
            default:
            {
                return null;
            }
        }
    }

    public void Despawn(EntityType type, int id)
    {
        switch (type)
        {
            case EntityType.MyPlayer:
            {
                NetEntity entity = entitySystem.Get(id);
                if (entity == null)
                {
                    Debug.Log("Can't find " + type.ToString());
                    return;
                }

                if (entity.type != EntityType.MyPlayer)
                {
                    Debug.Log("Incorrect entityType " + type.ToString());
                    return;
                }

                entitySystem.Remove(id);
                GameObject.Destroy(entity.gameObject);
                break;
            }
            case EntityType.OtherPlayer:
            {
                NetEntity entity = entitySystem.Get(id);
                if (entity == null)
                {
                    Debug.Log("Can't find " + type.ToString());
                    return;
                }

                if (entity.type != EntityType.OtherPlayer)
                {
                    Debug.Log("Incorrect entityType " + type.ToString());
                    return;
                }

                entitySystem.Remove(id);
                GameObject.Destroy(entity.gameObject);
                break;
            }
        }
    }
}
