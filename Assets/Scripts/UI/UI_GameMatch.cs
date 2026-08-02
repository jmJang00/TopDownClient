using MoreMountains.Tools;
using UnityEngine;

public class UI_GameMatch : MonoBehaviour
{
    public MMTouchButton startButton;
    public MMTouchButton cancelButton;

    public void OnEnable()
    {
        UIEventBus.Subscribe((ushort)PacketID.S_MatchFound, TurnOn);
        UIEventBus.Subscribe((ushort)PacketID.S_ResLoginGameServer, TurnOn);
    }

    public void OnDisable()
    {
        UIEventBus.Unsubscribe((ushort)PacketID.S_MatchFound, TurnOn);
        UIEventBus.Unsubscribe((ushort)PacketID.S_ResLoginGameServer, TurnOn);
    }

    public void Start()
    {
        // 다시 처음씬이 로드될 때 실행되지 않도록
        if (NetworkManager.State == NetworkState.None)
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
        NetworkManager.Instance.Send(matchCancel.Write());
        startButton.EnableButton();
        cancelButton.DisableButton();
    }

    public void TurnOn(IPacket packet)
    {
        startButton.EnableButton();
        cancelButton.DisableButton();
    }
}
