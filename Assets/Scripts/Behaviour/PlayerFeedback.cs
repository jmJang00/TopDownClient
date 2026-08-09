using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.PlayerLoop;

public class PlayerFeedback : NetBehaviour 
{
    public override int RenderingOrder => 4;

    public override ITickRunner Runner => null;

    public override NetBehaviourType Type => NetBehaviourType.Feedback;    

    [SerializeField]
    private MMFeedbacks PickerMMFeedback;

    public override void Init()
    {
        base.Init();
        PickerMMFeedback?.Initialization(this.gameObject);        
    }

    public override void OnSpawn(int tick)
    {
        base.OnSpawn(tick);        
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
    }
    
    public override void OnRender(float alpha)
    {

    }

    public override void DispatchPacket(IPacket packet)
    {
        switch (packet.Protocol)
        {
            case (ushort)PacketID.S_NtfDespawnItemPicker:
            {
                var p = packet as S_NtfDespawnItemPicker;
                _tickScheduler.ScheduleAt(p.serverTick, () =>
                {
                    PickerMMFeedback?.PlayFeedbacks(this.transform.position);
                });

                break;
            }
        }
    }    

    void Update()
    {       
    }
}
