using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingGun : MonoBehaviour
{
    public bool updateWeapon = false;
    public bool fireWeapon = false;
    public bool resetStats = false;

    public int shotsFiredAtTarget = 0;
    public int shotsThatHitTarget = 0;
    public string shotAcc = null;

    public enum TestWeapons
    {
        Weapon_Human_Pistol,
        Weapon_Human_Shotgun,
        Weapon_Human_MachineGun,
        Weapon_Vehicle_MachineGun,
        Weapon_Human_AntiArmorRifle,
        Weapon_Vehicle_Cannon,
        Weapon_Human_SubMachineGun
    }
    [SerializeField]
    public TestWeapons selectedWeapon;
    public Weapon_Master currentWeapon;

    public float UnitStat_Accuracy = .90f;
    float Calculated_WeaponAccuracy;
    public float aimModifier;

    int ShotsFired;
    int BurstsFired;
    public bool isFiring;

    int shotLimit = 1;

    public GameObject AimingNode;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (updateWeapon)
        {
            fireWeapon = false;
            updateWeapon = false;
            equipWeapon();
        }

        if (fireWeapon == true)
        {
            if (currentWeapon == null)
            {
                equipWeapon();
            }
            updateWeapon = false;
            TestShooting(aimModifier);
        }

        if (shotsFiredAtTarget >= shotLimit)
        {
            fireWeapon = false;
        }

        if (resetStats)
        {
            shotsFiredAtTarget = 0;
            shotsThatHitTarget = 0;
            shotAcc = null;
            resetStats = false;
        }

        if (shotsFiredAtTarget != 0)
        {
            shotAcc = (float)((float)shotsThatHitTarget / (float)shotsFiredAtTarget)*100 + "%";
        }
    }

    void equipWeapon()
    {
        currentWeapon = (Weapon_Master)ScriptableObject.CreateInstance(selectedWeapon.ToString());
    }

    public void CalculateWeaponStats()
    {
        Calculated_WeaponAccuracy = (UnitStat_Accuracy + currentWeapon.Accuracy) / 2;
    }

    //public void Repeatshot()
    //{
    //    TestShooting(RpstAimTestValue);
    //}

    public void TestShooting(float accMod)
    {
        CalculateWeaponStats();

        if (!isFiring)
        {
            ShotsFired = 0;
            BurstsFired = 0;

            isFiring = true;
            StartCoroutine(TestShootingRoutine(accMod));
        }
    }


    IEnumerator TestShootingRoutine(float AccMod)
    {
        #region Fields
        ShotsFired = 0;
        IDamagable objectToBeDamaged;
        Vector3 DirectionToFire;

        //float Acc_W_Mod = 1 - ((Calculated_WeaponAccuracy * AccMod) / 1000);
        float Acc_W_Mod = ((Calculated_WeaponAccuracy * AccMod) / 1000) + .9f;
        #endregion 

        while (BurstsFired < currentWeapon.BurstCount)
        {
            ShotsFired = 0;

            while (ShotsFired < currentWeapon.ShotCount)
            {
                #region Shooting Code Block
                float randomXVector = UnityEngine.Random.Range((1f - Acc_W_Mod), ((1f - Acc_W_Mod) * -1));
                float randomYVector = UnityEngine.Random.Range((1f - Acc_W_Mod), ((1f - Acc_W_Mod) * -1));
                float randomZVector = UnityEngine.Random.Range((1f - Acc_W_Mod), ((1f - Acc_W_Mod) * -1));

                DirectionToFire =
                    AimingNode.transform.forward + new Vector3(randomXVector, randomYVector, randomZVector);

                RaycastHit hit;

                shotsFiredAtTarget++;

                if (Physics.Raycast(AimingNode.transform.position, DirectionToFire, out hit, Mathf.Infinity))
                {
                    objectToBeDamaged = hit.collider.gameObject.GetComponent<IDamagable>();

                    if (objectToBeDamaged != null)
                    {
                        shotsThatHitTarget++;
                        objectToBeDamaged.TakeDamage(currentWeapon.Damage, currentWeapon.damageType, this.name);
                    }

                    Debug.DrawLine(AimingNode.transform.position, hit.point, Color.red, 0.1f);

                        //GameObject dmgCube = Instantiate(DamageCube, hit.point, new Quaternion(0, 0, 0, 0));

                        //DamageEffect dmgCubeScript = dmgCube.GetComponent<DamageEffect>();

                        //dmgCubeScript.SetOrigin(AimingNode.transform.position);
                }

                #endregion

                ShotsFired++;

                if (currentWeapon.fireMode == Weapon_Master.FireModes.SingleShot)
                {
                    yield return new WaitForSeconds(currentWeapon.FireRate);
                }
            }

            BurstsFired++;

            if (currentWeapon.fireMode == Weapon_Master.FireModes.SpreadShot)
            {
                yield return new WaitForSeconds(currentWeapon.FireRate);
            }

        }

        isFiring = false;
    }
}