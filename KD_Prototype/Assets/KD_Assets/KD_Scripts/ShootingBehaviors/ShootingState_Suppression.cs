using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingState_Suppression : StateMachineBehaviour
{
    //Suppression Loop?

    public Unit tempUnit;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //AddSuppressShot();
    }
    
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if(tempUnit == null)
        {
            tempUnit = animator.gameObject.GetComponent<Unit>();
            Debug.Log("Hit");
        }

        tempUnit.SuppressionUpdate();
    }
}