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

    public void Awake()
    {
        button.Interactable = false;
    }

    public void MatchStart()
    {
        NetworkManager.Instance.StartFindGame();
        button.Interactable = false;
    }

    public void TurnOn(IPacket packet)
    {
        button.Interactable = true;
    }
}
