using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret_Manager : MonoBehaviour, IInteractable
{
    #region Game Object Components
    //Rotating Turret which has our weapon
    [SerializeField]
    public GameObject turret;

    [SerializeField]
    RangedWeapon_Class EquippedRangedWeapon;

    [SerializeField]
    public Transform GunPosition;
    #endregion

    #region Script Components
    Turret_Behavior behaviorScript;
    Turret_Detection detectionScript;

    //LaserSight Script
    //LaserSightScript laserSightScript;
    #endregion

    #region Stats
    //Speed the turret rotates and tracks
    //float movementSpeed = 1f;
    float movementSpeed = 5f;

    //Range of the Turret's detection
    float detectionRange = 40f;

    //Enemy Tag to target
    string targetTag = "Enemy";

    //Currently targetted enemy
    public GameObject target;

    float DetectAllRate = 2;
    public GameObject[] DetectAllArray;

    //float DetectAllLOSRate = 1;
    float DetectAllLOSRate = 0.1f;
    public List<GameObject> DetectAllLOSList = new List<GameObject>();

    float DetectBestTargetRate = 1f;

    int playerLayer = (1 << 8 | 1 << 10 | 1 << 11 | 1 << 12 | 1 << 2);

    string displayText = "Press E to Trade Weapon w/ SmartBoy";
    //string displayText = "5M4R7-B0Y";

    public string DisplayText
    {
        get
        {
            return displayText;
        }
    }

    #endregion

    #region BehaviorStates
    public enum BehaviorState
    {
        Idle,
        DetectsEnemy
    }

    public BehaviorState currentBehaviorState;
    #endregion

    #region LOSstates
    public enum LosState
    {
        LosFalse,
        LosTrue,
        LosSearching
    }

    public LosState currentLosState;
    #endregion

    void Awake()
    {
        behaviorScript = GetComponent<Turret_Behavior>();
        detectionScript = GetComponent<Turret_Detection>();
        //laserSightScript = GetComponentInChildren<LaserSightScript>();
    }

    void Start()
    {
        EquippedRangedWeapon = GetComponentInChildren<RangedWeapon_Class>();
        EquippedRangedWeapon.TurnOffInteraction();
        StartCoroutine(DetectAllEnemies());
        StartCoroutine(DetectAllLOSEnemies());
        StartCoroutine(DetectBestTarget());
        StartCoroutine(Aim());
        StartCoroutine(ShootAt());
    }

    //Draws the detection range the turret in the inspector
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    #region Combat & Detection

    IEnumerator ShootAt()
    {
        if (target != null)
        {
            int layerMask = ~playerLayer;

            if (detectionScript.DetectTarget_LOS_Gun(turret, target, detectionRange, layerMask))
            {
                //Debug.Log("Shoot!");
                //behaviorScript.Shoot(gunTip, projectile, bulletSpeed);
                behaviorScript.Shoot(EquippedRangedWeapon);
            }
        }

        if (EquippedRangedWeapon.currentAmmo == 0)
        {
            EquippedRangedWeapon.Reload();
            yield return new WaitForSeconds(EquippedRangedWeapon.reloadSpeed);
        }

        //yield return new WaitForFixedUpdate();

        yield return new WaitForSeconds(EquippedRangedWeapon.fireRate);

        StartCoroutine(ShootAt());
    }

    IEnumerator Aim()
    {
        if (target != null)
        {
            behaviorScript.AimAtTarget(turret, target, movementSpeed);

            //laserSightScript.HasTarget = true;
        }

        if (target == null)
        {
            behaviorScript.ResetOrientation(turret, movementSpeed);

            //laserSightScript.HasTarget = false;
        }

        yield return new WaitForFixedUpdate();

        StartCoroutine(Aim());
    }

    IEnumerator DetectAllEnemies()
    {
        DetectAllArray = null;

        DetectAllArray = detectionScript.FindAllEnemies(targetTag);

        yield return new WaitForSeconds(DetectAllRate);

        StartCoroutine(DetectAllEnemies());
    }

    IEnumerator DetectAllLOSEnemies()
    {
        int layerMask = ~playerLayer;

        if (DetectAllArray != null)
        {
            DetectAllLOSList = null;

            DetectAllLOSList = detectionScript.FindAllEnemiesLOS(DetectAllArray, detectionRange, targetTag, layerMask);
        }

        yield return new WaitForSeconds(DetectAllLOSRate);

        StartCoroutine(DetectAllLOSEnemies());
    }

    IEnumerator DetectBestTarget()
    {
        if (DetectAllLOSList != null)
        {
            target = detectionScript.SelectTarget(DetectAllLOSList, detectionRange);
        }

        yield return new WaitForSeconds(DetectBestTargetRate);

        StartCoroutine(DetectBestTarget());
    }

    #endregion

    #region Activation & inventory management

    public void Activate(MasterInventory_Class inventory)
    {
        if (inventory.EquippedRangedWeaponScipt != null && inventory.readyToEquip == true)
        {
            SwapWeapon(inventory);
        }
    }

    public void SwapWeapon(MasterInventory_Class inventory)
    {
        RangedWeapon_Class WeaponToGive = EquippedRangedWeapon;
        RangedWeapon_Class WeaponToTake = inventory.EquippedRangedWeaponScipt;

        int x = 0;

        foreach (RangedWeapon_Class w in inventory.RangedWeapons)
        {
            if (w != null)
            {
                if (WeaponToGive.weaponName == w.weaponName)
                {
                    x++;
                }
            }
        }

        if (x > 0)
        {
            Debug.Log("hit case 1");
            //Do nothing
            //Make this destroy the weapon we dont want anymore later.
        }

        else
        {
            Debug.Log("hit case 2");


            //give old weapon to the player
            //move to position
            //equip new weapon
            //move to posiiton
            inventory.DropRangedWeapon(WeaponToTake);

            WeaponToGive.MoveToPosition(inventory.RangedWeaponPosition);
            inventory.EquipNewRangedWeapon(WeaponToGive);

            WeaponToTake.gameObject.SetActive(true);

            WeaponToTake.MoveToPosition(GunPosition);
            Debug.Log("Moved Weapon to correct spot");

            EquippedRangedWeapon = WeaponToTake;
            Debug.Log("Replaced old weapon");

            EquippedRangedWeapon.TurnOffInteraction();
        }
    }

    #endregion

    //temp for demo Aim assist
    void FixedUpdate()
    {
        if (EquippedRangedWeapon != null)
        {
            RaycastHit hit;

            if (Physics.Raycast(turret.transform.position, turret.transform.forward, out hit, Mathf.Infinity))
            {
                //Debug.DrawLine(uppderbody.transform.position, hit.point, Color.cyan, 0.1f);



                foreach (GameObject x in EquippedRangedWeapon.projectileOrigins)
                {
                    x.transform.LookAt(hit.point);
                    //Debug.DrawLine(x.transform.position, hit.point, Color.magenta, 0.1f);
                }
            }
        }
    }
}


