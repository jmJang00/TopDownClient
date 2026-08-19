using UnityEngine;
using TMPro;
using MoreMountains.Tools;

public class UI_Spectate : UI_Panel , IInputModeChangeable
{
    public TMP_Text nickname;
    public MMTouchButton prevButton;
    public MMTouchButton nextButton;
    public uint spectatedEntityId;
    private Player _player;

    void Start()
    {
        prevButton.ButtonReleased.AddListener(SpectatePrev);
        nextButton.ButtonReleased.AddListener(SpectateNext);
    }

    void Update()
    {
        if (_player == null)
        {
            if (PlayerManager.Instance.TryGetPlayer(spectatedEntityId, out _player))
            {
                _player.Spectate();
                nickname.text = AccountManager.Instance.GetNickname(_player.AccountId);
            }
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

    public InputModeChangeableInfo GetInputModeInfo()
    {
        InputModeChangeableInfo info;
        info.Name = "UI";
        info.Description = "Spectate UI";
        return info;
    }

    public void SetEntityId(uint id)
    {
        Spectate(id);
    }

    async void SpectateNext()
    {
        nextButton.DisableButton();
        C_ReqSpectateNext next = new C_ReqSpectateNext();
        var user = await NetworkManager.Instance.GameSendRequest<S_NtfSpectateUser>(next);
        NetworkManager.Instance.tickScheduler.ScheduleAt(user.tick, () =>
        {
            Spectate(user.entityId);
        });
        nextButton.EnableButton();
    }

    async void SpectatePrev()
    {
        prevButton.DisableButton();
        C_ReqSpectatePrev next = new C_ReqSpectatePrev();
        var user = await NetworkManager.Instance.GameSendRequest<S_NtfSpectateUser>(next);
        NetworkManager.Instance.tickScheduler.ScheduleAt(user.tick, () =>
        {
            Spectate(user.entityId);
        });
        prevButton.EnableButton();
    }

    public void Spectate(uint entityId)
    {
        spectatedEntityId = entityId;
        _player = null;
    }
}
