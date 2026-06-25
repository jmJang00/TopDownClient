using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using NUnit.Framework;
using UnityEngine;

public class MyChestInventoryManager : MMSingleton<MyChestInventoryManager>, MMEventListener<MMMyChestInventoryEvent>
{
    static MyChestInventoryManager _instance;
    public static MyChestInventoryManager Instance { get { return _instance; } }

    
    public MyInventory DefaultPlayerInventory;
    public MyInventory CurrentPlayerInventory { get { return _currentPlayerInventory; } }
    private MyInventory _currentPlayerInventory;    
    public MyInventory CurrentChestInventory;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_instance == null)
        {
            if (DefaultPlayerInventory == null)
            {
                Debug.LogError("플레이어 인벤토리를 지정해야합니다");
            }
            _currentPlayerInventory = DefaultPlayerInventory;

            _instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        this.MMEventStartListening<MMMyChestInventoryEvent>();
    }

    private void OnDisable()
    {
        this.MMEventStopListening<MMMyChestInventoryEvent>();
    }

    public void SetCurrentChestInventory(MyInventory inventory)
    {
        CurrentChestInventory = inventory;
    }


    public void OnMMEvent(MMMyChestInventoryEvent eventType)
    {
        switch(eventType.ChestInventoryEventType)
        {
            case MMMyChestInventoryEventType.InventoryToChest:
                {                    
                    //상자 인벤토리가 가득 찼느냐.
                    if (CurrentChestInventory == null || CurrentChestInventory.IsFull)
                    {
                        return;
                    }

                    MyInventorySlot slot = eventType.InventoryTargetSlot as MyInventorySlot;
                    int inventoryTargetIndex = slot.Index;

                    InventoryItem item = slot.CurrentItem.Copy();
                    CurrentPlayerInventory.DestroyItem(inventoryTargetIndex);
                    CurrentChestInventory.AddItem(item, item.Quantity);
                }
                break;
            case MMMyChestInventoryEventType.ChestToInventory:
                {
                    //플레이어 인벤토리가 가득찼냐.
                    if (CurrentPlayerInventory == null || CurrentPlayerInventory.IsFull)
                    {
                        return;
                    }


                    MyInventorySlot slot = eventType.ChestTargetSlot as MyInventorySlot;
                    int chestTargetIndex = slot.Index;

                    InventoryItem item = slot.CurrentItem.Copy();
                    CurrentChestInventory.DestroyItem(chestTargetIndex);
                    CurrentPlayerInventory.AddItem(item, item.Quantity);

                }
                break;


        }
    }
}
