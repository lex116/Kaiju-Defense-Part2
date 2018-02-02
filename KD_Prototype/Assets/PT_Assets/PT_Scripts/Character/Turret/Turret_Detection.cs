using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret_Detection : MonoBehaviour
{
    int x = 0;

    //Checks if the Turret's gun  has Line of sight of it's target; returning true or false
    public bool DetectTarget_LOS_Gun(GameObject Turret, GameObject Target, float DetectionRange, int LayerMask)
    {
        RaycastHit hit;
        if (Physics.Raycast(Turret.transform.position, Turret.transform.forward, out hit, DetectionRange, LayerMask))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red, 0.1f);
            if (hit.collider.gameObject == Target)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        else
        {
            return false;
        }
    }

    //(TestScript) Find all enemies in the scene and return as an array
    public GameObject[] FindAllEnemies(string TargetTag)
    {
        GameObject[] Enemies = GameObject.FindGameObjectsWithTag(TargetTag);

        if (x < Enemies.Length)
        {
            return Enemies;
        }

        if (x == Enemies.Length)
        {
            return null;
        }

        else
        {
            return null;
        }
    }

    //(TestScript) Try to draw to every traget and return the ones we can actually hit as a list
    public List<GameObject> FindAllEnemiesLOS(GameObject[] AllEnemies, float DetectionRange, string TargetTag, int LayerMask)
    {
        if (x < AllEnemies.Length)
        {
            List<GameObject> EnemiesLOS = new List<GameObject>();

            foreach (GameObject i in AllEnemies)
            {
                if (i != null)
                {
                    Vector3 targetDir = i.transform.position - transform.position;

                    RaycastHit hit;

                    if (Physics.Raycast(transform.position, targetDir, out hit, DetectionRange, LayerMask))
                    {
                        if (hit.collider.gameObject.tag == TargetTag)
                        {
                            //Debug.DrawLine(transform.position, hit.point, Color.red, 1f);
                            EnemiesLOS.Add(i);
                        }
                    }
                }
            }

            return EnemiesLOS;
        }

        if (x == AllEnemies.Length)
        {
            return null;
        }

        else
        {
            return null;
        }

    }

    //(TestScript) Find which enemy is closest and return it as a gameobject
    public GameObject SelectTarget(List<GameObject> EnemiesLOS, float DetectionRange)
    {
        if (x < EnemiesLOS.Count)
        {
            GameObject EnemyToTarget = null;

            float MinimumDistanceToTarget = Mathf.Infinity;

            foreach (GameObject i in EnemiesLOS)
            {
                if (i != null)
                {
                    float DistanceToTarget = Vector3.Distance(i.transform.position, transform.position);

                    if (DistanceToTarget < DetectionRange && DistanceToTarget < MinimumDistanceToTarget)
                    {
                        MinimumDistanceToTarget = DistanceToTarget;
                        EnemyToTarget = i;
                    }
                }
            }

            return EnemyToTarget;
        }

        if (x == EnemiesLOS.Count)
        {
            return null;
        }

        else
        {
            return null;
        }
    }
}
