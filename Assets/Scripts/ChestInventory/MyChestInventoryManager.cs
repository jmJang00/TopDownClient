using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using NUnit.Framework;
using UnityEngine;

public class MyChestInventoryManager : MonoBehaviour, MMEventListener<MMMyChestInventoryEvent>
{
    static MyChestInventoryManager _instance;
    public static MyChestInventoryManager Instance { get { return _instance; } }

    [SerializeField]
    private MyInventoryDisplay _defaultChestInventoryDisplay;
    public MyInventoryDisplay CurrentChestInventoryDisplay { get { return _defaultChestInventoryDisplay; } }

    [SerializeField]
    private MyInventoryDisplay _defaultInventoryDisplay;
    public MyInventoryDisplay CurrentInventoryDisplay { get { return _defaultInventoryDisplay; } }

    [SerializeField]
    private MyInventoryInputManager _defaultInventoryInputManager;
    public MyInventoryInputManager CurrentInventoryInputManager { get { return _defaultInventoryInputManager; } }

    [SerializeField]
    private MyInventory _defaultPlayerInventory;
    public MyInventory CurrentPlayerInventory { get { return _currentPlayerInventory; } }
    private MyInventory _currentPlayerInventory;

    public Chest CurrentChest;
    public MyInventory CurrentChestInventory;

    public bool IsOpenChest { get { return _isOpen; }  set { _isOpen = value; } }
    private bool _isOpen = false;

    private ushort _playerMaxAmmo = 0;
    private ushort _playerCurrentAmmo = 0;
    public ushort MaxUsableBullet { get { return _playerMaxAmmo; } }
    public ushort CurrentUsableBullet { get { return _playerCurrentAmmo; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_instance == null)
        {
            _instance = this;

            if (_defaultPlayerInventory == null)
            {
                Debug.LogError("플레이어 인벤토리를 지정해야합니다");
            }
            _currentPlayerInventory = _defaultPlayerInventory;            
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

    public void SetCurrentChest(Chest chest)
    {
        CurrentChest = chest;        
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

    public void UpdateAmmoInfo(ushort maxAmmo, ushort currentAmmo)
    {
        _playerMaxAmmo = maxAmmo;
        _playerCurrentAmmo = currentAmmo;
        UpdateAmmoDisplay();
    }

    public void UpdateAmmoDisplay()
    {
        GUIManager.Instance.SetAmmoDisplays(true, "Player1", 0);
        GUIManager.Instance.UpdateAmmoDisplays(false, _playerCurrentAmmo, _playerMaxAmmo, 0, 0, "Player1", 0, false);
    }

    public bool DecreaseAmmo()
    {
        if(_playerCurrentAmmo > 0)
        {
            --_playerCurrentAmmo;
            UpdateAmmoDisplay();
            return true;
        }
        else
        {
            return false;
        }
    }
}
