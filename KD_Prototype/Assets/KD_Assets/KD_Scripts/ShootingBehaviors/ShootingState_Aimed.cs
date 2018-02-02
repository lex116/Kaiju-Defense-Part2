using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingState_Aimed : StateMachineBehaviour
{
    //ShootingState 2

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AddAimedShot();
    }

    void AddAimedShot()
    {
        RoundManager RM = FindObjectOfType<RoundManager>();

        Action_AimedShot newAimedShot = new Action_AimedShot();

        newAimedShot.SetUp(RM.SelectedUnit, RM.SelectedUnit.TargetUnit);

        RM.AddAction(newAimedShot);
    }
}