using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Inventory : MasterInventory_Class
{
    #region Fields

    #region HUD
    [SerializeField]
    Text UIDisplayText;

    [SerializeField]
    Text AmmoCounter;

    [SerializeField]
    Text ScrapCounter;

    [SerializeField]
    Text HealthDisplay;
    #endregion

    #region Interaction Fields
    float InteractiveRange = 3;

    public IInteractable InteractableToActivate;

    public GameObject cameraToDrawFrom;

    Vector3 endPoint;

    bool isNotReadyToInteract;
    float InteractionCooldown;
    #endregion

    #region Armor Fields

    //This is for later
    public List<GameObject> ArmorPrefabs = new List<GameObject>();

    GameObject ArmorToClone;

    public List<int> ArmorStoredType = new List<int>();
    public List<Vector3> ArmorStoredScale = new List<Vector3>();

    public int ScrapCount;

    bool readyToPlaceArmor = true;
    #endregion

    #endregion

    #region Functions

    //TEMP
    float tempHealthForDemo = 500;
    void Start()
    {
        healthScript.HitPoints = tempHealthForDemo;
    }

    public override void SetUp()
    {
        MaximumNumberOfRangedWeapons = 2;
    }

    public override void DeathEffect()
    {
        Rigidbody temprb = GetComponent<Rigidbody>();
        temprb.constraints = RigidbodyConstraints.None;
        DropEverything();
        //LoseScreen();
        losecan.SetActive(true);
        StartCoroutine(DeSpawn());
    }

    IEnumerator DeSpawn()
    {
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
    }

    [SerializeField]
    public Transform RespawnPoint;

    //Dont need this anymore
    public void Respawn()
    {
        //tempRespawnToggle = false;
        //healthScript.HitPoints = 10;
        healthScript.isDestroyed = false;
        healthScript.HitPoints = tempHealthForDemo;
        isBeingDestroyed = false;
        Rigidbody tempRb = GetComponent<Rigidbody>();
        tempRb.constraints = RigidbodyConstraints.FreezeRotation;
        readyToEquip = true;

        //turn off respawn canvas
        transform.position = RespawnPoint.transform.position;
        transform.rotation = RespawnPoint.transform.rotation;
    }

    #region HUD Methods
    //All HUD functions we want to perform once a frame
    public override void HUDUpdateFunctions()
    {
        HUDAmmoUpdate();
        HUDScrapUpdate();
        HUDInteractionUpdate();
        HUDHealthUpdate();
    }

    //HUD update that displays our ammo
    public override void HUDAmmoUpdate()
    {
        if (EquippedRangedWeaponScipt != null)
        {
            AmmoCounter.text =
                (EquippedRangedWeaponScipt.currentAmmo + " / "
                + EquippedRangedWeaponScipt.reserveAmmo
                + "\n" + EquippedRangedWeaponScipt.currentWeaponState);
        }
        else
        {
            AmmoCounter.text = "\nNothing Equipped";
        }
    }

    //HUD update that displays our scrap total
    void HUDScrapUpdate()
    {
        ScrapCounter.text = ScrapCount + " Scrap";
    }

    //HUD update that displays the interactable we can interact with
    void HUDInteractionUpdate()
    {
        CheckForInteractables();
        InteractableText();
    }

    //HUD update that displays the players remaining Hitpoints
    void HUDHealthUpdate()
    {
        //temp
        HealthDisplay.text = "HP: " + ((healthScript.HitPoints / tempHealthForDemo) * 100) + "%";
        //HealthDisplay.text = "HP: " + ((healthScript.HitPoints / 100) * 100) + "%";
    }
    #endregion

    #region Interaction Methods
    //Updates our tooltip text with the text displayed by the interactable
    private void InteractableText()
    {
        if (InteractableToActivate == null)
        {
            UIDisplayText.gameObject.SetActive(false);
        }
        else
        {
            UIDisplayText.text = InteractableToActivate.DisplayText;
            UIDisplayText.gameObject.SetActive(true);
        }
    }

    //Checks if we are drawing to any interactables
    private void CheckForInteractables()
    {
        endPoint = (cameraToDrawFrom.transform.forward * InteractiveRange) + cameraToDrawFrom.transform.position;

        RaycastHit raycastHit;
        if (Physics.Raycast(cameraToDrawFrom.transform.position, cameraToDrawFrom.transform.forward,
            out raycastHit, InteractiveRange) && raycastHit.transform.gameObject.layer == 12)
        {
            InteractableToActivate = raycastHit.transform.gameObject.GetComponent<IInteractable>();
        }
        else
        {
            InteractableToActivate = null;
        }
    }
    #endregion

    #region Armor Interaction
    void OnCollisionEnter(Collision collision)
    {
        //check to see if we can touch armor on ground
        if (collision.gameObject.tag == "Armor")
        {
            ScrapCount++;
            ArmorStoredScale.Add(collision.gameObject.transform.lossyScale);
            Destroy(collision.gameObject);
        }
    }

    //Allow us to access a target object's armor handler and place armor 
    public void PlaceArmor()
    {
        if (ScrapCount > 0 && readyToPlaceArmor)
        {
            StartCoroutine(PlaceArmorRoutine());
        }
    }

    IEnumerator PlaceArmorRoutine()
    {
        readyToPlaceArmor = false;

        yield return new WaitForSeconds(0.25f);

        GameObject AttachTargetGO;

        //Draw forward to try to hit something
        Vector3 endPoint = (cameraToDrawFrom.transform.forward * InteractiveRange) + cameraToDrawFrom.transform.position;
        Debug.DrawLine(cameraToDrawFrom.transform.position, endPoint, Color.red);

        RaycastHit raycastHit;

        //If we hit something begin the process
        if (Physics.Raycast(cameraToDrawFrom.transform.position, cameraToDrawFrom.transform.forward,
        out raycastHit, InteractiveRange))
        {

            //Debug.Log(raycastHit.collider.gameObject.name);
            //Get references to the object we hit with the raycast
            AttachTargetGO = raycastHit.collider.gameObject;
            //AttachTargetRB = AttachTargetGO.GetComponent<Rigidbody>();
        }

        readyToPlaceArmor = true;
    }
    #endregion

    #endregion

    [SerializeField]
    public GameObject wincan;

    [SerializeField]
    public GameObject losecan;
}