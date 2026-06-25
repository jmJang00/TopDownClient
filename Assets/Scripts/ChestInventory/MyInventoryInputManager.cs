using UnityEngine;
using System.Collections;

namespace MoreMountains.InventoryEngine
{
	public class MyInventoryInputManager: InventoryInputManager
	{
        public MyChestInventoryManager DefaultChestInventoryManager;
        private MyChestInventoryManager CurrentChestInventoryManager;
        public CanvasGroup TargetChestInventoryGroup;
        public CanvasGroup TargetInventoryButtonGroup;

        protected override void Start()
        {
            base.Start();
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
            pkt.clientTick = 0xFFEE;
            NetworkManager.Instance.Send(pkt.Write());

            base.OpenInventory();
        }

        public override void CloseInventory()
        {            
            base.CloseInventory();
            CurrentChestInventoryManager.SetCurrentChestInventory(null);
        }
        public virtual void OpenInventoryWithChest(int index)        
        {
            TargetChestInventoryGroup.interactable = true;
            TargetChestInventoryGroup.blocksRaycasts = true;
            TargetInventoryButtonGroup.interactable = false;
            TargetInventoryButtonGroup.blocksRaycasts = false;

            TargetChestInventoryGroup.alpha = 1;
            TargetInventoryButtonGroup.alpha = 0;

            C_ReqInventoryInfo pkt = new C_ReqInventoryInfo();
            pkt.clientTick = 0xFFEE;
            NetworkManager.Instance.Send(pkt.Write());

            C_ReqChestInfo pkt2 = new C_ReqChestInfo();
            pkt2.clientTick = 0xFFCC;
            pkt2.chestId = (uint)index;
            NetworkManager.Instance.Send(pkt2.Write());

            base.OpenInventory();
            Debug.Log("Selected Chest : " + index.ToString());
        }        
	}
}