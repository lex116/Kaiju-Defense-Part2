using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret_Behavior : MonoBehaviour
{
    //Rotates the turret towards it's current target
    public void AimAtTarget(GameObject Turret, GameObject Target, float TurretMovementSpeed)
    {
        Vector3 targetDir = Target.transform.position - Turret.transform.position;

        float turnSpeed = TurretMovementSpeed * Time.deltaTime;

        Vector3 newDir = Vector3.RotateTowards(Turret.transform.forward, targetDir, turnSpeed, 1f);

        Turret.transform.rotation = Quaternion.LookRotation(newDir);
    }

    //Fires the weapon
    public void Shoot(RangedWeapon_Class Weapon)
    {
        Weapon.Fire();
    }

    //Reset our turret's rotation
    public void ResetOrientation(GameObject Turret, float TurretMovementSpeed)
    {
        Turret.transform.localRotation = 
            Quaternion.Slerp(Turret.transform.localRotation, Quaternion.Euler(0, 0, 0), TurretMovementSpeed * Time.deltaTime);
    }
}