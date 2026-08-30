using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public enum NetworkState 
{
    None,
    ConnectRequested,
    Connected,
    Disconnected,
    Authorized,
    GameFound,
    GameReady,
    GameStarted,
}

public class NetworkManager : MonoBehaviour
{
    private ServerSession _gameSession = null;
    private ServerSession _chatSession = null;

    public ServerSession GameSession {  get { return _gameSession; } }
    public ServerSession ChatSession {  get { return _chatSession; } }

    private static NetworkManager s_instance;
    public static NetworkManager Instance { get { Init(); return s_instance; } }

    public GameScene game;

    public TickScheduler tickScheduler { get { return game.tickScheduler; } }
    public EntitySystem entitySystem { get { return game.entitySystem; } }
    public SpawnManager spawnManager { get { return game.spawnManager; } }

    public NetworkState State { get { return _gameSession.State; } }

    public bool autoConnect = true;
    public string ipStr = "127.0.0.1";
    public short portNum = 6000;
    public string sessionKeyStr = "HGUEynFxicSxGWfQuwnOvRkPEgxPryTWYYCPvKFMnMkswrnkCftsysaPoFzCeUPa";

    public Dictionary<ushort, Queue<PendingRequest>> pendingRequests = new();
    private CancellationTokenSource _shutdownCts = new();

    public class PendingRequest
    {
        public TaskCompletionSource<IPacket> Source = new();
        public CancellationTokenRegistration Registration;

        public void TryCancel(CancellationToken token)
        {
            Source.TrySetCanceled(token);
            Registration.Dispose();
        }

        public bool TryComplete(IPacket packet)
        {
            Registration.Dispose();
            return Source.TrySetResult(packet);
        }
    }

    private void OnEnable()
    {
        NetworkEventBus.Subscribe(PacketID.S_MatchFound, OnGameFound);
    }

    private void OnDisable()
    {
        NetworkEventBus.Unsubscribe(PacketID.S_MatchFound, OnGameFound);
    }

