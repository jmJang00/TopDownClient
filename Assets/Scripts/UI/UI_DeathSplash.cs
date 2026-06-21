using MoreMountains.Tools;
using UnityEngine;

public class UI_DeathSplash : UI_Panel 
{
    public MMTouchButton button;

    public void ReturnToLobby()
    {
        NetworkManager.Instance.ReturnToLobby();
        button.Interactable = false;
    }
}
