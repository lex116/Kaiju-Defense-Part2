using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    Weapon_Master currentWeapon = null;

    public Unit_Master unit;

    public int ShotsFired = 0;
    public int BurstsFired = 0;

    public bool isFiring;

    public GameObject DamageCube;
    public GameObject DamageSphere;

    internal AudioSource audioSource;
    float audioSource_Pitch_Min = 0.75f;
    float audioSource_Pitch_Max = 1.25f;

    LayerMask rayMask = ~((1 << 14) | (1 << 2));

    public void Start()
    {
        audioSource = this.gameObject.GetComponent<AudioSource>();
    }

    public Vector3 CalculateAccuracy(float accMod)
    {
        float Accuracy_WithModifier = unit.Calculated_WeaponAccuracy * accMod;
        float Accuracy_WithHighRoll = 99f;
        float Accuracy_ToUse = 0;
        float AccRoll = 0;

        AccRoll = UnityEngine.Random.Range(0, 99);

        if (Accuracy_WithModifier > AccRoll)
        {
            Accuracy_ToUse = Accuracy_WithHighRoll;
        }
        else
        {
            Accuracy_ToUse = Accuracy_WithModifier;
        }

        float Accuracy_Translated = ((Accuracy_ToUse) / 1000) + .9f;

        float randomXVector = UnityEngine.Random.Range((1f - Accuracy_Translated), ((1f - Accuracy_Translated) * -1));
        float randomYVector = UnityEngine.Random.Range((1f - Accuracy_Translated), ((1f - Accuracy_Translated) * -1));
        float randomZVector = UnityEngine.Random.Range((1f - Accuracy_Translated), ((1f - Accuracy_Translated) * -1));

        return unit.AimingNode.transform.forward + new Vector3(randomXVector, randomYVector, randomZVector);
    }

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
        IDamagable objectToBeDamaged = null;
        Vector3 DirectionToFire;
        #endregion 

        while (BurstsFired < unit.equippedWeapon.BurstCount)
        {
            ShotsFired = 0;

            if (unit.equippedWeapon.fireMode == Weapon_Master.FireModes.SpreadShot)
                PlayClip_FiringSound();

            while (ShotsFired < unit.equippedWeapon.ShotCount)
            {
                if (unit.isDead == false)
                {
                    #region Shooting Code Block

                    DirectionToFire = CalculateAccuracy(AccMod);

                    RaycastHit objectToBeHit;

                    if (unit.equippedWeapon.fireMode == Weapon_Master.FireModes.SingleShot || unit.equippedWeapon.fireMode == Weapon_Master.FireModes.AoeShot)
                        PlayClip_FiringSound();

                    #region Firing the weapon
                    if (Physics.Raycast(unit.AimingNode.transform.position, DirectionToFire, out objectToBeHit, unit.equippedWeapon.Range, rayMask))
                    {
                        if (unit.equippedWeapon.fireMode == Weapon_Master.FireModes.SingleShot || unit.equippedWeapon.fireMode == Weapon_Master.FireModes.SpreadShot)
                        {
                            SingleTargetEffect(objectToBeDamaged, objectToBeHit);
                        }

                        if (unit.equippedWeapon.fireMode == Weapon_Master.FireModes.AoeShot)
                        {
                            AoeEffect(objectToBeDamaged, objectToBeHit);
                        }
                    }
                    #endregion

                    #endregion
                }

                ShotsFired++;

                if (unit.equippedWeapon.fireMode == Weapon_Master.FireModes.SingleShot || unit.equippedWeapon.fireMode == Weapon_Master.FireModes.AoeShot)
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

        PlayClip_ReloadSound();

        if (unit.Current_Unit_Suppression_State == Unit_Master.Unit_Suppression_States.State_Waiting)
        {
            unit.Current_Unit_State = Unit_Master.Unit_States.State_PreparingToAct;

            if (unit.AP == 0)
            {
                unit.ToggleMovingState();
                unit.roundManager.Reticle.sprite = unit.Default_Reticle;
            }
        }
    }

    public void SingleTargetEffect(IDamagable objectToDamage, RaycastHit objectToHit)
    {
        objectToDamage = objectToHit.collider.gameObject.GetComponent<IDamagable>();

        if (objectToDamage != unit.GetComponent<IDamagable>())
        {
            if (objectToDamage != null)
            {
                objectToDamage.TakeDamage(unit.equippedWeapon.Damage, unit.equippedWeapon.damageType, unit.characterSheet.UnitStat_Name);
            }

            GameObject dmgCube = Instantiate(DamageCube, objectToHit.point, new Quaternion(0, 0, 0, 0));

            DamageEffect dmgCubeScript = dmgCube.GetComponent<DamageEffect>();

            dmgCubeScript.SetOrigin(unit.AimingNode.transform.position);
        }
    }

    public void AoeEffect(IDamagable objectToDamage, RaycastHit objectToHit)
    {
        objectToDamage = objectToHit.collider.gameObject.GetComponent<IDamagable>();

        if (objectToDamage != unit.GetComponent<IDamagable>())
        {
            if (objectToDamage != null)
            {
                objectToDamage.TakeDamage(unit.equippedWeapon.Damage, unit.equippedWeapon.damageType, unit.characterSheet.UnitStat_Name);
            }
        }

        Collider[] hitColliders = Physics.OverlapSphere(objectToHit.point, unit.equippedWeapon.EffectRadius / 2);

        foreach (Collider x in hitColliders)
        {
            RaycastHit hit;

            if (Physics.Raycast(objectToHit.point, (x.transform.position - objectToHit.point).normalized, out hit, unit.equippedWeapon.EffectRadius))
            {
                if (hit.collider == x)
                {
                    IDamagable objectToBeDamaged;

                    objectToBeDamaged = x.gameObject.GetComponent<IDamagable>();

                    if (objectToBeDamaged != null && objectToBeDamaged != objectToDamage)
                    {
                        objectToBeDamaged.TakeDamage(unit.equippedWeapon.Damage, unit.equippedWeapon.damageType, unit.characterSheet.UnitStat_Name);
                    }
                }
            }
        }

        GameObject dmgSphere = Instantiate(DamageSphere, objectToHit.point, new Quaternion(0, 0, 0, 0));

        DamageEffect dmgCubeScript = dmgSphere.GetComponent<DamageEffect>();

        dmgCubeScript.SetOrigin(unit.AimingNode.transform.position);
    }

    public void PlayClip_FiringSound()
    {
        if (audioSource.clip != unit.equippedWeapon.Firing_Clip)
            audioSource.clip = unit.equippedWeapon.Firing_Clip;

        audioSource.pitch = UnityEngine.Random.Range(audioSource_Pitch_Min, audioSource_Pitch_Max);
        audioSource.Play();
    }

    public void PlayClip_ReloadSound()
    {
        if (audioSource.clip != unit.equippedWeapon.Reload_Clip)
            audioSource.clip = unit.equippedWeapon.Reload_Clip;

        audioSource.pitch = 1;
        audioSource.Play();
    }
}