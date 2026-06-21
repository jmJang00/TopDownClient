using MoreMountains.Tools;
using System.Collections;
using UnityEngine;

public enum SelectWeaponType
{
    Rifle = 0,
    Laser = 1,
}

public class UI_WeaponSelect : UI_Panel 
{
    public MMTouchButton laserButton;
    public MMTouchButton rifleButton;

    public void SelectLaserWeapon()
    {
        StartCoroutine(CoSelectWeapon(SelectWeaponType.Laser));
    }

    public void SelectRifleWeapon()
    {
        StartCoroutine(CoSelectWeapon(SelectWeaponType.Rifle));
    }

    public IEnumerator CoSelectWeapon(SelectWeaponType type)
    {
        C_WeaponSelect weaponSelect = new C_WeaponSelect();
        weaponSelect.weaponId = (ushort)type;
        NetworkManager.Instance.Send(weaponSelect.Write());
        laserButton.Interactable = false;
        rifleButton.Interactable = false;
        yield return new WaitForSeconds(2.0f);
        laserButton.Interactable = true;
        rifleButton.Interactable = true;
    }
}
