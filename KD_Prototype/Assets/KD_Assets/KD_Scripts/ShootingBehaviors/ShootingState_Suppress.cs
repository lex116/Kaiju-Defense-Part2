using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingState_Suppress : StateMachineBehaviour
{
    //ShootingState 3

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AddSuppressShot();
    }

    void AddSuppressShot()
    {
        RoundManager RM = FindObjectOfType<RoundManager>();

        Action_SuppressShot newSuppressShot = new Action_SuppressShot();

        newSuppressShot.SetUp(RM.SelectedUnit, RM.SelectedUnit.TargetUnit);

        RM.AddAction(newSuppressShot);
    }
}