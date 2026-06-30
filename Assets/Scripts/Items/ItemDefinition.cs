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


//아래는 다음 커밋때 제거하자.
//public enum ItemType
//{
//    ProjectileWeapon,
//    HitscanWeapon,
//    ProjectileAmmo,
//    HitscanAmmo,
//    Grenade,
//    HealPack,
//    ExpPack,
//    //.....
//    Max
//}

public static class EnumToItemResource
{
    private static readonly string[] PickerPaths =
    {
        //총알, 힐팩, 경험치팩만 드롭될거임 나머지는 그냥 표시
        "NotUse/None",
        "NotUse/MyPlayer",
        "NotUse/OtherPlayer",
        "NotUse/MyPlayerH",
        "NotUse/OtherPlayerH",
        "NotUse/Projectile",
        "NotUse/Chest",
        "Prefabs/Items/Picker/ExpPack",
        "Prefabs/Items/Picker/HealPack",        
        "Prefabs/Items/Picker/AmmoP",
        "Prefabs/Items/Picker/AmmoH",
        "NotUse/Max"
    };

    private static readonly string[] InventoryPaths =
   {
        "NotUse/None",
        "NotUse/MyPlayer",
        "NotUse/OtherPlayer",
        "NotUse/MyPlayerH",
        "NotUse/OtherPlayerH",
        "NotUse/Projectile",
        "NotUse/Chest",
        "Prefabs/Items/Inventory/Undefined",
        "Prefabs/Items/Inventory/Undefined",
        "Prefabs/Items/Inventory/LoftAssaultRifleAmmo",
        "Prefabs/Items/Inventory/LoftAssaultRifleHitscanAmmo",
        "Undefined"
    };

    public static string GetPickerPath(EntityType type)
    {
        return PickerPaths[(int)type];
    }

    public static string GetInventoryPath(EntityType type)
    {
        return InventoryPaths[(int)type];
    }

    public static GameObject GetPickerPrefab(EntityType type)
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

    public static InventoryItem GetNewInventoryItem(EntityType type)
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