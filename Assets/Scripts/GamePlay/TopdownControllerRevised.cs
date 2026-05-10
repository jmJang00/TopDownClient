using System;
using System.Collections;
using UnityEngine;
using MoreMountains.TopDownEngine;
using UnityEditor.Experimental.GraphView;

public class TopdownControllerRevised : TopDownController3D
{
    public override bool CollidingAbove() { return false; }

    /// <summary>
    /// Moves the character controller by the computed _motion 
    /// </summary>
    protected override void MoveCharacterController()
    {
        GroundNormal.x = GroundNormal.y = GroundNormal.z = 0f;

        _collisionFlags = _characterController.Move(new Vector3(0, _motion.y, 0));

        _lastHitPoint = _hitPoint;
        _lastGroundNormal = GroundNormal;
    }
}
