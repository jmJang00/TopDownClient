using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : NetEntity 
{
    PlayerController _controller;
    Character _character;

    public uint AccountId { get; set; }

    public override void Init() 
    { 
        base.Init();
        _character = GetComponent<Character>();
        _controller = GetComponent<PlayerController>();
    }

    public override void OnSpawn(int t)
    {
        MMCameraEvent.Trigger(MMCameraEventTypes.SetTargetCharacter, _character);
        //TopDownEngineEvent.Trigger(TopDownEngineEventTypes.SpawnCharacterStarts, null);

        _character.RespawnAt(transform.position, Character.FacingDirections.East);
        TopDownEngineEvent.Trigger(TopDownEngineEventTypes.SpawnComplete, _character);

        MMSetFeedbackRangeCenterEvent.Trigger(_character.transform);

        MMCameraEvent.Trigger(MMCameraEventTypes.SetTargetCharacter, _character);
        MMCameraEvent.Trigger(MMCameraEventTypes.StartFollowing);
        MMGameEvent.Trigger("CameraBound");

        base.OnSpawn(t);
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
    }
}
