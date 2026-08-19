using System.Collections.Generic;
using UnityEngine;

public class UI_GameMenu : MonoBehaviour
{
    [SerializeField] private UI_WeaponSelect weaponSelectPanel;
    [SerializeField] private UI_WinSplash victoryPanel;
    [SerializeField] private UI_DeathSplash defeatPanel;
    [SerializeField] private UI_PauseSplash pausePanel;
    [SerializeField] private UI_Spectate spectate;

    public void Update()
    {

    }

    public void ShowSpectate(uint entityId)
    {
        if (!spectate.IsVisible)
        {
            HideAll();
        }

        spectate.SetEntityId(entityId);

        if (!spectate.IsVisible)
        {
            spectate.RequestShow();
        }
    }

    public void ShowWeaponSelect()
    {        
        weaponSelectPanel.RequestShow();        
    }

    public void HideWeaponSelect()
    {
        weaponSelectPanel.RequestHide();
    }

    public void ShowVictory(bool isWinner, IReadOnlyList<PlayerResult> results)
    {        
        victoryPanel.SetGameEndInfo(isWinner, results);
        victoryPanel.RequestShow();        
    }

    public void HideVictory()
    {
        victoryPanel.RequestHide();
    }

    public void ShowDefeat()
    {        
        defeatPanel.RequestShow();        
    }

    public void HideDefeat()
    {
        defeatPanel.RequestHide();
    }

    public void ShowPause()
    {        
        pausePanel.RequestShow();        
    }

    public void HidePause()
    {
        pausePanel.RequestHide();
    }

    public void HideAll()
    {
        if(weaponSelectPanel.IsVisible)
            weaponSelectPanel.RequestHide();
        if(victoryPanel.IsVisible)
            victoryPanel.RequestHide();
        if(defeatPanel.IsVisible)
            defeatPanel.RequestHide();
        if(pausePanel.IsVisible)
            pausePanel.RequestHide();
        if (spectate.IsVisible)
            spectate.RequestHide();
    }

    public InputModeChangeableInfo GetDescription()
    {
        InputModeChangeableInfo info;
        info.Name = "UI";
        info.Description = "Pause UI";
        return info;
    }
}
