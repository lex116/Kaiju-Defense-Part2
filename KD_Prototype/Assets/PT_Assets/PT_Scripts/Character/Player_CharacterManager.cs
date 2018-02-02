using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_CharacterManager : MonoBehaviour
{
    Health HealthScript;
    Player_Inventory Player_InventoryScript;

    public void Setup()
    {
        HealthScript = GetComponent<Health>();
        Player_InventoryScript = GetComponent<Player_Inventory>();
    }

    // Use this for initialization
    void Awake ()
    {
        Setup();
	}

    void Update()
    {
        InputHandler();
    }

    private void InputHandler()
    {
        if (Player_InventoryScript.EquippedRangedWeaponScipt != null)
        {
            if (Input.GetMouseButton(0))
            {
                Player_InventoryScript.EquippedRangedWeaponScipt.Fire();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Player_InventoryScript.EquippedRangedWeaponScipt.Reload();
            }  
        }

        if (Player_InventoryScript.EquippedThrowableScript != null)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Player_InventoryScript.ThrowThrowable();
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Player_InventoryScript.ChangeWeapon();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Player_InventoryScript.InteractableToActivate != null)
            {
                Player_InventoryScript.InteractableToActivate.Activate(Player_InventoryScript);
            }
        }

        if (Input.GetKey(KeyCode.Q))
        {
            Player_InventoryScript.PlaceArmor();
        }

    }
}
