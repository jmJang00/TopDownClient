using MoreMountains.TopDownEngine;
using System;
using System.Collections.Generic;
using System.Text;


public class CharacterHandleWeaponRevised : CharacterHandleWeapon
{    
    public override void ProcessAbility()
    {
        HandleCharacterState();
        HandleFeedbacks();        
        HandleBuffer();
    }    
}

