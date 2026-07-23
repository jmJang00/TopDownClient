using MoreMountains.TopDownEngine;
using UnityEngine;

public class Picker : NetEntity
{
    public DummyPicker picker;
    public override void Init()
    {
        base.Init();
        picker.transform.position = this.transform.position;
        picker.OnPick.AddListener(PickUpPicker);
    }    

    private void PickUpPicker()
    {
        C_ReqPickupItemPicker pkt = new C_ReqPickupItemPicker();
        pkt.clientTick = NetworkManager.Instance.tickScheduler.GetCurrentTick();
        pkt.entityId = this.entityId;

        NetworkManager.Instance.Send(pkt.Write());
    }
}
