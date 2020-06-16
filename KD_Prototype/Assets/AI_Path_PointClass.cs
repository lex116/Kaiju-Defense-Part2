using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Path_PointClass : ScriptableObject
{
    public Vector3 worldSpacePostion;
    public float numberOfEnemiesInLOS = 0;
    public float numberOfAlliesInLOS = 0;
    public float numberOfExposedDetectionNodes = 0;
    public float distanceToClosestTarget = 0;
    public float averageDistanceToEnemies = 0;
    public float averageDistanceToAllies = 0;
    public float numberOfEnemiesInRange = 0;
    public Vector3 nearestEnemyPosition;

    public int unappealScore = 0;

    //public AI_Path_PointClass(Vector3 coordinates, int nOfEnemiesInLOS, int nOfAlliesInLOS, 
    //    int nOfExposedDetectionNodes, float distToTarget, bool tInRange)
    //{
    //    numberOfEnemiesInLOS = nOfEnemiesInLOS;
    //    numberOfAlliesInLOS = nOfAlliesInLOS;
    //    numberOfExposedDetectionNodes = nOfExposedDetectionNodes;
    //    distanceToClosestTarget = distToTarget;
    //    targetInRange = tInRange;
    //}
}
