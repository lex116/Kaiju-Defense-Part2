using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_Tank : Unit_VehicleMaster
{
    public GameObject Turret;

    public override void TrackSuppressTarget()
    {
        Turret.transform.LookAt(suppressionTarget.transform);

        Vector3 turretEulerAngles = Turret.transform.rotation.eulerAngles;
        turretEulerAngles.x = 0;
        turretEulerAngles.z = 0;

        Turret.transform.rotation = Quaternion.Euler(turretEulerAngles);

        //Rotate the camera to face the target
        aimingNode.transform.LookAt(suppressionTarget.transform);

        Vector3 camEulerAngles = aimingNode.transform.rotation.eulerAngles;
        camEulerAngles.y = turretEulerAngles.y;
        camEulerAngles.z = 0;

        aimingNode.transform.rotation = Quaternion.Euler(camEulerAngles);
    }

    //public override void LookAtTarget()
    //{
    //    LookedAtUnit_Master = null;
    //    LookedAtUnit_VehicleHardPoint = null;

    //    RaycastHit hit;

    //    if (Physics.Raycast(aimingNode.transform.position, aimingNode.transform.forward, out hit, equippedWeapon.Range))
    //    {
    //        if (hit.collider.gameObject.name != null)
    //        {
    //            LookedAtUnit_Master = hit.collider.gameObject.GetComponent<Unit_Master>();
    //            LookedAtUnit_VehicleHardPoint = hit.collider.gameObject.GetComponent<Unit_VehicleHardPoint>();
    //        }
    //    }
    //}
}

