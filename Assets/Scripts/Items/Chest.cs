using MoreMountains.InventoryEngine;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class Chest : NetEntity
{
    private MyInventory _inventory;
    private MyInventoryInputManager _inventoryInputManager;
    private MyChestInventoryManager _inventoryManager;    
    private Switch _switch;
    public override void Init()
    {
        base.Init();        

        _inventory = GetComponent<MyInventory>();
        _inventory.name = $"Chest{entityId}Inventory";
        _inventory.Index = entityId;
        _inventoryInputManager = MyInventoryInputManager.Instance;
        _inventoryManager = MyChestInventoryManager.Instance;        

        _switch = GetComponent<Switch>();
        _switch.SwitchToggle.AddListener(SetUpInventory);
    }    

    private void SetUpInventory()
    {
        _inventoryManager.CurrentChestInventoryDisplay.ChangeTargetInventory(_inventory.name);
        _inventoryManager.SetCurrentChestInventory(_inventory);
        _inventoryInputManager.OpenInventoryWithChest(entityId);
    }
}
