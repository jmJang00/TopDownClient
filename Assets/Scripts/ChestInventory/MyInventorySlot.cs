using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if MM_UGUI2
using TMPro;
#endif

namespace MoreMountains.InventoryEngine
{
    public class MyInventorySlot : InventorySlot , IPointerClickHandler
    {
       
        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            //오른쪽 클릭일경우.
            if(eventData.button == PointerEventData.InputButton.Right)
            {
                MyInventory inven = (MyInventory)ParentInventoryDisplay.TargetInventory;
                MyChestInventoryManager manager = MyChestInventoryManager.Instance;
                InventoryItem item = inven.Content[Index];

                if(this.CurrentItem == null)
                {
                    return;
                }

                //인벤토리에서 상자로.
                if(inven.isPlayer)
                {
                    C_ReqInventoryToChest pkt = new C_ReqInventoryToChest();
                    pkt.clientTick = 0xFFBB;
                    pkt.chestId = manager.CurrentChestInventory.Index;
                    pkt.inventoryCursor = (uint)this.Index;
                    NetworkManager.Instance.Send(pkt.Write());

                    //MMMyChestInventoryEvent.Trigger(MMMyChestInventoryEventType.InventoryToChest,
                    //    this,
                    //    null,
                    //    "");
                }
                //상자에서 인벤토리로.
                else
                {

                    C_ReqChestToInventory pkt = new C_ReqChestToInventory();
                    pkt.clientTick = 0xFFAA;
                    pkt.chestId = manager.CurrentChestInventory.Index;
                    pkt.chestCursor = (uint)this.Index;
                    NetworkManager.Instance.Send(pkt.Write());

                    //MMMyChestInventoryEvent.Trigger(MMMyChestInventoryEventType.ChestToInventory,
                    //    null,
                    //    this,
                    //    "");
                }
                
                Debug.Log($"RightClick! Index : {Index}, item : {item.name}");
            }
        }


   
    }
}
