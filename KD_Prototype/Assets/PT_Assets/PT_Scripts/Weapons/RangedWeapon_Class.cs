using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon_Class : MasterWeapon_Class
{
    #region Stats
    //Amount of ammo loaded
    [HideInInspector]
    public int currentAmmo;

    //Amount of ammo that can be loaded at once
    [HideInInspector]
    public int maxAmmoCapacity;

    //Amount of ammo in storage
    [HideInInspector]
    public int reserveAmmo;

    //Amount of ammo that can be in storage
    [HideInInspector]
    public int maxReserveAmmoCapacity;

    //How quickly the weapon reloads
    [HideInInspector]
    public float reloadSpeed;

    [SerializeField]
    AudioClip Clip_Shot;

    [SerializeField]
    AudioClip Clip_Cock;

    [SerializeField]
    AudioClip Clip_Reload;

    [SerializeField]
    AudioClip Clip_Drop;
    #endregion

    #region Functions

    public override void Setup()
    {

    }

    public override void Fire()
    {
        if (currentWeaponState == State.Ready && currentAmmo > 0)
        {
            currentAmmo = currentAmmo - 1;
            //////////////////////////////////////////////////////////////
            if (PrimaryWeaponAudioSource.clip != Clip_Shot)
            {
                PrimaryWeaponAudioSource.clip = Clip_Shot;
            }

            StartCoroutine(ShotSound());
            ///////////////////////////////////////////////////////////////////

            // Play Shot sound ////////////////////////////////////////////////////
            //WeaponAudioSource.clip = Clip_Shot;
            //WeaponAudioSource.Play();
            ///////////////////////////////////////////////////////////////////////

            foreach (GameObject i in projectileOrigins)
            {
                Rigidbody clone =
                 Instantiate(projectile,
                 i.transform.position,
                 i.transform.rotation);

                Projectile_Class projectileClone = clone.GetComponent<Projectile_Class>();

                if (projectileClone != null)
                {
                    projectileClone.ProjectileDamage = projectileClone.ProjectileDamage * DamageModifer;
                }

                Throwable_Class throwableClone = clone.GetComponent<Throwable_Class>();

                if (throwableClone != null)
                {
                    throwableClone.SetLive();
                }

                clone.transform.Rotate
     (Random.Range(-1 * ProjectileSpread, ProjectileSpread),
     Random.Range(-1 * ProjectileSpread, ProjectileSpread),
     0);

                clone.AddForce(clone.transform.forward * projectileSpeed);
            }

            StartCoroutine(FireRateCoolDown());
        }
    } /////

    public override IEnumerator FireRateCoolDown()
    {
        if (currentAmmo == 0)
        {
            //yield return new WaitForSeconds(Clip_Shot.length);
            Reload();
        }

        else
        {
            currentWeaponState = State.CoolingDown;
            yield return new WaitForSeconds(fireRate);
            // Play Cock sound //////////////////////////////////////////////////
            //WeaponAudioSource.clip = Clip_Cock;
            //WeaponAudioSource.Play();
            ////////////////////////////////////////////////////////////////////
            if (currentWeaponState == State.CoolingDown)
            {
                currentWeaponState = State.Ready;
            }
        }
    } /////
    public void Reload()
    {
        //if (reserveAmmo > 0 && currentWeaponState == State.Ready && currentAmmo < maxAmmoCapacity)
        if ((reserveAmmo > 0 && currentAmmo < maxAmmoCapacity) && 
            (currentWeaponState != State.Reloading && currentWeaponState != State.Switching))
        {
            StopAllCoroutines();
            currentWeaponState = State.Reloading;
            SecondaryWeaponAudioSource.Stop(); ///////////////////////////////////////////////
            StartCoroutine(ReloadCoroutine());
        } 
    } /////

    IEnumerator ReloadCoroutine()
    {
        int AmmoToPull = 0;

        // Play Reload sound //////////////////////////////////////////////////
        PrimaryWeaponAudioSource.clip = Clip_Reload;
        PrimaryWeaponAudioSource.Play();
        ///////////////////////////////////////////////////////////////////////

        yield return new WaitForSeconds(reloadSpeed);

        AmmoToPull = maxAmmoCapacity - currentAmmo;

        reserveAmmo = reserveAmmo - AmmoToPull;

        if (reserveAmmo < 0)
        {
            AmmoToPull = (AmmoToPull + reserveAmmo);
            reserveAmmo = 0;
        }

        currentAmmo = currentAmmo + AmmoToPull;

        /////////////////////////////////////////////////////////
        PrimaryWeaponAudioSource.clip = Clip_Shot;
        /////////////////////////////////////////////////////////

        currentWeaponState = State.Ready;
    } //////
    //Pick up this weapon
    public override void Activate (MasterInventory_Class inventory)
    {
        if (inventory.readyToEquip == true)
        {
            MoveToPosition(inventory.RangedWeaponPosition);
            inventory.EquipNewRangedWeapon(this);
            TurnOffInteraction();
        }
    }

    //Add Ammo to this weapon
    public void AddAmmo(int AmmoToAdd)
    {
        reserveAmmo = reserveAmmo + AmmoToAdd;
        if (reserveAmmo > maxReserveAmmoCapacity)
        {
            reserveAmmo = maxReserveAmmoCapacity;
        }
    }

    //////////////////////////////////////////////
    public IEnumerator ShotSound()
    {
        SecondaryWeaponAudioSource.Stop();
        PrimaryWeaponAudioSource.Play();
        yield return new WaitForSeconds(Clip_Shot.length);
        SecondaryWeaponAudioSource.Play();
    }
    //////////////////////////////////////////////

    #endregion
}