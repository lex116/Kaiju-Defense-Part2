using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingGun : MonoBehaviour
{
    public enum DemoWeapon
    {
        Human_Pistol,
        Human_Shotgun,
        Human_MachineGun,
        Vehicle_MachineGun
    }

    [SerializeField]
    public DemoWeapon selectedWeapon;

    public float UnitStat_Accuracy = .90f;

    public Weapon currentWeapon;
    public GameObject DamageCube;

    int ShotsFired;
    int BurstsFired;
    bool isFiring;

    float Calculated_WeaponAccuracy;

    public GameObject AimingNode;

    public float RpstAimTestValue;

    // Use this for initialization
    void Awake()
    {
        if (selectedWeapon == DemoWeapon.Human_Pistol)
        {
            currentWeapon = (Weapon)ScriptableObject.CreateInstance("Human_Pistol");
        }

        if (selectedWeapon == DemoWeapon.Human_Shotgun)
        {
            currentWeapon = (Weapon)ScriptableObject.CreateInstance("Human_Shotgun");
        }

        if (selectedWeapon == DemoWeapon.Human_MachineGun)
        {
            currentWeapon = (Weapon)ScriptableObject.CreateInstance("Human_MachineGun");
        }

        if (selectedWeapon == DemoWeapon.Vehicle_MachineGun)
        {
            currentWeapon = (Weapon)ScriptableObject.CreateInstance("Vehicle_MachineGun");
        }

        InvokeRepeating("Repeatshot", 0f, 2f);
    }

    public void Repeatshot()
    {
        TestShooting(RpstAimTestValue);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            TestShooting(1f);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            TestShooting(.95f);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            TestShooting(.9f);
        }
    }

    public void CalculateWeaponStats()
    {
        Calculated_WeaponAccuracy = (UnitStat_Accuracy + currentWeapon.Accuracy) / 2;
    }


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

        float Acc_W_Mod = Calculated_WeaponAccuracy * AccMod;
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

                if (Physics.Raycast(AimingNode.transform.position, DirectionToFire, out hit, Mathf.Infinity))
                {
                    objectToBeDamaged = hit.collider.gameObject.GetComponent<IDamagable>();

                        if (objectToBeDamaged != null)
                        {
                            objectToBeDamaged.TakeDamage(currentWeapon.Damage);
                        }

                        GameObject dmgCube = Instantiate(DamageCube, hit.point, new Quaternion(0, 0, 0, 0));

                        DamageCube dmgCubeScript = dmgCube.GetComponent<DamageCube>();

                        dmgCubeScript.SetOrigin(AimingNode.transform.position);
                }

                #endregion

                ShotsFired++;

                if (currentWeapon.thisFireMode == Weapon.FireModes.SingleShot)
                {
                    yield return new WaitForSeconds(currentWeapon.FireRate);
                }
            }

            BurstsFired++;

            if (currentWeapon.thisFireMode == Weapon.FireModes.SpreadShot)
            {
                yield return new WaitForSeconds(currentWeapon.FireRate);
            }

        }

        isFiring = false;
    }
}