using MoreMountains.Tools;
using System.Collections;
using UnityEngine;

public class UI_WeaponSelect : UI_Panel , IInputModeChangeable
{
    public MMTouchButton laserButton;
    public MMTouchButton rifleButton;

    public InputModeChangeableInfo GetInputModeInfo()
    {
        InputModeChangeableInfo info;
        info.Name = "UI";
        info.Description = "WeaponSelect UI";
        return info;
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


    public void SelectLaserWeapon()
    {
        SelectWeapon(WeaponType.Laser);
    }

    public void SelectRifleWeapon()
    {
        SelectWeapon(WeaponType.Rifle);
    }

    public void SelectWeapon(WeaponType type)
    {
        C_WeaponSelect weaponSelect = new C_WeaponSelect();
        weaponSelect.weaponId = (ushort)type;
        NetworkManager.Instance.GameSend(weaponSelect.Write());
    }
}
