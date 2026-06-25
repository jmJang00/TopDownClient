using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using UnityEngine;

public enum MMMyChestInventoryEventType { SetCurrentChestInventory, ChestToInventory, InventoryToChest  }

/// <summary>
/// Inventory events are used throughout the Inventory Engine to let other interested classes know that something happened to an inventory.  
/// </summary>
public struct MMMyChestInventoryEvent
{
    /// the type of event
    public MMMyChestInventoryEventType ChestInventoryEventType;
    /// the slot involved in the event
    public InventorySlot InventoryTargetSlot;
    public InventorySlot ChestTargetSlot;
    public string ChestInventoryName;

    public MMMyChestInventoryEvent(MMMyChestInventoryEventType eventType, InventorySlot inventoryslot, InventorySlot chestSlot , string chestInventoryName)
    {
        ChestInventoryEventType = eventType;
        InventoryTargetSlot = inventoryslot;
        ChestTargetSlot = chestSlot;
        ChestInventoryName = chestInventoryName;
    }

    static MMMyChestInventoryEvent e;
    public static void Trigger(MMMyChestInventoryEventType eventType, InventorySlot inventoryslot, InventorySlot chestSlot, string chestInventoryName)
    {
        e.ChestInventoryEventType = eventType;
        e.InventoryTargetSlot = inventoryslot;
        e.ChestTargetSlot = chestSlot;
        e.ChestInventoryName = chestInventoryName;
        MMEventManager.TriggerEvent(e);
    }
}