using MoreMountains.Tools;
using UnityEngine;

public class UI_PauseSplash : UI_Panel 
{
    public MMTouchButton resumeButton;
    public MMTouchButton returnButton;

    public void Start()
	{
        resumeButton.ButtonReleased.AddListener(Hide);
        returnButton.ButtonReleased.AddListener(QuitGame);
	}

    public void QuitGame()
    {
        NetworkManager.Instance.QuitGame();
    }
}