    public bool OnResponse(IPacket packet)
    {
        if (pendingRequests.TryGetValue(packet.Protocol, out var queue))
        {
            while (queue.Count > 0)
            {
                var pending = queue.Dequeue();
                if (pending.TryComplete(packet))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void GameSend(ArraySegment<byte> sendBuff)
    {
        _gameSession.Send(sendBuff);
    }

    public void ChatSend(ArraySegment<byte> sendBuff)
    {
        _chatSession.Send(sendBuff);
    }

    public async Task<T> GameSendRequest<T>(IPacket packet) where T : IPacket
    {
        try
        {
            return await _gameSession.SendRequest<T>(packet, _shutdownCts.Token);
        }
        catch
        {
            Debug.Log("Shutdown GameSendRequest");
            return default(T);
        }
    }

    public async Task<T> ChatSendRequest<T>(IPacket packet) where T : IPacket
    {
        try
        {
            return await _chatSession.SendRequest<T>(packet, _shutdownCts.Token);
        }
        catch
        {
            Debug.Log("Shutdown ChatSendRequest");
            return default(T);
        }
    }

    public static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("NetworkManager");
            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<NetworkManager>();
            s_instance._gameSession = new ServerSession();
            s_instance._chatSession = new ServerSession();
            if (s_instance.autoConnect)
            {
                s_instance.ConnectToGame(s_instance.sessionKeyStr, s_instance.ipStr, s_instance.portNum);
            }
        }
    }
    
    public async void ConnectToGame(string sessionKey, string ip, int port)
    {
        _chatSession.ConnectHandler = () =>
        {
            C_ReqLoginChatServer login = new C_ReqLoginChatServer();
            login.sessionKey = sessionKey;
            _chatSession.Send(login.Write());
        };

        _chatSession.DisconnectHandler = () =>
        {
            Debug.Log("채팅서버와의 연결이 끊어졌습니다");
        };

        _gameSession.ConnectHandler = () =>
        {
            C_ReqLoginGameServer login = new C_ReqLoginGameServer();
            login.sessionKey = sessionKey;
            _gameSession.Send(login.Write());
        };

        _gameSession.DisconnectHandler = () =>
        {
            ApplicationUtil.ShowErrorAndQuit("게임서버와의 연결이 끊어졌습니다.", "네트워크 오류");
        };
        try
        {
            Task task1 = _chatSession.TryConnectAndAuthorize(sessionKey, ip, port, _shutdownCts.Token);
            Task task2 = _gameSession.TryConnectAndAuthorize(sessionKey, ip, port, _shutdownCts.Token);
            await Task.WhenAll(task1, task2);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Network request canceled.");
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public void StartFindGame()
    {
        StartCoroutine(CoFindGame());
    }

    // 매칭을 요청하고 매칭을 찾으면 게임씬으로 이동하는 함수
    // 코루틴이 네트워크 매니저의 소유이고 네트워크 매니저는 씬 이동 중에 삭제되지 않으므로 
    // 씬 전환에 있어서 안전하다
    public IEnumerator CoFindGame()
    {
        NetworkState? state = GameSession?.State;
        if (!state.HasValue || state != NetworkState.Authorized)
            yield break;

        C_MatchStart matchStart = new C_MatchStart();
        GameSend(matchStart.Write());
    }

    public void QuitGame()
    {
        C_QuitGame quitGame = new C_QuitGame();
        GameSend(quitGame.Write());
        game.gameSelectUI.HideAll();
    }

    // 클라이언트에서 서버측에 보내는 로비로 되돌아가고 싶다는 요청
    // 서버에서 게임씬에 들어간 상태에서만 유효
    public void ReturnToLobby()
    {
        C_ReturnToLobby gameEnd = new C_ReturnToLobby();
        GameSend(gameEnd.Write());
        game.gameSelectUI.HideAll();
    }

    // success는 매칭 취소 요청이 전달된 경우를 구분하기 위함
    // 정상적으로 취소 요청이 반영되어서 매칭이 취소되면 false가 전달
    // 이미 서버에서 게임이 시작되어 버린 경우 그냥 게임 시작
    public void OnGameFound(IPacket packet)
    {
        S_MatchFound matchFound = packet as S_MatchFound;
        StartCoroutine(CoOnGameFound(matchFound.success));
    }

    public IEnumerator CoOnGameFound(bool success)
    {
        if (success)
        {
            var config = DevConfig.Load();
            MMSceneLoadingManager.LoadScene(config.StartScene);

            yield return new WaitUntil(() => { return game != null; });

            C_SceneReady sceneReady = new C_SceneReady();
            GameSend(sceneReady.Write());

            game.gameSelectUI.HideAll();
            game.gameSelectUI.ShowWeaponSelect();
            _gameSession.ChangeState(NetworkState.Authorized, NetworkState.GameReady);
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
            _gameSession.ChangeState(NetworkState.GameReady, NetworkState.GameStarted);
        }
        else
        {
            game = null;
            MMSceneLoadingManager.LoadScene("StartScene");
            _gameSession.ChangeState(NetworkState.GameReady, NetworkState.Authorized);
        }
    }

    // 게임이 종료 조건이 만족되었을 때 접속해 있는 플레이어에게 승패의 결과를 보내면
    // 그에 맞게 UI를 띄워주고 UI에서 버튼을 눌렀을 때, 원래 로비씬으로 돌아간다 
    // 타임아웃을 둬서 플레이어가 버튼을 누르지 않으면 서버가 자동으로 로비씬으로 넘어가는 패킷을 전송
    public void OnGameEnd(bool isWinner, IReadOnlyList<PlayerResult> results)
    {
        game.gameSelectUI.HideAll();
        game.gameSelectUI.ShowVictory(isWinner, results);
        game.tickScheduler.Stop();
    }

    // 대부분 클라이언트가 먼저 로비로 이동하겠다고 요청하고 서버에서 처리해주는 형태
    // 플레이어를 로비로 이동시킨 후에 서버가 패킷을 보내 알려주면
    // 그제서야 클라이언트는 플레이어를 로비로 이동시켜야 한다
    public void OnReturnToLobby()
    {
        game.gameSelectUI.HideAll();
        game = null;
        MMSceneLoadingManager.LoadScene("StartScene");
        _gameSession.AssignState(NetworkState.Authorized);
    }

    // 게임씬이 로딩되었을 때 씬에 존재하는 GameScene 컴포넌트의 Start 부분에서 이 함수를 호출
    // 함수가 호출되면 game에 대한 링크를 지정하고 이를 통해 게임 씬이 완전히 로딩되었고
    // 게임씬에 있는 원하는 컴포넌트를 접근할 수 있다는 걸 확인해준다
    public void SetGameScene(GameScene scene)
    {
        game = scene;
    }

    void Start()
    {
        Init();
    }

    void OnApplicationQuit()
    {
        _shutdownCts.Cancel();

        if (_gameSession != null)
        {
            _gameSession.Disconnect();
            _gameSession = null;
        }

        if (_chatSession != null)
        {
            _chatSession.Disconnect();
            _chatSession = null;
        }
    }

    void OnDestroy()
    {
        _shutdownCts.Cancel();

        game?.Clear();

        if (_gameSession != null)
        {
            _gameSession.Disconnect();
            _gameSession = null;
        }

        if (_chatSession != null)
        {
            _chatSession.Disconnect();
            _chatSession = null;
        }
    }

    private void Update()
    {
        List<IPacket> list = PacketQueue.Instance.PopAll();
        foreach (IPacket packet in list)
        {
            if (!OnResponse(packet))
            {
                // Fall back
                PacketManager.Instance.HandlePacket(_gameSession, packet);
            }
        }
        game?.ProcessUpdate();
    }
}
