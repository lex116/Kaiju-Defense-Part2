using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingState_Quick : StateMachineBehaviour
{
    //ShootingState 1

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //AddQuickShot();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AddQuickShot();
        Debug.Log(" adding a shot");
    }

    void AddQuickShot()
    {
        RoundManager RM = FindObjectOfType<RoundManager>();

        Action_QuickShot newQuickShot = new Action_QuickShot();

        newQuickShot.SetUp(RM.SelectedUnit, RM.SelectedUnit.TargetUnit);

        RM.AddAction(newQuickShot);
    }
}