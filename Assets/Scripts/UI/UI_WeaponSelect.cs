using MoreMountains.Tools;
using System.Collections;
using UnityEngine;

public class UI_WeaponSelect : UI_Panel 
{
    public MMTouchButton laserButton;
    public MMTouchButton rifleButton;

    public void SelectLaserWeapon()
    {
        StartCoroutine(CoSelectWeapon(WeaponType.Laser));
    }

    public void SelectRifleWeapon()
    {
        StartCoroutine(CoSelectWeapon(WeaponType.Rifle));
    }

    public IEnumerator CoSelectWeapon(WeaponType type)
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
