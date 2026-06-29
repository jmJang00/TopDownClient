using MoreMountains.Tools;
using UnityEngine;

public class GameMatch : MonoBehaviour
{
    public MMTouchButton button;

    public void OnEnable()
    {
        UIEventBus.Subscribe((ushort)PacketID.S_MatchFound, TurnOn);
        UIEventBus.Subscribe((ushort)PacketID.C_AccountInfoDebug, TurnOn);
    }

    public void OnDisable()
    {
        UIEventBus.Unsubscribe((ushort)PacketID.S_MatchFound, TurnOn);
        UIEventBus.Unsubscribe((ushort)PacketID.C_AccountInfoDebug, TurnOn);
    }

    public void Start()
    {
        // 다시 처음씬이 로드될 때 실행되지 않도록
        if (NetworkManager.State == NetworkState.None)
        {
            button.DisableButton();
        }
    }

    public void MatchStart()
    {
        NetworkManager.Instance.StartFindGame();
        button.DisableButton();
    }

    public void TurnOn(IPacket packet)
    {
        button.EnableButton();
    }
}
