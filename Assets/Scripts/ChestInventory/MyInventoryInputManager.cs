using UnityEngine;
using System.Collections;

namespace MoreMountains.InventoryEngine
{
	public class MyInventoryInputManager: InventoryInputManager
	{

        static MyInventoryInputManager _instance;
        public static MyInventoryInputManager Instance { get { return _instance; } }
        public MyChestInventoryManager DefaultChestInventoryManager;
        private MyChestInventoryManager CurrentChestInventoryManager;
        public CanvasGroup TargetChestInventoryGroup;
        public CanvasGroup TargetInventoryButtonGroup;

        protected override void Start()
        {            
            base.Start();
            if (_instance == null)
            {
                _instance = this;
            }
            CurrentChestInventoryManager = DefaultChestInventoryManager;           
        }

        public override void OpenInventory()
        {
            TargetChestInventoryGroup.interactable = false;
            TargetChestInventoryGroup.blocksRaycasts = false;
            TargetInventoryButtonGroup.interactable = true;
            TargetInventoryButtonGroup.blocksRaycasts = true;

            TargetChestInventoryGroup.alpha = 0;            
            TargetInventoryButtonGroup.alpha = 1;

            C_ReqInventoryInfo pkt = new C_ReqInventoryInfo();
            pkt.clientTick = NetworkManager.Instance.tickScheduler.GetCurrentTick();
            NetworkManager.Instance.Send(pkt.Write());

            base.OpenInventory();
        }

        public override void CloseInventory()
        {               
            if(CurrentChestInventoryManager.IsOpenChest)
            {
                C_CloseChest pkt1 = new C_CloseChest();
                pkt1.clientTick = NetworkManager.Instance.tickScheduler.GetCurrentTick();
                pkt1.chestId = CurrentChestInventoryManager.CurrentChest.entityId;
                NetworkManager.Instance.Send(pkt1.Write());
                CurrentChestInventoryManager.IsOpenChest = false;
            }
            base.CloseInventory();
            
            //CurrentChestInventoryManager.SetCurrentChestInventory(null);
        }
        public virtual void OpenInventoryWithChest(uint index)        
        {
            TargetChestInventoryGroup.interactable = true;
            TargetChestInventoryGroup.blocksRaycasts = true;
            TargetInventoryButtonGroup.interactable = false;
            TargetInventoryButtonGroup.blocksRaycasts = false;

            TargetChestInventoryGroup.alpha = 1;
            TargetInventoryButtonGroup.alpha = 0;

            CurrentChestInventoryManager.IsOpenChest = true;
            base.OpenInventory();
            Debug.Log("Selected Chest : " + index.ToString());
        }

	}
}