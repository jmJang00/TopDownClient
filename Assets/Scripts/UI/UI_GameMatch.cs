using MoreMountains.Tools;
using UnityEngine;

public class UI_GameMatch : MonoBehaviour
{
    public MMTouchButton startButton;
    public MMTouchButton cancelButton;

    public void OnEnable()
    {
        NetworkEventBus.Subscribe(PacketID.S_MatchFound, TurnOn);
        NetworkEventBus.Subscribe(PacketID.S_ResLoginGameServer, TurnOn);
    }

    public void OnDisable()
    {
        NetworkEventBus.Unsubscribe(PacketID.S_MatchFound, TurnOn);
        NetworkEventBus.Unsubscribe(PacketID.S_ResLoginGameServer, TurnOn);
    }

    public void Start()
    {
        // 다시 처음씬이 로드될 때 실행되지 않도록
        NetworkState? state = NetworkManager.Instance.GameSession?.State;
        if (!state.HasValue || state == NetworkState.None)
        {
            startButton.DisableButton();
        }

        cancelButton.DisableButton();
        startButton.ButtonReleased.AddListener(MatchStart);
        cancelButton.ButtonReleased.AddListener(MatchCancel);
    }

    public void MatchStart()
    {
        NetworkManager.Instance.StartFindGame();
        startButton.DisableButton();
        cancelButton.EnableButton();
    }

    public void MatchCancel()
    {
        C_MatchCancel matchCancel = new C_MatchCancel();
        NetworkManager.Instance.GameSend(matchCancel.Write());
        startButton.EnableButton();
        cancelButton.DisableButton();
    }

    public void TurnOn(IPacket packet)
    {
        startButton.EnableButton();
        cancelButton.DisableButton();
    }
}
