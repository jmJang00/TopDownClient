using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetEntity
{
    PlayerController _controller;
    Character _character;
    public int AccountId { get; set; }

    public override void Init()
    {
        base.Init();
        _character = GetComponent<Character>();
        _controller = GetComponent<PlayerController>();
    }

    public override void OnSpawn(int tick)
    {
        _character.RespawnAt(transform.position, Character.FacingDirections.East);
        base.OnSpawn(tick);
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
    }
}
