using UnityEngine;

public class UI_GameEnd : MonoBehaviour
{
    [SerializeField] private UI_Panel weaponSelectPanel;
    [SerializeField] private UI_Panel victoryPanel;
    [SerializeField] private UI_Panel defeatPanel;

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

    public void HideAll()
    {
        weaponSelectPanel.Hide();
        victoryPanel.Hide();
        defeatPanel.Hide();
    }
}
