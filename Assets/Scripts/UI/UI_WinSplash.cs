using MoreMountains.Tools;
using System.Collections.Generic;
using TMPro;

public class UI_WinSplash : UI_Panel , IInputModeChangeable
{
    public MMTouchButton button;
    public TMP_Text winText;
    public UI_GameResult gameResult;

    public InputModeChangeableInfo GetInputModeInfo()
    {
        InputModeChangeableInfo info;
        info.Name = "UI";
        info.Description = "WinSplash UI";
        return info;
    }

    public void OnDestroy()
    {
        if (IsVisible)
        {
            RequestHide();
        }
    }

    public void SetGameEndInfo(bool isWinner, IReadOnlyList<PlayerResult> results)
    {
        if (isWinner)
        {
            winText.text = "You Win!";
        }
        else
        {
            winText.text = "You Lose!";
        }

        gameResult.SetResults(results);
    }

    public override bool RequestShow()
    {
        if (InputModeManager.Instance.CurrentMode != InputMode.Game)
        {
            return false;
        }

        if (!InputModeManager.Instance.Enter(InputMode.UI, this))
        {
            return false;
        }

        ShowInternal();
        return true;
    }

    public override bool RequestHide()
    {
        if (!InputModeManager.Instance.Release(this))
        {
            return false;
        }

        HideInternal();
        return true;
    }    

    public void ReturnToLobby()
    {
        RequestHide();
        NetworkManager.Instance.ReturnToLobby();
        button.Interactable = false;
    }
}
