using UnityEngine;

public class UI_GameEnd : MonoBehaviour
{
    [SerializeField] private UI_WeaponSelect weaponSelectPanel;
    [SerializeField] private UI_WinSplash victoryPanel;
    [SerializeField] private UI_DeathSplash defeatPanel;
    [SerializeField] private UI_PauseSplash pausePanel;

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (!pausePanel.IsVisible)
            {
                pausePanel.Show();
            }
        }
    }

    public void ShowWeaponSelect()
    {
        weaponSelectPanel.Show();
    }

    public void ShowVictory()
    {
        victoryPanel.Show();
    }

    public void ShowDefeat()
    {
        defeatPanel.Show();
    }

    public void ShowPause()
    {
        pausePanel.Show();
    }

    public void HideAll()
    {
        weaponSelectPanel.Hide();
        victoryPanel.Hide();
        defeatPanel.Hide();
        pausePanel.Hide();
    }
}
