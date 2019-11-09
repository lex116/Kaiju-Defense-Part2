using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Path_PointClass : MonoBehaviour
{
    public int numberOfEnemiesInLOS = 0;
    public int numberOfAlliesInLOS = 0;
    public int numberOfExposedDetectionNodes = 0;
    public float distanceToTarget = 0;
    public bool targetInRange = false;

    public AI_Path_PointClass(int nOfEnemiesInLOS, int nOfAlliesInLOS, 
        int nOfExposedDetectionNodes, float distToTarget, bool tInRange)
    {
        numberOfEnemiesInLOS = nOfEnemiesInLOS;
        numberOfAlliesInLOS = nOfAlliesInLOS;
        numberOfExposedDetectionNodes = nOfExposedDetectionNodes;
        distanceToTarget = distToTarget;
        targetInRange = tInRange;
    }
}
