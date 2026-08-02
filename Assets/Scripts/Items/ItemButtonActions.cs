using MoreMountains.InventoryEngine;
using UnityEngine;

public class ItemButtonActions : MonoBehaviour
{    

    public void OnClickUseButton()
    {        
        C_ReqUseItem pkt = new C_ReqUseItem();
        pkt.clientTick = NetworkManager.Instance.tickScheduler.GetCurrentTick();
        pkt.lastInventoryUpdateTick = MyChestInventoryManager.Instance.CurrentPlayerInventory.LastUpdateTick;
        pkt.inventoryCursor = (uint)MyChestInventoryManager.Instance.CurrentInventoryDisplay.CurrentlySelectedInventorySlot().Index;

        NetworkManager.Instance.Send(pkt.Write());
    }

    public void OnClickDropButton()
    {
        C_ReqDropItem pkt = new C_ReqDropItem();
        pkt.clientTick = NetworkManager.Instance.tickScheduler.GetCurrentTick();
        pkt.lastInventoryUpdateTick = MyChestInventoryManager.Instance.CurrentPlayerInventory.LastUpdateTick;
        pkt.inventoryCursor = (uint)MyChestInventoryManager.Instance.CurrentInventoryDisplay.CurrentlySelectedInventorySlot().Index;

        NetworkManager.Instance.Send(pkt.Write());
    }
    
}
