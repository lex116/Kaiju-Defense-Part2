using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]

public class Throwing : MonoBehaviour
{
    internal Unit_Master unit;

    public GameObject AreaOfEffectGuide;

    internal LineRenderer lineRenderer;

    public GameObject Projectile;

    internal Transform LaunchTransform;
    internal Vector3 TargetTransform;

    internal float DefaultMaximumVerticalDisplacement = 25;

    internal float ceilingOffset = 1f;
    internal float currentMaximumVerticalDisplacement = 0;
    internal float gravity;

    int resolution = 20;

    internal bool isDrawingLaunchPath;

    internal Vector3[] ArcPointArray;

    internal bool isTargetting;
    public bool CanThrow;

    internal Deployable_Behavior_Master deployable;

    public void Update()
    {
        if (isTargetting)
        {
            CeilingCheck();
            TargettingUpdate();

            if (isDrawingLaunchPath)
            {
                DrawPath();
                lineRenderer.SetPositions(ArcPointArray);
            }
        }
    }

    public void Awake()
    {
        gravity = Physics.gravity.y;
    }

    public void Start()
    {
        lineRenderer.positionCount = resolution + 1;
    }

    public void LaunchProjectile()
    {
        GameObject cloneProjecile = Instantiate(Projectile, LaunchTransform.position, LaunchTransform.rotation);
        Rigidbody tempProjectile = cloneProjecile.GetComponent<Rigidbody>();
        tempProjectile.useGravity = true;
        tempProjectile.velocity = CalculateLaunchData().initalVelocity;
        deployable = cloneProjecile.GetComponent<Deployable_Behavior_Master>();
        deployable.DeployableOwner = unit;
    }

    public LaunchData CalculateLaunchData()
    {
        float displacementY = TargetTransform.y - LaunchTransform.position.y;

        Vector3 displacementXZ = new Vector3(TargetTransform.x - LaunchTransform.position.x, 0, TargetTransform.z - LaunchTransform.position.z);

        float time = (Mathf.Sqrt(-2 * currentMaximumVerticalDisplacement / gravity) + Mathf.Sqrt(2 * (displacementY - currentMaximumVerticalDisplacement) / gravity));

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * currentMaximumVerticalDisplacement);

        Vector3 velocityXZ = displacementXZ / time;

        return new LaunchData(velocityXZ + velocityY, time);
    }

    void DrawPath()
    {
        LaunchData launchData = CalculateLaunchData();
        Vector3 previousDrawPoint = transform.position;

        Vector3[] tempArcArray = new Vector3[resolution + 1];

        for (int i = 0; i < resolution; i++)
        {
            float simulationTime = i / (float)resolution * launchData.timeToTarget;
            Vector3 displacement = launchData.initalVelocity * simulationTime + Vector3.up * gravity * simulationTime * simulationTime / 2f;
            Vector3 drawPoint = LaunchTransform.position + displacement;
            //Debug.DrawLine(previousDrawPoint, drawPoint, Color.red);
            previousDrawPoint = drawPoint;

            tempArcArray[i] = transform.InverseTransformPoint(drawPoint);
        }

        tempArcArray[resolution] = transform.InverseTransformPoint(TargetTransform);

        ArcPointArray = tempArcArray;
    }

    public struct LaunchData
    {
        public readonly Vector3 initalVelocity;
        public readonly float timeToTarget;

        public LaunchData(Vector3 initalVelocity, float timeToTarget)
        {
            this.initalVelocity = initalVelocity;
            this.timeToTarget = timeToTarget;
        }
    }

    public void CeilingCheck()
    {
        RaycastHit hit;

        if (Physics.Raycast(unit.transform.position, unit.transform.up, out hit, currentMaximumVerticalDisplacement))
        {
            if (hit.collider != null)
            {
                currentMaximumVerticalDisplacement = hit.point.y - ceilingOffset;
            }

            else
            {
                currentMaximumVerticalDisplacement = DefaultMaximumVerticalDisplacement;
            }
        }
    }

    public void TargettingUpdate()
    {
        RaycastHit hit;

        ToggleTargettingGraphics(false);

        if (Physics.Raycast(unit.AimingNode.transform.position, unit.AimingNode.transform.forward, out hit, unit.throwRange))
        {
            if (hit.collider.gameObject != null)
            {
                if (!float.IsNaN(CalculateLaunchData().initalVelocity.x) && !float.IsNaN(CalculateLaunchData().initalVelocity.y) && !float.IsNaN(CalculateLaunchData().initalVelocity.z))

                ToggleTargettingGraphics(true);

                TargetTransform = hit.point;
                AreaOfEffectGuide.transform.localScale = new Vector3(unit.equippedEquipment.EffectRadius * 2, unit.equippedEquipment.EffectRadius * 2, unit.equippedEquipment.EffectRadius * 2);
                AreaOfEffectGuide.transform.position = hit.point;
            }
        }
    }

    public void ToggleTargettingGraphics(bool Toggle)
    {
        lineRenderer.enabled = Toggle;
        isDrawingLaunchPath = Toggle;
        CanThrow = Toggle;
        AreaOfEffectGuide.SetActive(Toggle);
    }
}
