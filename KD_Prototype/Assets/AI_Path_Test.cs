using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Path_Test : MonoBehaviour
{
    #region setup
    public bool start = false;
    public bool draw = false;

    public Unit_Master thisUnit;

    public GameObject aimingNodeAI;
    #endregion

    #region movement
    public float movementRange;
    public GameObject movementPoint;
    public int movementNodeSteps;
    public int pCount = 0;
    public Transform target;
    [SerializeField]
    List<Vector3> points = new List<Vector3>();
    [SerializeField]
    List<Vector3> validPoints = new List<Vector3>();

    List<AI_Path_PointClass> pointsToEvaulate = new List<AI_Path_PointClass>();

    public enum navState
    {
        aggressive,
        defensive,
        regrouping,
        searching
    };

    [SerializeField]
    public navState currentNavState;
    #endregion


    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        thisUnit = GetComponent<Unit_Master>();

        if (start == true)
        {
            DrawPoints();
            //ValidatePoints();
            start = false;
        }
        if (draw == true)
        {
            //AimAtTarget();
            //ValidatePoints();
           // draw = false;
        }
	}

    //IEnumerator
    void DrawPoints()
    {
        Vector3 newTrans;
        int intRange = (int)movementRange;

        for (float x = -intRange; x <= intRange; x = x+movementNodeSteps)
        {
            for (float y = -intRange; y <= intRange; y = y+movementNodeSteps)
            {
                for (float z = -intRange; z <= intRange; z = z+movementNodeSteps)
                {
                    float nodeDistantce =
                        Mathf.Abs(x) + Mathf.Abs(y) + Mathf.Abs(z);

                    if (movementRange >= nodeDistantce)
                    {
                        newTrans = 
                        new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z + z);

                        //Instantiate(point, newTrans, transform.rotation);
                        
                        points.Add(newTrans);
                        pCount++;
                        //yield return new WaitForFixedUpdate();
                        //yield return new WaitForSeconds(0f);
                    }
                }
            }
        }
        Debug.Log("done");
    }

    void AimAtTarget()
    {
        //foreach (Vector3 x in points)
        //{
        //    Vector3 forwardToTarget;
        //    forwardToTarget = (target.position - x).normalized;

        //    Debug.DrawLine(x, target.position, Color.blue);
        //}
        //Debug.DrawRay(transform.position, Color.green);
        //transform.LookAt(target);
    }

    void ValidatePoints()
    {
        validPoints.Clear();

        foreach (Vector3 x in points)
        {
            Vector3 directionToCheck;
            directionToCheck = (transform.position - x).normalized;

            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            //if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
            if (Physics.Raycast(x, directionToCheck, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    if (Physics.Raycast(x, Vector3.down, out hit, 1f))
                    {
                        Vector3 directionToCheckReverse;
                        directionToCheckReverse = (x - transform.position).normalized;

                        float distanceToCheck = Vector3.Distance(x, transform.position);

                        RaycastHit hitReverse;
                        // Does the ray intersect any objects excluding the player layer
                        //if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
                        if (Physics.Raycast(transform.position, directionToCheckReverse, out hitReverse, distanceToCheck))
                        {

                        }
                        else
                        {
                            Debug.DrawLine(transform.position, x, Color.blue, 200f);
                            validPoints.Add(x);
                        }
                    }
                }
            }
        }

        points.Clear();
    }

    void EvaluateAllValidPoints()
    {



        foreach (Vector3 x in validPoints)
        {

        }
    }

    void EvaluateSinglePoint(Vector3 x)
    {
        Unit_Master[] allUnits = GameObject.FindObjectsOfType<Unit_Master>();
        List<Unit_Master> allAllies = new List<Unit_Master>();
        List<Unit_Master> allEnemies = new List<Unit_Master>();

        foreach (Unit_Master y in allUnits)
        {
            bool redundantCheck = false;

            if (y.characterSheet.UnitStat_FactionTag == thisUnit.characterSheet.UnitStat_FactionTag)
            { 
                allAllies.Add(y);
            }

            if (y.characterSheet.UnitStat_FactionTag != thisUnit.characterSheet.UnitStat_FactionTag)
            {
                foreach (Transform a in y.detectionNodes)
                {
                    if (redundantCheck == false)
                    {
                        RaycastHit hit;
                        Vector3 forwardToTarget;
                        forwardToTarget = (a.transform.position - aimingNodeAI.transform.position).normalized;

                        if (Physics.Raycast(aimingNodeAI.transform.position, forwardToTarget, out hit, Mathf.Infinity))
                        {
                            if (y == hit.collider.GetComponent<Unit_Master>())
                            {
                                allEnemies.Add(y);
                                redundantCheck = true;
                            }
                        }
                    }
                }  
            }
        }
    }

    void EvaluateSinglePoint_DrawToAll(Vector3 z)
    {

    }
}

