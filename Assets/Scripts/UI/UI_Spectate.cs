using UnityEngine;
using TMPro;
using MoreMountains.Tools;

public class UI_Spectate : UI_Panel
{
    public TMP_Text nickname;
    public MMTouchButton prevButton;
    public MMTouchButton nextButton;
    public uint entityId;

    void Start()
    {
        prevButton.ButtonReleased.AddListener(SpectatePrev);
        nextButton.ButtonReleased.AddListener(SpectateNext);
    }

    void Update()
    {
        
    }

    public override bool RequestHide()
    {
        return true;
    }

    public override bool RequestShow()
    {
        return true;
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
        if (PlayerManager.Instance.TryGetPlayer(entityId, out var player))
        {
            player.Spectate();
            nickname.text = AccountManager.Instance.GetNickname(player.AccountId);
        }
    }
}
