using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterInventory_Class : MonoBehaviour
{
    //temp for demo
    public bool readyToEquip = true;

    //temp
    [SerializeField]
    Transform upperbody;

    public string AmmoCounterString;

    public Health healthScript;
    public bool isBeingDestroyed;

    #region Weapon Fields
    //The number of ranged weapons the manager can hold
    public int MaximumNumberOfRangedWeapons;

    //The index number of the currently selected weapon
    public int EquippedRangedWeaponIndex = 0;

    //The script of the weapon we have equipped
    public RangedWeapon_Class EquippedRangedWeaponScipt;

    //List of all weapons equipped
    public List<RangedWeapon_Class> RangedWeapons = 
        new List<RangedWeapon_Class>();

    //Transform where Ranged Weapons move to when equipped
    [SerializeField]
    public Transform RangedWeaponPosition;

    //How much force we throw dropped weapons with
    //300 for now
    int WeaponDropForce = 300;

    bool OK_ToSwitch = true;
    #endregion

    #region Throwable Fields
    //The script of the throwable weapon we have equipped
    public Throwable_Class EquippedThrowableScript;

    //Transform where Throwables move to when equipped
    [SerializeField]
    public Transform ThrowablePosition;

    //How much force we throw Throwables with
    //7 for now
    int ThrowableForce = 7;

    //How much force we throw dropped Throwables with
    //7 for now
    int ThrowableDropForce = 3;
    #endregion

    //Position Objects we want to drop move to
    [SerializeField]
    Transform ItemDropPosition;

    void Awake()
    {
        SetUp();

        healthScript = GetComponent<Health>();

        for (int i = RangedWeapons.Count; i < MaximumNumberOfRangedWeapons; i++)
        {
            RangedWeapons.Add(null);
        }

        SelectWeaponToEquip();

        //temp for demo
        readyToEquip = true;
    }

    public virtual void SetUp()
    {

    }

    void Start()
    {

    }

    void Update()
    {
        HUDUpdateFunctions();

        if (healthScript.isDestroyed && isBeingDestroyed == false)
        {
            isBeingDestroyed = true;
            DeathEffect();
        }
    }

    public virtual void DeathEffect()
    {

    }

    #region HUD Functions
    //All HUD functions we want to perform once a frame
    public virtual void HUDUpdateFunctions()
    {
        HUDAmmoUpdate();
    }

    //Debug to tell us the status of the equipped weapon
    public virtual void HUDAmmoUpdate()
    {
        if (EquippedRangedWeaponScipt != null)
        {
            AmmoCounterString =
                (EquippedRangedWeaponScipt.currentAmmo + " / " 
                + EquippedRangedWeaponScipt.reserveAmmo
                + "\n" + EquippedRangedWeaponScipt.currentWeaponState);
        }
        else
        {
            AmmoCounterString = "";
        }
    }
    #endregion

    #region Ranged Weapons & Throwables Methods

    //Changes what weapon we have equipped
    public void ChangeWeapon()
    {
        int WeaponCount = 0;
        foreach (RangedWeapon_Class i in RangedWeapons)
        {
            if (i != null)
            {
                WeaponCount++;
            }
        }

        if (OK_ToSwitch == true && WeaponCount == MaximumNumberOfRangedWeapons)
        {
            //RangedWeaponPosition.gameObject.transform.rotation = new Quaternion(45, 0, 0, 0);
            if (EquippedRangedWeaponScipt != null)
            {
                EquippedRangedWeaponScipt.currentWeaponState = MasterWeapon_Class.State.Switching;
                StartCoroutine(ChangeWeaponRoutine());
            }
            
        }
    }

    IEnumerator ChangeWeaponRoutine()
    {
        OK_ToSwitch = false;

        yield return new WaitForSeconds(1f);

        int lastWeaponIndex = EquippedRangedWeaponIndex;

        if (EquippedRangedWeaponIndex >= RangedWeapons.Count - 1)
        {
            EquippedRangedWeaponIndex = -1;
        }

        EquippedRangedWeaponIndex++;

        if (lastWeaponIndex != EquippedRangedWeaponIndex)
        {
            SelectWeaponToEquip();
        }

        OK_ToSwitch = true;
    }

    //Activates the weapon we want and deactivates the rest
    public void SelectWeaponToEquip()
    {
        EquippedRangedWeaponScipt = null;

        int i = 0;
        foreach (RangedWeapon_Class weapon in RangedWeapons)
        {
            if (weapon != null)
            {
                weapon.TurnOffInteraction();

                if (i == EquippedRangedWeaponIndex)
                {
                    weapon.gameObject.SetActive(true);
                    EquippedRangedWeaponScipt = weapon;
                    EquippedRangedWeaponScipt.Refresh();
                }
                else
                {
                    weapon.gameObject.SetActive(false);
                }
            }

            i++;
        }

        if (RangedWeapons[EquippedRangedWeaponIndex] == null)
        {
            EquippedRangedWeaponScipt = null;
        }
    }

    //Allows a new weapon to enter the inventory, if the inventory is full, a weapon is dropped
    public void EquipNewRangedWeapon(RangedWeapon_Class WeaponToPickUp)
    {
        readyToEquip = false;    

        int WeaponCount = 0;
        RangedWeapon_Class DuplicateWeaponAlreadyHeld = null;
        int WeaponSlotToFill = 0;

        foreach (RangedWeapon_Class i in RangedWeapons)
        {
            if (i != null && i.weaponName == WeaponToPickUp.weaponName)
            {
                DuplicateWeaponAlreadyHeld = i;
            }
        }

        if (DuplicateWeaponAlreadyHeld != null && DuplicateWeaponAlreadyHeld != WeaponToPickUp)
        {
            DuplicateWeaponAlreadyHeld.AddAmmo(WeaponToPickUp.currentAmmo + WeaponToPickUp.reserveAmmo);
            Destroy(WeaponToPickUp.gameObject);

            StartCoroutine(EquipCoolDown());
        }

        else
        {
            foreach (RangedWeapon_Class i in RangedWeapons)
            {
                if (i != null)
                {
                    WeaponCount++;
                }
            }

            if (WeaponCount == RangedWeapons.Count)
            {
                DropRangedWeapon(EquippedRangedWeaponScipt);
            }

            foreach (RangedWeapon_Class weapon in RangedWeapons)
            {
                if (weapon == null)
                {

                    WeaponSlotToFill = RangedWeapons.IndexOf(weapon);
                }
            }

            //WeaponToPickUp.MoveToPosition(RangedWeaponPosition);

            RangedWeapons[WeaponSlotToFill] = WeaponToPickUp;

            EquippedRangedWeaponScipt = WeaponToPickUp;

            //EquippedRangedWeaponScipt.transform.localScale = new Vector3 (1,1,1);

            //EquippedRangedWeaponIndex = RangedWeapons.IndexOf(EquippedRangedWeaponScipt);

            SelectWeaponToEquip();

            StartCoroutine(EquipCoolDown());
        }
    }

    //temp for demo
    IEnumerator EquipCoolDown()
    {
        yield return new WaitForSeconds(0.1f);
        readyToEquip = true;
    }

    //Activates a weapon and throws it to the ground
    public void DropRangedWeapon(RangedWeapon_Class rangedWeaponToDrop)
    {
        //RangedWeapons[EquippedRangedWeaponIndex].Refresh();
        if (RangedWeapons[EquippedRangedWeaponIndex] != null)
        {
            RangedWeapons[EquippedRangedWeaponIndex] = null;
        }
        rangedWeaponToDrop.transform.position = ItemDropPosition.position;
        rangedWeaponToDrop.transform.rotation = ItemDropPosition.rotation;
        rangedWeaponToDrop.transform.parent = null;
        Rigidbody tempRB = rangedWeaponToDrop.gameObject.GetComponent<Rigidbody>();
        if (tempRB == null)
        {
            rangedWeaponToDrop.gameObject.AddComponent<Rigidbody>();
        }
        Rigidbody weaponToDrop = rangedWeaponToDrop.GetComponent<Rigidbody>();
        weaponToDrop.AddForce(weaponToDrop.transform.forward * WeaponDropForce);
        if (rangedWeaponToDrop.currentAmmo == 0 && rangedWeaponToDrop.reserveAmmo == 0)
        {
            Destroy(rangedWeaponToDrop.gameObject);
        }
        if (rangedWeaponToDrop != null)
        {
            rangedWeaponToDrop.TurnOnInteraction();
        }
    }

    //Allows a new throwable to enter the intventory, if the inventory is full, a throwable is dropped
    public void EquipNewThrowable(Throwable_Class ThrowableToPickup)
    {
        if (EquippedThrowableScript != null)
        {
            DropThrowable(EquippedThrowableScript);
        }

        ThrowableToPickup.TurnOffInteraction();
        EquippedThrowableScript = ThrowableToPickup;

        //EquippedRangedWeaponScipt.transform.localScale = new Vector3(1, 1, 1);

        StartCoroutine(EquipCoolDown());
    }

    //Throws the equipped throwable and arms it
    public void ThrowThrowable()
    {
        EquippedThrowableScript.Release(ThrowableForce, GetComponent<Collider>());
        EquippedThrowableScript.SetLive();
        EquippedThrowableScript = null;
    }

    //Throws the equipped throwable to the ground unarmed
    public void DropThrowable(Throwable_Class throwableToDrop)
    {
        throwableToDrop.transform.position = ItemDropPosition.position;
        throwableToDrop.transform.rotation = ItemDropPosition.rotation;
        throwableToDrop.Release(ThrowableDropForce, GetComponent<Collider>());
        throwableToDrop.TurnOnInteraction();
        throwableToDrop = null;
    }

    #endregion

    public void DropEverything()
    {
        foreach (RangedWeapon_Class x in RangedWeapons)
        {
            if (x != null)
            {
                //RangedWeapons[EquippedRangedWeaponIndex].Refresh();
                //RangedWeapons[EquippedRangedWeaponIndex] = null;
                x.gameObject.SetActive(true);
                x.transform.position = ItemDropPosition.position;
                x.transform.rotation = ItemDropPosition.rotation;
                x.transform.parent = null;
                x.gameObject.AddComponent<Rigidbody>();
                Rigidbody weaponToDrop = x.GetComponent<Rigidbody>();
                weaponToDrop.AddForce(weaponToDrop.transform.forward * WeaponDropForce);
                if (x.currentAmmo == 0 && x.reserveAmmo == 0)
                {
                    Destroy(x.gameObject);
                }
                if (x != null)
                {
                    x.TurnOnInteraction();
                }
            }
        }

        for (int i = 0; i < RangedWeapons.Count; i++)
        {
            RangedWeapons[i] = null;
        }

        if (EquippedRangedWeaponScipt != null)
        {
            DropRangedWeapon(EquippedRangedWeaponScipt);
        }

        EquippedRangedWeaponScipt = null;

        EquippedRangedWeaponIndex = 0;

        if (EquippedThrowableScript != null)
        {
            DropThrowable(EquippedThrowableScript);
            EquippedThrowableScript = null;
        }
    }

    //temp for demo Aim assist
    void FixedUpdate()
    {
        if (EquippedRangedWeaponScipt != null)
        {
            RaycastHit hit;

            if (Physics.Raycast(upperbody.transform.position, upperbody.transform.forward, out hit, Mathf.Infinity))
            {
                //Debug.DrawLine(uppderbody.transform.position, hit.point, Color.cyan, 0.1f);

                

                foreach (GameObject x in EquippedRangedWeaponScipt.projectileOrigins)
                {
                    x.transform.LookAt(hit.point);
                    //Debug.DrawLine(x.transform.position, hit.point, Color.magenta, 0.1f);
                }
            }
        }
    }
}
