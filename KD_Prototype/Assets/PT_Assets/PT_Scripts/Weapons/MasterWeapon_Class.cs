using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterWeapon_Class : MonoBehaviour, IInteractable
{
    #region Fields
    //Text to display when hit by the interaction cast
    public string DisplayText
    {
        get
        {
            return "E to pickup " + weaponName;
        }
    }

    //Name of the weapon
    [HideInInspector]
    public string weaponName;

    //Type of weapon
    public enum Types
    {
        Gun,
        Throwable,
        Melee
    }
    [HideInInspector]
    public Types WeaponType;

    //Faction the weapon belongs to
    public enum Faction
    {
        Player,
        Fire,
        Robot,
        Bandit,
        Shark
    }
    [HideInInspector]
    public Faction FactionTag;

    //Projectile the weapon fires
    [SerializeField]
    public Rigidbody projectile;

    //Amount of damage each projectile does
    [HideInInspector]
    public int DamageModifer;

    //Speed the projectiles are fired from the weapon
    [HideInInspector]
    public float projectileSpeed;

    //How quickly the weapon will fire
    [HideInInspector]
    public float fireRate;

    //How much varience projectiles have in flight
    [HideInInspector]
    public float ProjectileSpread;

    //Where projectiles are emitted from
    [SerializeField]
    public GameObject[] projectileOrigins;

    //What the weapon currently can do
    public enum State
    {
        Ready,
        Reloading,
        CoolingDown,
        Switching
    }
    public State currentWeaponState;

    //This weapon's collider for activation
    public BoxCollider InteractionCollider;

    public AudioSource PrimaryWeaponAudioSource;
    public AudioSource SecondaryWeaponAudioSource;
    #endregion

    #region Functions

    //Perform our SetUp funtion and set our interaction collider 
    void Awake()
    {
        Setup();
        InteractionCollider = GetComponent<BoxCollider>();
    }

    //Each weapon uses this to set it's stats
    public virtual void Setup()
    {

    }

    //Each weapon type overrites this
    public virtual void Fire()
    {

    }

    //Wait time until the weapon can fire again
    public virtual IEnumerator FireRateCoolDown()
    {
        currentWeaponState = State.CoolingDown;
        yield return new WaitForSeconds(fireRate);
        currentWeaponState = State.Ready;
    }

    //This is used to reset the weapon's state when equipped
    public void Refresh()
    {
        currentWeaponState = State.Ready;
    }

    //Each weapon uses this to be picked up
    public virtual void Activate(MasterInventory_Class inventory)
    {

    }

    //Turn off this weapon's interaction Collider
    public virtual void TurnOffInteraction()
    {
        this.gameObject.layer = 2;
        //InteractionCollider.enabled = false;
    }

    //Turn on this weapon's interaction Collider
    public virtual void TurnOnInteraction()
    {
        this.gameObject.layer = 12;
        //InteractionCollider.enabled = true;
    }

    //Remove this weapons Rigidbody body and move it to a model's holder
    public void MoveToPosition(Transform targetTransform)
    {
        Rigidbody rigidbodyToDestroy = GetComponent<Rigidbody>();
        if (rigidbodyToDestroy != null)
        {
            Destroy(rigidbodyToDestroy);
        }
        this.transform.parent = targetTransform;
        this.transform.position = targetTransform.position;
        this.transform.rotation = targetTransform.rotation;
    }

    #endregion
}
