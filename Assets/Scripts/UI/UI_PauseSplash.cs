using MoreMountains.Tools;
using UnityEngine;

public class UI_PauseSplash : UI_Panel , IInputModeChangeable
{
    public MMTouchButton resumeButton;
    public MMTouchButton returnButton;

    public void Start()
	{
        resumeButton.ButtonReleased.AddListener(ResumeGame);
        returnButton.ButtonReleased.AddListener(QuitGame);
	}
    public void Update()
    {
        if (!IsVisible)
        {
            if (InputModeManager.Instance.CurrentMode != InputMode.Game)
                return;

            if (Input.GetKeyDown(KeyCode.Escape))            
                RequestShow();
            
            return;
        }

        if (InputModeManager.Instance.CurrentMode != InputMode.UI)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
            RequestHide();
    }
    public void OnDestroy()
    {
        if (IsVisible)
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

    private void ResumeGame()
    {
        RequestHide();
    }

    private void QuitGame()
    {
        RequestHide();
        NetworkManager.Instance.QuitGame();
    }

    public InputModeChangeableInfo GetInputModeInfo()
    {
        InputModeChangeableInfo info;
        info.Name = "UI";
        info.Description = "PauseSplash UI";
        return info;
    }
}
