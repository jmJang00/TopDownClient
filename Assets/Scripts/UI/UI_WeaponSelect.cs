using MoreMountains.Tools;
using System.Collections;
using UnityEngine;

public class UI_WeaponSelect : UI_Panel 
{
    public MMTouchButton laserButton;
    public MMTouchButton rifleButton;

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
        NetworkManager.Instance.Send(weaponSelect.Write());
    }
}
