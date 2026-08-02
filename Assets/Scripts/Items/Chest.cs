using MoreMountains.InventoryEngine;
using MoreMountains.TopDownEngine;
using System;
using UnityEngine;

public class Chest : NetEntity
{
    

    private MyInventory _inventory;    
    private MyChestInventoryManager _inventoryManager;    
    private Switch _switch;
    private ButtonActivatedZone _buttonActivatedZone;

    public int LastUpdateTick { get { return _lastUpdateTick; } }
    private int _lastUpdateTick;
    //Last Update Itemlist 시점의 Tick을 가지고있어서
    //클라이언트와 서버의 아이템리스트가 동기화가 안되어있을때
    //다시 최신의 Tick 기반의 ItemList를 가져오도록 변경.
    
    public override void Init()
    {
        base.Init();        

        _inventory = GetComponent<MyInventory>();
        _inventory.name = $"Chest{entityId}Inventory";
        _inventory.Index = entityId;        
        _inventoryManager = MyChestInventoryManager.Instance;        

        _switch = GetComponent<Switch>();
        _switch.SwitchToggle.AddListener(SetUpInventory);

        _buttonActivatedZone = GetComponent<ButtonActivatedZone>();
        _buttonActivatedZone.OnEnter.AddListener(OnEnterChest);
        _buttonActivatedZone.OnExit.AddListener(OnExitChest);
    }    

    public void SetLastUpdateTick(int serverTick)
    {
        _lastUpdateTick = serverTick;
    }

    private void SetUpInventory()
    {
        if (!_inventoryManager.IsOpenChest)
        {
            _inventoryManager.CurrentChestInventoryDisplay.ChangeTargetInventory(_inventory.name);
            _inventoryManager.SetCurrentChest(this);
            _inventoryManager.SetCurrentChestInventory(_inventory);

            int tick = NetworkManager.Instance.tickScheduler.GetCurrentTick();
            C_OpenInventory pkt = new C_OpenInventory();
            pkt.clientTick = tick;
            NetworkManager.Instance.Send(pkt.Write());

            C_OpenChest pkt2 = new C_OpenChest();
            pkt2.clientTick = tick;
            pkt2.chestId = this.entityId;
            NetworkManager.Instance.Send(pkt2.Write());
        }
    }

    private void OnEnterChest()
    {

    }

    private void OnExitChest()
    {
        if(_inventoryManager.IsOpenChest)
        {         
            _inventoryManager.CurrentInventoryInputManager.CloseInventory();
        }
    }
}
