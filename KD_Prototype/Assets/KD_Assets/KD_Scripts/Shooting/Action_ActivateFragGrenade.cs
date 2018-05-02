﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_ActivateFragGrenade : TimeScaleAction
{
    //temp
    public Human_FragGrenade thisGrenade;

    public Action_ActivateFragGrenade()
    {
        actionName = "Frag Grenade";
        timeScaleOffSet = 2;
    }

    public override void ActionEffect()
    {
        thisGrenade.Explode();
    }
}
