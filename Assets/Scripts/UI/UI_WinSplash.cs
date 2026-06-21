using MoreMountains.Tools;
using System.Collections;
using UnityEngine;

public class UI_WinSplash : UI_Panel 
{
    public MMTouchButton button;

    public void ReturnToLobby()
    {
        NetworkManager.Instance.ReturnToLobby();
        button.Interactable = false;
    }
}
