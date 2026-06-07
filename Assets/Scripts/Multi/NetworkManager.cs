using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using UnityEngine;

public enum NetworkState 
{
    None,
    ConnectRequested,
    Connected,
    Disconnected,
    Authorized,
    WaitingForMatch,
    GameFound,
    MatchFailed,
    GameReady,
    GameStarted,
}

public class NetworkManager : MonoBehaviour
{
    private ServerSession _session;

    private static NetworkManager s_instance;
    public static NetworkManager Instance { get { Init(); return s_instance; } }

    private static int _state = (int)NetworkState.None;

    public static NetworkState State { get { return (NetworkState)Volatile.Read(ref _state); } }

    private GameScene _game;

    public TickScheduler tickScheduler { get { return _game.tickScheduler; } }
    public EntitySystem entitySystem { get { return _game.entitySystem; } }
    public SpawnManager spawnManager { get { return _game.spawnManager; } }


    public string ipStr = "127.0.0.1";
    public short port = 6000;

    public static bool ChangeState(NetworkState expected, NetworkState desired)
    {
        bool success = Interlocked.CompareExchange(ref _state, (int)desired, (int)expected) == (int)expected;
        if (!success)
        {
            Debug.LogWarning("Expected: " + expected.ToString() + " Desired: " + desired.ToString());
        }

        return success;
    }

    public void Send(ArraySegment<byte> sendBuff)
    {
        _session.Send(sendBuff);
    }

    public static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("NetworkManager");
            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<NetworkManager>();
            s_instance._session = new ServerSession();
            s_instance.StartCoroutine(s_instance.TryConnectAndAuthorize());
        }
    }
    
    public IEnumerator TryConnectAndAuthorize()
    {
        while (true)
        {
            if (ChangeState(NetworkState.None, NetworkState.ConnectRequested))
            {
                ConnectServer();
                Debug.Log("Try Connect");
            }

            if (State != NetworkState.None && State != NetworkState.ConnectRequested)
            {
                break;
            }

            yield return new WaitForSeconds(3);
        }

        if (State == NetworkState.Connected)
        {
            Debug.Log("Connect Success");

            //TODO: 나중에 인증과정 추가

            ChangeState(NetworkState.Connected, NetworkState.Authorized);
        }
    }

    public IEnumerator FindGame()
    {
        if (State != NetworkState.Authorized)
            yield break;

        C_MatchStart matchStart = new C_MatchStart();
        Send(matchStart.Write());

        ChangeState(NetworkState.Authorized, NetworkState.WaitingForMatch);

        yield return new WaitUntil(() => { return State != NetworkState.WaitingForMatch; });

        if (State == NetworkState.GameFound)
        {
            MMSceneLoadingManager.LoadScene("ServerSyncTest");

            yield return new WaitUntil(() => { return _game != null; });

            C_SceneReady sceneReady = new C_SceneReady();
            Send(sceneReady.Write());

            ChangeState(NetworkState.GameFound, NetworkState.GameReady);
        }
    }

    public void EndGame()
    {
        C_GameEnd gameEnd = new C_GameEnd();
        Send(gameEnd.Write());
    }

    public void OnGameFound()
    {
        ChangeState(NetworkState.WaitingForMatch, NetworkState.GameFound);
    }

    public void OnGameStart()
    {
        ChangeState(NetworkState.GameReady, NetworkState.GameStarted);
    }

    public void OnGameEnd()
    {
        _game = null;
        MMSceneLoadingManager.LoadScene("StartScene");
        ChangeState(NetworkState.GameStarted, NetworkState.Authorized);
    }

    public void SetGameScene(GameScene scene)
    {
        _game = scene;
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

    void OnApplicationQuit()
    {
        if (_session != null)
        {
            _session.Disconnect();
            _session = null;
        }
    }

    void OnDestroy()
    {
        _game?.Clear();

        if (_session != null)
        {
            _session.Disconnect();
            _session = null;
        }
    }

    private void Update()
    {
        List<IPacket> list = PacketQueue.Instance.PopAll();
        foreach (IPacket packet in list)
            PacketManager.Instance.HandlePacket(_session, packet);

        _game?.ProcessUpdate();
    }
}
