using MoreMountains.InventoryEngine;
using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;

public enum ItemType
{
    ProjectileWeapon,
    HitscanWeapon,
    ProjectileAmmo,
    HitscanAmmo,
    Grenade,
    HealPack,
    ExpPack,
    //.....
    Max
}

public static class EnumToItemResource
{
    private static readonly string[] PickerPaths =
    {
        "Prefabs/Items/Picker/LoftAssaultRiifle",
        "Prefabs/Items/Picker/LoftAssaultRifleHitscan",
        "Prefabs/Items/Picker/LoftAmmoCrateAssaultRifle",
        "Prefabs/Items/Picker/LoftAmmoCrateAssaultRifleHitscan",
        "Prefabs/Items/Picker/LoftStimpack",
        "Prefabs/Items/Picker/LoftStimpack",
        "Prefabs/Items/Picker/LoftStimpack",
        "Prefabs/Items/Picker/LoftStimpack",
        "Undefined"
    };

    private static readonly string[] InventoryPaths =
   {
        "Prefabs/Items/Inventory/LoftAssaultRifle",
        "Prefabs/Items/Inventory/LoftAssaultRifleHitscan",
        "Prefabs/Items/Inventory/LoftAssaultRifleAmmo",
        "Prefabs/Items/Inventory/LoftAssaultRifleHitscanAmmo",
        "Prefabs/Items/Inventory/Undefined",
        "Prefabs/Items/Inventory/Undefined",
        "Prefabs/Items/Inventory/Undefined",
        "Prefabs/Items/Inventory/Undefined",
        "Undefined"
    };

    public static string GetPickerPath(ItemType type)
    {
        return PickerPaths[(int)type];
    }

    public static string GetInventoryPath(ItemType type)
    {
        return InventoryPaths[(int)type];
    }

    public static GameObject GetPickerPrefab(ItemType type)
    {        
        GameObject item = Resources.Load<GameObject>(GetPickerPath(type));
        if(item == null)
        {
            Debug.LogError("Undefined Item Type");
            return null;
        }

        GameObject newItem = UnityEngine.Object.Instantiate(item);
        return newItem;
    }

    public static InventoryItem GetNewInventoryItem(ItemType type)
    {
        InventoryItem item = Resources.Load<InventoryItem>(GetInventoryPath(type));

        if (item == null)
        {
            Debug.LogError("Undefined Item Type");
            return null;
        }

        InventoryItem newItem = UnityEngine.Object.Instantiate(item);
        return newItem;

        
    }

}