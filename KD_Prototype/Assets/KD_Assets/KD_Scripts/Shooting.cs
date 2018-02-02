using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    internal Weapon [] Weapons;
    internal bool IsAiming;

    Weapon currentWeapon;

    KD_CharacterController charController;
    ReferenceManager referenceManager;

    int damage;
    float range;

	// Update is called once per frame
	void Update () 
    {
        //DetectAccuracy();
	}

    private void DetectAccuracy()
    {
        float accuracy = 0;
        int accuracyMeasurementsIncriments = 50;
        //draw 100 raycasts in the hit zone, of the 100 add 1 to the accuracy %
        float startX = Screen.width / 2 - (referenceManager.HitBoxImage.rectTransform.rect.width / 2);
        float endX = Screen.width / 2 + (referenceManager.HitBoxImage.rectTransform.rect.width / 2);
        float currentX = startX;
        float incrimentX = Mathf.Abs(endX - startX) / accuracyMeasurementsIncriments;

        float startY = Screen.height / 2 - (referenceManager.HitBoxImage.rectTransform.rect.height / 2);
        float endY = Screen.height / 2 + (referenceManager.HitBoxImage.rectTransform.rect.height / 2);
        float currentY = Screen.height / 2;
        float incrimentY = Mathf.Abs(endY - startY) / accuracyMeasurementsIncriments;

        //measure across X axis
        for (int i = 0; i < accuracyMeasurementsIncriments; i++)
        {
            accuracy += DrawRaycast(currentX, currentY);
            currentX += incrimentX;
        }
        currentX = Screen.width / 2;
        for (int i = 0; i < accuracyMeasurementsIncriments; i++)
        {
            accuracy += DrawRaycast(currentX, currentY);
            currentY += incrimentY;
        }
        accuracy *= currentWeapon.Accuracy / 100f;
    }

    private int DrawRaycast(float xScreenPos, float yScreenPos)
    {
        RaycastHit[] hits;
        DrawRaycast(xScreenPos, yScreenPos, out hits);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject.GetComponent<Unit>())
            {
                return 1;
            }
        }
        return 0;
    }

    private void DrawRaycast(float xScreenPos, float yScreenPos, out RaycastHit [] hits)
    {
        Ray ray = new Ray(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0f)), Camera.main.transform.forward);
        hits = Physics.RaycastAll(ray, range, layerMask: LayerMask.NameToLayer("Default"), queryTriggerInteraction: QueryTriggerInteraction.Ignore);
    }

    public void Shoot()
    {
        for (int j = 0; j < currentWeapon.ShotCount; j++)
        {
            RaycastHit[] hits;
            DrawRaycast(UnityEngine.Random.Range(referenceManager.HitBoxImage.rectTransform.rect.xMin, referenceManager.HitBoxImage.rectTransform.rect.xMax),
                UnityEngine.Random.Range(referenceManager.HitBoxImage.rectTransform.rect.yMin, referenceManager.HitBoxImage.rectTransform.rect.yMax), out hits);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.gameObject.GetComponent<Unit>())
                {
                    //random in weapon accuracy for hit calculation
                    if (UnityEngine.Random.Range(0, 100) <= currentWeapon.Accuracy)
                        {
                            hits[i].collider.gameObject.GetComponent<IDamagable>().TakeDamage(currentWeapon.Damage);
                        }
                }
            }
        }
        
    }

    public void TestShooting()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.red, 4f);

    }
}