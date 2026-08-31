using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;


//아래는 EntityType을 옮겨온 것임 피커들을 Entity로 병합하면서 같은 타입을 공유하는것이 나아보여
//기존의 itemType대신 entityType에 동작하도록 변경.
//총은 소환하지않을거니 제거하도록함.
//None = 0,
//MyPlayer,
//OtherPlayer,
//MyPlayerH,
//OtherPlayerH,
//Projectile,
//Chest,
//ExpPack,
//HealPack,
//AmmoP,
//AmmoH,
//Granade,


public enum ItemType
{
    ExpPack = 0,
    HealPack,
    AmmoP,    
    AmmoH,
    Shield,
    Grenade,        
    //...
    Max
}


public static class EnumToItemResource
{
    private static readonly string[] PickerPaths =
    {
        //총알, 힐팩, 경험치팩만 드롭될거임 나머지는 그냥 표시        
        "Prefabs/Items/Picker/ExpPack",
        "Prefabs/Items/Picker/HealPack",        
        "Prefabs/Items/Picker/AmmoP",
        "Prefabs/Items/Picker/AmmoH",
        "Prefabs/Items/Picker/Shield",
        "NotUse/Grenade",
        "NotUse/Max"
    };

    private static readonly string[] InventoryPaths =
   {        
        "Prefabs/Items/Inventory/Undefined",
        "Prefabs/Items/Inventory/HealPack",
        "Prefabs/Items/Inventory/LoftAssaultRifleAmmo",
        "Prefabs/Items/Inventory/LoftAssaultRifleHitscanAmmo",
        "Prefabs/Items/Inventory/Shield",
        "Undefined",
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

    public static InventoryItem GetWeaponItem(WeaponType type)
    {
        if(type == WeaponType.Rifle)
        {
            InventoryItem item = Resources.Load<InventoryItem>("Prefabs/Items/Inventory/LoftAssaultRifle");

            if (item == null)
            {
                Debug.LogError("Undefined Item Type");
                return null;
            }

            InventoryItem newItem = UnityEngine.Object.Instantiate(item);
            return newItem;            
        }
        else
        {
            InventoryItem item = Resources.Load<InventoryItem>("Prefabs/Items/Inventory/LoftAssaultRifleHitscan");

            if (item == null)
            {
                Debug.LogError("Undefined Item Type");
                return null;
            }

            InventoryItem newItem = UnityEngine.Object.Instantiate(item);
            return newItem;
        }
    }

    public static EntityType ConvertToEntityType(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.ExpPack:
                return EntityType.ExpPack;                
            case ItemType.HealPack:
                return EntityType.HealPack;                
            case ItemType.AmmoP:
                return EntityType.AmmoP;              
            case ItemType.AmmoH:
                return EntityType.AmmoH;
            case ItemType.Shield:
                return EntityType.Shield;
            default:
                return EntityType.None;                
        }
    }

}