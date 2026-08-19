using MoreMountains.Tools;
using UnityEngine;

public class UI_DeathSplash : UI_Panel , IInputModeChangeable
{
    public MMTouchButton returnButton;
    public MMTouchButton spectateButton;


    private void Start()
    {
        returnButton.ButtonReleased.AddListener(ReturnToLobby);
        spectateButton.ButtonReleased.AddListener(Spectate);
    }

    private void OnEnable()
    {
        returnButton.Interactable = true;
        spectateButton.Interactable = true;
    }

    public InputModeChangeableInfo GetInputModeInfo()
    {
        InputModeChangeableInfo info;
        info.Name = "UI";
        info.Description = "DeathSplash UI";
        return info;
    }

    public void OnDestroy()
    {
        if(IsVisible)
        {
            RequestHide();
        }
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
        returnButton.Interactable = false;
    }

    public async void Spectate()
    {
        C_ReqSpectateGame spectate = new C_ReqSpectateGame();
        var user = await NetworkManager.Instance.GameSendRequest<S_NtfSpectateUser>(spectate);
        NetworkManager.Instance.tickScheduler.ScheduleAt(user.tick, () =>
        {
            var gameScene = NetworkManager.Instance.game;
            gameScene.gameSelectUI.ShowSpectate(user.entityId);
        });
        spectateButton.Interactable = false;
    }
}
