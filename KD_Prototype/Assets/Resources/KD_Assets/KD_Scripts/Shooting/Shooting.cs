using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    ReferenceManager referenceManager;
    internal Weapon_Master [] Weapons;

    Weapon_Master currentWeapon;

    public Unit_Master unit;

    public int ShotsFired = 0;
    public int BurstsFired = 0;

    public bool isFiring;

    public GameObject DamageCube;

    #region Thomas Shooting
    //private void DetectAccuracy()
    //{
    //    float accuracy = 0;
    //    int accuracyMeasurementsIncriments = 50;
    //    //draw 100 raycasts in the hit zone, of the 100 add 1 to the accuracy %
    //    float startX = Screen.width / 2 - (referenceManager.HitBoxImage.rectTransform.rect.width / 2);
    //    float endX = Screen.width / 2 + (referenceManager.HitBoxImage.rectTransform.rect.width / 2);
    //    float currentX = startX;
    //    float incrimentX = Mathf.Abs(endX - startX) / accuracyMeasurementsIncriments;

    //    float startY = Screen.height / 2 - (referenceManager.HitBoxImage.rectTransform.rect.height / 2);
    //    float endY = Screen.height / 2 + (referenceManager.HitBoxImage.rectTransform.rect.height / 2);
    //    float currentY = Screen.height / 2;
    //    float incrimentY = Mathf.Abs(endY - startY) / accuracyMeasurementsIncriments;

    //    //measure across X axis
    //    for (int i = 0; i < accuracyMeasurementsIncriments; i++)
    //    {
    //        accuracy += DrawRaycast(currentX, currentY);
    //        currentX += incrimentX;
    //    }
    //    currentX = Screen.width / 2;
    //    for (int i = 0; i < accuracyMeasurementsIncriments; i++)
    //    {
    //        accuracy += DrawRaycast(currentX, currentY);
    //        currentY += incrimentY;
    //    }
    //    //accuracy *= currentWeapon.Accuracy / 100f;
    //}

    //private int DrawRaycast(float xScreenPos, float yScreenPos)
    //{
    //    RaycastHit[] hits;
    //    DrawRaycast(xScreenPos, yScreenPos, out hits);
    //    for (int i = 0; i < hits.Length; i++)
    //    {
    //        if (hits[i].collider.gameObject.GetComponent<Unit>())
    //        {
    //            return 1;
    //        }
    //    }
    //    return 0;
    //}

    //private void DrawRaycast(float xScreenPos, float yScreenPos, out RaycastHit [] hits)
    //{
    //    Ray ray = new Ray(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0f)), Camera.main.transform.forward);
    //    hits = Physics.RaycastAll(ray, range, layerMask: LayerMask.NameToLayer("Default"), queryTriggerInteraction: QueryTriggerInteraction.Ignore);
    //}

    //public void Shoot()
    //{
    //    for (int j = 0; j < currentWeapon.ShotCount; j++)
    //    {
    //        RaycastHit[] hits;
    //        DrawRaycast(UnityEngine.Random.Range(referenceManager.HitBoxImage.rectTransform.rect.xMin, referenceManager.HitBoxImage.rectTransform.rect.xMax),
    //            UnityEngine.Random.Range(referenceManager.HitBoxImage.rectTransform.rect.yMin, referenceManager.HitBoxImage.rectTransform.rect.yMax), out hits);
    //        for (int i = 0; i < hits.Length; i++)
    //        {
    //            if (hits[i].collider.gameObject.GetComponent<Unit>())
    //            {
    //                //random in weapon accuracy for hit calculation
    //                if (UnityEngine.Random.Range(0, 100) <= currentWeapon.Accuracy)
    //                    {
    //                        hits[i].collider.gameObject.GetComponent<IDamagable>().TakeDamage(currentWeapon.Damage);
    //                    }
    //            }
    //        }
    //    }
        
    //}
    #endregion

    public void TestShooting(float accMod)
    {
        RoundManager RM = FindObjectOfType<RoundManager>();

        RM.AddNotificationToFeed(unit.characterSheet.UnitStat_Name + " takes a shot!");

        currentWeapon = unit.equippedWeapon;

        unit.CalculateWeaponStats();

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

        //float Acc_W_Mod = 1 - ((unit.Calculated_WeaponAccuracy * AccMod) / 1000);
        float Acc_W_Mod = ((unit.Calculated_WeaponAccuracy * AccMod) / 1000) + .9f;
        #endregion 

        while (BurstsFired < unit.equippedWeapon.BurstCount)
        {
            ShotsFired = 0;

            while (ShotsFired < unit.equippedWeapon.ShotCount)
            {
                #region Shooting Code Block
                float randomXVector = UnityEngine.Random.Range((1f - Acc_W_Mod), ((1f - Acc_W_Mod) * -1));
                float randomYVector = UnityEngine.Random.Range((1f - Acc_W_Mod), ((1f - Acc_W_Mod) * -1));
                float randomZVector = UnityEngine.Random.Range((1f - Acc_W_Mod), ((1f - Acc_W_Mod) * -1));

                DirectionToFire =
                    unit.AimingNode.transform.forward + new Vector3(randomXVector, randomYVector, randomZVector);

                RaycastHit hit;

                if (Physics.Raycast(unit.AimingNode.transform.position, DirectionToFire, out hit, unit.equippedWeapon.Range))
                {
                    objectToBeDamaged = hit.collider.gameObject.GetComponent<IDamagable>();

                    if (objectToBeDamaged != unit.GetComponent<IDamagable>())
                    {
                        if (objectToBeDamaged != null)
                        {
                            objectToBeDamaged.TakeDamage(unit.equippedWeapon.Damage, unit.equippedWeapon.damageType, unit.characterSheet.UnitStat_Name);
                        }

                        GameObject dmgCube = Instantiate(DamageCube, hit.point, new Quaternion(0, 0, 0, 0));

                        DamageCube dmgCubeScript = dmgCube.GetComponent<DamageCube>();

                        dmgCubeScript.SetOrigin(unit.AimingNode.transform.position);
                    }
                }

                #endregion

                ShotsFired++;

                if (unit.equippedWeapon.fireMode == Weapon_Master.FireModes.SingleShot)
                {
                    yield return new WaitForSeconds(unit.equippedWeapon.FireRate);
                }
            }

            BurstsFired++;

            if (unit.equippedWeapon.fireMode == Weapon_Master.FireModes.SpreadShot)
            {
                yield return new WaitForSeconds(unit.equippedWeapon.FireRate);
            }

        }

        isFiring = false;
    }
}