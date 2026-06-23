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

    public GameScene game;

    public TickScheduler tickScheduler { get { return game.tickScheduler; } }
    public EntitySystem entitySystem { get { return game.entitySystem; } }
    public SpawnManager spawnManager { get { return game.spawnManager; } }


    public long accountId;
    public string ipStr = "127.0.0.1";
    public short port = 6000;

    // 이전 상태와 관련없이 무조건 강제되는 상태인 경우 사용
    public static void AssignState(NetworkState desired)
    {
        Interlocked.Exchange(ref _state, (int)desired);
    }

    // 이전 상태의 연속성이 확보되어야 하는 경우 사용
    public static bool ChangeState(NetworkState expected, NetworkState desired)
    {
        int value = Interlocked.CompareExchange(ref _state, (int)desired, (int)expected);
        bool success = value == (int)expected;
        if (!success)
        {
            Debug.LogWarning("Current: " + ((NetworkState)value).ToString() + " Expected: " + expected.ToString() + " Desired: " + desired.ToString());
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
    
    // 처음 연결을 시작할 때는 연결이 붙을 때까지 계속 재시도
    // 연결이 붙고나서는 한 번 연결이 끊어지면 게임을 종료
    public IEnumerator TryConnectAndAuthorize()
    {
        while (true)
        {
            while (State != NetworkState.Disconnected && State != NetworkState.None)
            {
                yield return new WaitForSeconds(1);
            }

            if (State == NetworkState.Disconnected)
            {
                //game = null;
                //MMSceneLoadingManager.LoadScene("StartScene");
                //ChangeState(NetworkState.Disconnected, NetworkState.None);

                // 원래는 로비씬으로 이동하게 뒀지만, 서버가 끊은 걸 수도 있기 때문에 알기 쉽게 종료창을 띄우도록 수정
                ApplicationUtil.ShowErrorAndQuit("서버와의 연결이 끊어졌습니다.", "네트워크 오류");
                yield break;
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
                    ConnectServer();
                    Debug.Log("Try Connect");
                }

                yield return new WaitForSeconds(1);
            }

            if (State == NetworkState.Connected)
            {
                Debug.Log("Connect Success");

                C_AccountInfoDebug accountInfoDebug = new C_AccountInfoDebug();
                accountInfoDebug.accountId = accountId;
                Send(accountInfoDebug.Write());
                UIEventBus.Publish(accountInfoDebug);

                ChangeState(NetworkState.Connected, NetworkState.Authorized);
                Debug.Log("Player Authorized");
            }
        }
    }

    public void StartFindGame()
    {
        StartCoroutine(FindGame());
    }

    // 매칭을 요청하고 매칭을 찾으면 게임씬으로 이동하는 함수
    // 코루틴이 네트워크 매니저의 소유이고 네트워크 매니저는 씬 이동 중에 삭제되지 않으므로 
    // 씬 전환에 있어서 안전하다
    public IEnumerator FindGame()
    {
        if (State != NetworkState.Authorized)
            yield break;

        C_MatchStart matchStart = new C_MatchStart();
        Send(matchStart.Write());

        if (!ChangeState(NetworkState.Authorized, NetworkState.WaitingForMatch))
        {
            yield break;
        }

        yield return new WaitUntil(() => { return State != NetworkState.WaitingForMatch; });

        if (State == NetworkState.GameFound)
        {
            MMSceneLoadingManager.LoadScene("ServerSyncTest");

            yield return new WaitUntil(() => { return game != null; });

            C_SceneReady sceneReady = new C_SceneReady();
            Send(sceneReady.Write());

            ChangeState(NetworkState.GameFound, NetworkState.GameReady);

            game.gameSelectUI.HideAll();
            game.gameSelectUI.ShowWeaponSelect();
        }
    }

    // 클라이언트에서 서버측에 보내는 로비로 되돌아가고 싶다는 요청
    // 서버에서 게임씬에 들어간 상태에서만 유효
    public void ReturnToLobby()
    {
        C_ReturnToLobby gameEnd = new C_ReturnToLobby();
        Send(gameEnd.Write());
    }

    // success는 매칭 취소 요청이 전달된 경우를 구분하기 위함
    // 정상적으로 취소 요청이 반영되어서 매칭이 취소되면 false가 전달
    // 이미 서버에서 게임이 시작되어 버린 경우 그냥 게임 시작
    public void OnGameFound(bool success)
    {
        if (success)
        {
            ChangeState(NetworkState.WaitingForMatch, NetworkState.GameFound);
        }
        else
        {
            ChangeState(NetworkState.WaitingForMatch, NetworkState.Authorized);
        }
    }

    // success는 게임 도중 접속에 대한 예외를 처리하기 위한 플래그
    // 도중 접속해서 이전 게임에 다시 들어갔는데 게임이 이미 종료 중인 상태인 경우
    // 내가 씬레디 패킷까지 보낸 경우라서 
    // GameReady -> GameStarted 로 이어지는 로직을 타지 않고 원래 로비씬으로 돌아감
    public void OnGameStart(bool success)
    {
        game.gameSelectUI.HideAll();
        if (success)
        {
            ChangeState(NetworkState.GameReady, NetworkState.GameStarted);
        }
        else
        {
            game = null;
            MMSceneLoadingManager.LoadScene("StartScene");
            ChangeState(NetworkState.GameReady, NetworkState.Authorized);
        }
    }

    // 게임이 종료 조건이 만족되었을 때 접속해 있는 플레이어에게 승패의 결과를 보내면
    // 그에 맞게 UI를 띄워주고 UI에서 버튼을 눌렀을 때, 원래 로비씬으로 돌아간다 
    // 타임아웃을 둬서 플레이어가 버튼을 누르지 않으면 서버가 자동으로 로비씬으로 넘어가는 패킷을 전송
    public void OnGameEnd(bool isWinner)
    {
        game.gameSelectUI.HideAll();
        if (isWinner)
        {
            game.gameSelectUI.ShowVictory();
        }
        else
        {
            game.gameSelectUI.ShowDefeat();
        }
    }

    // 대부분 클라이언트가 먼저 로비로 이동하겠다고 요청하고 서버에서 처리해주는 형태
    // 플레이어를 로비로 이동시킨 후에 서버가 패킷을 보내 알려주면
    // 그제서야 클라이언트는 플레이어를 로비로 이동시켜야 한다
    public void OnReturnToLobby()
    {
        game = null;
        MMSceneLoadingManager.LoadScene("StartScene");
        AssignState(NetworkState.Authorized);
    }

    // 게임씬이 로딩되었을 때 씬에 존재하는 GameScene 컴포넌트의 Start 부분에서 이 함수를 호출
    // 함수가 호출되면 game에 대한 링크를 지정하고 이를 통해 게임 씬이 완전히 로딩되었고
    // 게임씬에 있는 원하는 컴포넌트를 접근할 수 있다는 걸 확인해준다
    public void SetGameScene(GameScene scene)
    {
        game = scene;
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
        game?.Clear();

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
        game?.ProcessUpdate();
    }
}
