using MoreMountains.TopDownEngine;
using UnityEngine;

public class Picker : NetEntity
{
    public DummyPicker picker;
    public override void Init()
    {
        base.Init();
        picker.transform.position = this.transform.position;
    }    
}
