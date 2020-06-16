using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class AI_Path_Test : MonoBehaviour
{
    #region setup
    public bool StartStateMachineBool = false;
    public bool resetStateMachine = false;
    public bool move = false;
    public bool evaluate = false;
    public bool clearance = false;
    public bool shootTheGun = false;
    public bool drawCameraLine = false;

    [SerializeField]
    KD_Global.FactionTag thisFactionTag;
    //public Unit_Master thisUnit;
    #endregion

    #region movement 
    //this is the maximum range to draw movement nodes in a single movement
    //10 seems like the most efficient range to calculate at currently
    //this could be a limitation of the hardware also :^)
    int maxMovementRange = 10; //fixed
    //this is the range to draw movement nodes for the current movement we are making
    [SerializeField]
    int movementRange = 0; //fixed
    //this is the maximum amount of movement the unit gets to make all movements 
    //and how much they should have after making a dash action
    [SerializeField]
    float maxMovementPoints = 10; //fixed
    //this is the amount of movement points the unit currently has to make movements
    [SerializeField]
    public float currentMovementPoints; //fixed
    //this is the amount of time to wait on each movement before it's considered failed
    float movementFailsafeWaitTime = 0;
    //this gets incremented each frame, when it hits the wait time the movement fails
    float movementFailsafeCurrentTime = 0;
    //this is how many seconds per untiy meter to give each movement to fail 
    float movementFailsafeTimeModifier = 1f;
    [SerializeField]
    int currentActionPoints = 2;
    int movementNodeSteps = 1;
    public int vpCount = 0;
    public int pCount = 0;
    
    public AI_Path_PointClass targetPosition;
    List<Vector3> points = new List<Vector3>();
    List<Vector3> validPoints = new List<Vector3>();
 
    public enum ai_BehaviorState
    {
        waitingToPerformAction,
        selectingTargetPosition,
        movingToTargetPosition,
        preparingToShootAtEnemy,
        shootingAtTargetEnemy,
        endingTurn
    };

    [SerializeField]
    public ai_BehaviorState current_ai_BehaviorState;

    public enum ai_PositionalPreferenceState
    {
        aggressive,
        defensive,
        regrouping,
        searching
    };

    [SerializeField]
    public ai_PositionalPreferenceState current_ai_PositionalPreferenceState;

    public CharacterController apt_CharacterController;
    public GameObject apt_AimingNode;
    //public bool trackingTest;
    public float Walkspeed;
    public float AimingNodeHeight;
    public bool hasTargetPosition;

    //test
    //Unit_Master[] allUnits;
    [SerializeField]
    Unit_AI_PathingDummy[] allUnits;
    [SerializeField]
    List<Unit_Master> allAllies = new List<Unit_Master>();
    [SerializeField]
    List<Unit_Master> allEnemies = new List<Unit_Master>();
    public float exampleWeaponRange;

    List<AI_Path_PointClass> pointsToEvaluate = new List<AI_Path_PointClass>();
    public AI_Path_PointClass pointBeingEvaluated;
    AI_Path_PointClass[] EvaluatedPointsArray;

    float targetPositionCheckTolerance = 0.50f;
    [Header("Watch")]
    public float targetPositionVerticalOffsetDifference = 0f;
    #endregion

    #region incline checker
    [Header("Incline Checking")]
    float slope = 0f;
    float numberOfStepsToCheckPerDraw = 100;
    Vector3 lastPosition;
    Vector3 newPosition;
    float maximumSlopeTolerance = 1;
    float drawPositionStart = -1;
    #endregion

    [Header("preference testing fields")]
    public int maxHealth = 100;
    public int currentHealth = 25;
    public float dangerValueHealth = .30f;
    public int maxMorale = 100;
    public int currentMorale = 25;
    public float dangerValueMorale = .30f;
    public int testingValueCount = 0;
    public bool calcTest = false;

    public Transform[] detectionNodesArray;

    public TestingGun testGun;

    public Vector3 startingPosition;
    //public Vector3 endingPosition;

    //Notes
    //just finished moving to a point the next thing to do is to create a "maximum walkable incline" this should make it so that the 
    //pathing points don't claim that the tops of heads and walls are walkable space. should also create something to check if the target
    //route is "Clear" wether it be unusual inclines, corners, windows, gaps, etc
    //walkable incline system not viable, doesnt intelligently account for enough variables. trying additional system
    //next step is to check at each position if there is room for our collider potentially

    void DecideWhereToGo()
    {
        DrawPoints();
        ValidatePoints();
        LocateAllUnits();
        UpdatePositionalPreferenceState();
        EvaluateAllValidPoints();
    }

    void DecideToMakeMovement()
    {
        current_ai_BehaviorState = ai_BehaviorState.selectingTargetPosition;

        if (currentMovementPoints > maxMovementRange)
        {
            movementRange = maxMovementRange;
        }
        else
        {
            movementRange = (int)currentMovementPoints;
        }

        startingPosition = transform.position;
        DecideWhereToGo();

        //this statement just determines whether it's even worth trying to move if the distance
        //covered wouldn't even be more than the distance of a single node 
        //hopefully this doesn't comepletely destroy everything and set back the project months
        float distanceToTargetPosition = Mathf.Abs(Vector3.Distance(startingPosition, targetPosition.worldSpacePostion));
        if (distanceToTargetPosition < movementNodeSteps + targetPositionVerticalOffsetDifference)
        {
            //Debug.Log("hit");
            currentMovementPoints = 0;
            current_ai_BehaviorState = ai_BehaviorState.waitingToPerformAction;
        }
        else
        {
            //Debug.Log("move");
            movementFailsafeWaitTime = distanceToTargetPosition * movementFailsafeTimeModifier;
            movementFailsafeCurrentTime = 0;
            current_ai_BehaviorState = ai_BehaviorState.movingToTargetPosition;
        }
    }

    void Attack()
    {
        testGun.fireWeapon = true;
        current_ai_BehaviorState = ai_BehaviorState.shootingAtTargetEnemy;
    }

    void Dash()
    {

    }

    // Use this for initialization
    void Start ()
    {
        AimingNodeHeight = Vector3.Distance(apt_AimingNode.transform.position, transform.position);
        current_ai_PositionalPreferenceState = ai_PositionalPreferenceState.aggressive;
        current_ai_BehaviorState = ai_BehaviorState.waitingToPerformAction;
    }

	// Update is called once per frame
	void FixedUpdate ()
    {
        //reset the statemachine to simulate taking a turn
        if(resetStateMachine)
        {
            resetStateMachine = false;
            StartStateMachineBool = true;
            current_ai_BehaviorState = ai_BehaviorState.waitingToPerformAction;
            currentMovementPoints = maxMovementPoints;
            currentActionPoints = 2;
        }
         
        //draw a line forward so i know where the test unit is looking
        //Debug.DrawRay(apt_AimingNode.transform.position, apt_AimingNode.transform.forward * exampleWeaponRange, Color.red, 0.1f);

        //changes the positional preference of the unit (at the start of it's turn each round)
        if (calcTest)
        {
            UpdatePositionalPreferenceState();
            calcTest = false;
        }

        //neutral state  
        if (StartStateMachineBool == true && current_ai_BehaviorState == ai_BehaviorState.waitingToPerformAction)
        {
            //Debug.Log("Start");
            //maxMovementRange = maxMovementPoints;
            
            if (currentMovementPoints >= 1) //we have movements points remaining 
            {
                //Debug.Log("we're making a move");
                DecideToMakeMovement();
            }
            else //if movement points are 0 or less
            {
                if (currentActionPoints == 0) //we have no action pts and no mv pts so we should end the turn
                {
                    //end the turn
                    //Debug.Log("end turn");
                    current_ai_BehaviorState = ai_BehaviorState.endingTurn; 
                }
                else // we have ap but no mv so now we should perform an action
                {
                    if (targetPosition.numberOfEnemiesInRange > 0) // we have a target to attack
                    {
                        //make an attack action this needs to be added,
                        currentActionPoints--;
                        //set behavior to attacking
                        current_ai_BehaviorState = ai_BehaviorState.preparingToShootAtEnemy;
                    }
                    else
                    {
                        //perform a dash action,
                        currentActionPoints--;
                        currentMovementPoints = maxMovementPoints;
                        //then go back to making a movement
                        DecideToMakeMovement();
                    }
                }
            }

        } 

        if (current_ai_BehaviorState == ai_BehaviorState.movingToTargetPosition)
        {
            //Debug.Log("moving to target position");
            RotateToFaceTargetPosition(targetPosition.worldSpacePostion);
            MoveToTargetDestination();

            if (checkIfAtTargetPosition())
            {
                //temp
                //Debug.Log("at target position");
                Debug.Log("movement complete");
                UpdateMovementPoints();
                current_ai_BehaviorState = ai_BehaviorState.waitingToPerformAction; 
            }
            else
            {
               if (MovementFailsafeCheck())
                {
                    Debug.Log("movement failed");
                    UpdateMovementPoints();
                    current_ai_BehaviorState = ai_BehaviorState.waitingToPerformAction;
                }
            }
        }

        if (current_ai_BehaviorState == ai_BehaviorState.preparingToShootAtEnemy)
        {
            //Debug.Log("preparing to shoot enemy");
            RotateToFaceTargetPosition(EvaluatedPointsArray[0].nearestEnemyPosition);
            Attack();
        } 

        if (current_ai_BehaviorState == ai_BehaviorState.shootingAtTargetEnemy)
        {
            //Debug.Log("shooting at enemy");
            if (testGun.fireWeapon == false && testGun.isFiring == false)
            {
                current_ai_BehaviorState = ai_BehaviorState.waitingToPerformAction;
            }
        }

        if (drawCameraLine)
        {
            Debug.DrawRay(apt_AimingNode.transform.position, (transform.forward * 20), Color.blue, 0.25f);
        }

        if (current_ai_BehaviorState == ai_BehaviorState.endingTurn)
        {
            //Debug.Log("turn ended");
            current_ai_BehaviorState = ai_BehaviorState.waitingToPerformAction;
            StartStateMachineBool = false;
        }
	}
       
    //creates a list of possible world space positions relative to where we are in world space, range, and frequency of steps
    //positions must be in possible range to be added to list of possible positions
    void DrawPoints()
    {
        points.Clear();
        pCount = 0;
        pointsToEvaluate.Clear();
        allUnits = null;
        allAllies.Clear();
        allEnemies.Clear();
        EvaluatedPointsArray = null;
        move = false;
        hasTargetPosition = false;
        targetPosition = null;
        Vector3 newTrans;

        for (float x = -movementRange; x <= movementRange; x = x+movementNodeSteps)
        {
            for (float y = -movementRange; y <= movementRange; y = y+movementNodeSteps)
            {
                for (float z = -movementRange; z <= movementRange; z = z+movementNodeSteps)
                {
                    float nodeDistantce =
                        Mathf.Abs(x) + Mathf.Abs(y) + Mathf.Abs(z);

                    if (movementRange >= nodeDistantce)
                    {
                        newTrans = 
                        new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z + z);
                        
                        points.Add(newTrans);
                        pCount++;
                    }
                }
            }
        }
    }

    //rotates the camera to look at the target destination, currently does nothing but look cool
    void RotateToFaceTargetPosition(Vector3 positionToLookAt)
    {
         //Rotate the body to face the target
        transform.LookAt(positionToLookAt);

        Vector3 bodyEulerAngles = transform.rotation.eulerAngles;
        bodyEulerAngles.x = 0;
        bodyEulerAngles.z = 0;

        transform.rotation = Quaternion.Euler(bodyEulerAngles);

        //Rotate the camera to face the target
        apt_AimingNode.transform.LookAt(positionToLookAt);

        Vector3 camEulerAngles = apt_AimingNode.transform.rotation.eulerAngles;
        bodyEulerAngles.y = 0;
        bodyEulerAngles.z = 0;

        apt_AimingNode.transform.rotation = Quaternion.Euler(camEulerAngles);
    }
    //using the character controller steps the unit to it's target position
    //NOTE
    //this needs to be changed because it doesn't function the same way as the player character controller currently
    //time.deltaTime isn't in the normal player movement function, this means that values controlling movement speed are
    //very inaccurate.
    //probably should change the player movement to use time.deltaTime, it should be more stable and smooth that way
    //this should probably also later tie into changing movement speed based on character carry weight
    void MoveToTargetDestination()
    {
        apt_CharacterController.SimpleMove(transform.forward * Walkspeed * Time.deltaTime);
    }

    void AimAtTarget() //currently does nothing
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

    //checks all listed points for three things
    //1. can we draw from the origin of the point to the unit (can the point see us)
    //2. can we draw from the unit to the origin of the point (can we see the point)
    //Note: 1b&2b we check both in case the point is inside of a wall or other unit
    //3. can we draw to the ground from half the height of our unit (is there ground)
    void ValidatePoints()
    {
        vpCount = 0;
        validPoints.Clear();
        
        foreach (Vector3 x in points)
        {
            Vector3 directionToCheck;
            directionToCheck = (transform.position - x).normalized;
            // Does the ray intersect any objects excluding the player layer?
            
            if (ValidatePoint_CheckLOS(x, directionToCheck))
            {
                if (ValidatePoint_CheckGrounded(x))
                {
                    if (ValidatePoint_CheckForWalkableIncline(x))
                    {
                        if (ValidatePoint_CheckClearance(x))
                        {
                            vpCount++;
                            //Debug.DrawLine(transform.position, x, Color.blue, 200f);
                            //Instantiate(movementPoint, x, transform.rotation);
                            validPoints.Add(x);
                        }
                    }
                }
            }
        }

        points.Clear();
    }
    bool ValidatePoint_CheckLOS(Vector3 point, Vector3 directionToCheck)
    {
        RaycastHit hit;
        if (Physics.Raycast(point, directionToCheck, out hit, Mathf.Infinity))
        {
            //change this to the be origin of the unit later
            if (hit.collider.gameObject == this.gameObject)
            {
                RaycastHit hitReverse;
                // Does the ray intersect any objects excluding the player layer
                //if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))

                Vector3 directionToCheckReverse;
                directionToCheckReverse = (point - transform.position).normalized;
                float distanceToCheck = Vector3.Distance(point, transform.position);
                if (Physics.Raycast(transform.position, directionToCheckReverse, out hitReverse, distanceToCheck))
                {
                    //we hit something so we shouldn't walk that way
                }
                else
                {
                    return true;
                }
            }
        }

        return false;
    }
    //note this needs to get layer masks to ignore anything that isnt ground
    bool ValidatePoint_CheckGrounded(Vector3 point)
    {
        float verticalOffset = transform.position.y;
        RaycastHit hit;
        if (Physics.Raycast(point, Vector3.down, out hit, verticalOffset))
        {
            return true;
        }
        return false;
    }
    //draws to points in front of the unit to check the slope of each point
    //down 0,-1,0
    //forward 0,0,1
    bool ValidatePoint_CheckForWalkableIncline(Vector3 targetPosition)
    {
        slope = 0f;
        bool aboveSlopeTolerance = false;
        lastPosition = new Vector3(0, 0);
        newPosition = new Vector3(0, 0);
        //Debug.Log("A: " + transform.position);
        //Debug.Log("B: " + targetPosition);
        //get the starting draw position: down
        Vector3 startDirectionDown = new Vector3(0, -1, 0); //C
        //Debug.Log("C: " + startDirectionDown);
        //find the direction to look at to be facing the target position
        Vector3 targetDirection = targetPosition - apt_AimingNode.transform.position; //D = B - A
        //Debug.Log("D = B - A: " + targetDirection);
        //find the difference between straight down and the direction we would face to face the target
        Vector3 directionDifference = targetDirection - startDirectionDown; //E = D - C
        //Debug.Log("E = D - C: " + directionDifference);
        //divide the difference by the number of steps we want to take to reach the position
        //numberOfStepsToCheckPerDraw = F
        //Debug.Log("F: " + numberOfStepsToCheckPerDraw);
        Vector3 vectorToShiftDirectionByPerStep = directionDifference / numberOfStepsToCheckPerDraw; //G = E / F
        //Debug.Log("G = E/F: " + vectorToShiftDirectionByPerStep);
        //set directionToDrawTo to down so that we can add the vector shift incrementally
        Vector3 directionToDrawTo = startDirectionDown;


        //Debug.Log("calculations work");
        for (float i = 0; i < numberOfStepsToCheckPerDraw; i++)
        {
            if (aboveSlopeTolerance == false)
            {
                RaycastHit hit;
                if (Physics.Raycast(apt_AimingNode.transform.position, directionToDrawTo, out hit, Mathf.Infinity))
                {
                    //Debug.DrawLine(apt_AimingNode.transform.position, hit.point, Color.blue, 20f);
                    newPosition = hit.point;

                    if (directionToDrawTo == startDirectionDown)
                    {
                        lastPosition = newPosition;
                    }
                    else
                    {
                        //if (maximumSlopeTolerance < Mathf.Abs((heightOfNewPoint - heightOfLastPoint)))
                        if (maximumSlopeTolerance < ValidatePoint_CheckForWalkableIncline_CalculateSlope(lastPosition, newPosition))
                        {
                            aboveSlopeTolerance = true;
                            return false;
                        }

                        lastPosition = newPosition;
                    }
                }
            }
            directionToDrawTo = directionToDrawTo + vectorToShiftDirectionByPerStep;
        }

        //Debug.Log("for loop works");
        return true;
    }
    //calculates the 3d space slope to check for walkable inclines
    float ValidatePoint_CheckForWalkableIncline_CalculateSlope(Vector3 point_A, Vector3 point_B)
    {
        //1. find the run
        //2. find the rise
        //3. divide the rise by the run
        float run = 0;
        float rise = 0;

        //get run
        float pythagorean_A = 0f;
        float pythagorean_B = 0f;
        float pythagorean_C = 0f;

        pythagorean_A = point_B.x - point_A.x;
        pythagorean_B = point_B.z - point_A.z;

        float sq_A = Mathf.Pow(pythagorean_A, 2);
        float sq_B = Mathf.Pow(pythagorean_B, 2);
        float sq_C = sq_A + sq_B;

        pythagorean_C = Mathf.Sqrt(sq_C);

        run = pythagorean_C;

        //get rise
        rise = point_B.y - point_A.y;

        //get slope
        slope = Mathf.Abs(rise / run);

        return slope;
    }
    //checks for obstructions not centered on the path, like low cover, windows etc
    bool ValidatePoint_CheckClearance(Vector3 position)
    {
        float distanceToTargetPosition = Vector3.Distance(transform.position, position);
        float sphereRadius = apt_CharacterController.radius; //0.5f on a human
        float verticalOffsetBottom = apt_CharacterController.height * .25f; // 0.5f on a human
        float verticalOffsetTop = apt_CharacterController.height * .75f; // 1.5f on human

        Vector3 pos1 = position;
        pos1.y = pos1.y + verticalOffsetBottom;

        Vector3 pos2 = position;
        pos2.y = pos2.y + verticalOffsetTop;

        RaycastHit hit;
        Ray sphereRay1 = new Ray(pos1, transform.forward);
        Ray sphereRay2 = new Ray(pos2, transform.forward);

        if (Physics.SphereCast(sphereRay1, sphereRadius, out hit, distanceToTargetPosition))
        {

        }
        else
        {
            if (Physics.SphereCast(sphereRay2, sphereRadius, out hit, distanceToTargetPosition))
            {

            }
            else
            {
                //Debug.Log("We did it boys");
                //we didnt hit anything either time woot
                return true;
            }
        }

        return false;
    }

    //find all units we can see by LOS, add them to our ally and enemy lists 
    void LocateAllUnits()
    { 
        //test
        //Unit_Master[] allUnits = GameObject.FindObjectsOfType<Unit_Master>();
        allUnits = GameObject.FindObjectsOfType<Unit_AI_PathingDummy>();
        allAllies.Clear();
        allEnemies.Clear();

        //test
        //foreach (Unit_Master y in allUnits)
        foreach (Unit_AI_PathingDummy y in allUnits)
        {
            bool redundantCheck = false;

            //test
            //if (y.characterSheet.UnitStat_FactionTag == thisFactionTag)
            if (y.dummyTag == thisFactionTag)
            {
                allAllies.Add(y);
            }

            //test change this backlater
            if (y.dummyTag != thisFactionTag)
            {
                //update for detection nodes
                //foreach (Transform a in y.detectionNodes)
                //{
                if (redundantCheck == false)
                {

                    Vector3 forwardToTarget;
                    //update for detection nodes
                    //forwardToTarget = (a.transform.position - aimingNodeAI.transform.position).normalized;
                    forwardToTarget = y.transform.position - apt_AimingNode.transform.position;
                    RaycastHit hit;

                    if (Physics.Raycast(apt_AimingNode.transform.position, forwardToTarget, out hit, Mathf.Infinity))
                    {
                        //Debug.DrawLine(apt_AimingNode.transform.position, hit.point, Color.blue, 100f);

                        if (y == hit.collider.GetComponent<Unit_AI_PathingDummy>())
                        {
                            allEnemies.Add(y);
                            redundantCheck = true;
                        }
                    }
                //}
                }
            }
        }
    }
    //get data on every valid point
    void EvaluateAllValidPoints()
    {
        pointsToEvaluate.Clear();
        pointBeingEvaluated = null;

        foreach (Vector3 x in validPoints)
        {
            EvaluateSinglePoint(x);
        }

        OrderPointsByBehavior();
    }
    void EvaluateSinglePoint(Vector3 x)
    {
        pointBeingEvaluated = (AI_Path_PointClass)ScriptableObject.CreateInstance("AI_Path_PointClass");

        pointBeingEvaluated.worldSpacePostion = x;
        pointBeingEvaluated.numberOfEnemiesInLOS = EvaluatePoint_NumberOfEnemiesInLOS(pointBeingEvaluated);
        pointBeingEvaluated.numberOfAlliesInLOS = EvaluatePoint_NumberOfAlliesInLOS(pointBeingEvaluated);
        pointBeingEvaluated.numberOfExposedDetectionNodes = EvaluatePoint_NumberOfExposedDetectionNodes(pointBeingEvaluated);
        pointBeingEvaluated.distanceToClosestTarget = EvaluatePoint_DistanceToClosestTarget(pointBeingEvaluated);
        pointBeingEvaluated.averageDistanceToEnemies = EvaluatePoint_AverageDistanceToEnemies(pointBeingEvaluated);
        pointBeingEvaluated.averageDistanceToAllies = EvaluatePoint_AverageDistanceToAllies(pointBeingEvaluated);
        pointBeingEvaluated.numberOfEnemiesInRange = EvaluatePoint_TargetsInRange(pointBeingEvaluated);

        pointsToEvaluate.Add(pointBeingEvaluated);
    }

    //this needs to be updated to try to hit all the detection nodes of enemies later when not using the dummy
    int EvaluatePoint_NumberOfEnemiesInLOS(AI_Path_PointClass x)
    {
        int count = 0;

        if (allEnemies != null)
        {
            foreach (Unit_AI_PathingDummy y in allEnemies)
            {
                RaycastHit hit;
                Vector3 forwardToTarget;
                forwardToTarget = (y.gameObject.transform.position - x.worldSpacePostion).normalized;

                if (Physics.Raycast(x.worldSpacePostion, forwardToTarget, out hit, Mathf.Infinity))
                {
                    count++;
                }
            }
        }
       
        return count;
    }
    int EvaluatePoint_NumberOfAlliesInLOS(AI_Path_PointClass x)
    {
        int count = 0;

        if (allEnemies != null)
        {
            foreach (Unit_AI_PathingDummy y in allAllies)
            {
                RaycastHit hit;
                Vector3 forwardToTarget;
                forwardToTarget = (y.gameObject.transform.position - x.worldSpacePostion).normalized;

                if (Physics.Raycast(x.worldSpacePostion, forwardToTarget, out hit, Mathf.Infinity))
                {
                    count++;
                }
            }

        }

        return count;
    }
    //Comeback to this one when we have a better way to do it currently 
    //we dont have a way to check the exposure rating of the position easily
    int EvaluatePoint_NumberOfExposedDetectionNodes(AI_Path_PointClass x)
    {
        int count = 0;

        foreach (Unit_AI_PathingDummy y in allEnemies)
        {
            foreach (Transform z in detectionNodesArray)
            {
                RaycastHit hit;
                Vector3 forwardToTarget;
                Vector3 dN_offset = transform.position - z.transform.position;
                Vector3 dN_offsetPos = x.worldSpacePostion + dN_offset;
                forwardToTarget = (y.aimingNode.transform.position - dN_offsetPos).normalized;

                if (Physics.Raycast(dN_offsetPos, forwardToTarget, out hit, Mathf.Infinity))
                {
                    if (hit.collider.gameObject == y.gameObject)
                    {
                        //Debug.DrawLine(dN_offsetPos, hit.point, Color.red, 200f);
                        count++;
                    }
                }
            }
        }

        return count;
    }
    float EvaluatePoint_DistanceToClosestTarget(AI_Path_PointClass x)
    {
        float distanceToY = 0f;
        float shortestDistanceToATarget = 0f;

        if (allEnemies != null)
        {
            foreach (Unit_AI_PathingDummy y in allEnemies)
            {
                distanceToY = Vector3.Distance(y.gameObject.transform.position, x.worldSpacePostion);

                if (shortestDistanceToATarget == 0)
                {
                    shortestDistanceToATarget = distanceToY;
                    x.nearestEnemyPosition = y.gameObject.transform.position;
                }

                else if (distanceToY < shortestDistanceToATarget)
                {
                    //this is for testing
                    shortestDistanceToATarget = distanceToY;
                    x.nearestEnemyPosition = y.gameObject.transform.position;
                }
            }
        }

        return shortestDistanceToATarget;
    }
    float EvaluatePoint_AverageDistanceToEnemies(AI_Path_PointClass x)
    {
        float sumDistances = 0f;
        int countDistances = 0;
        float averageOfDistances = 0f;

        if (allEnemies != null)
        {
            foreach (Unit_AI_PathingDummy y in allEnemies)
            {
                sumDistances = Vector3.Distance(y.gameObject.transform.position, x.worldSpacePostion) + sumDistances;
                countDistances++;
            }
        }

        averageOfDistances = sumDistances / countDistances;
        return averageOfDistances;
    }
    float EvaluatePoint_AverageDistanceToAllies(AI_Path_PointClass x)
    {
        float sumDistances = 0f;
        int countDistances = 0;
        float averageOfDistances = 0f;

        if (allEnemies != null)
        {
            foreach (Unit_AI_PathingDummy y in allAllies)
            {
                sumDistances = Vector3.Distance(y.gameObject.transform.position, x.worldSpacePostion) + sumDistances;
                countDistances++;
            }
        }

        averageOfDistances = sumDistances / countDistances;
        return averageOfDistances;
    }
    int EvaluatePoint_TargetsInRange(AI_Path_PointClass x)
    {
        float distanceToTarget = 0;
        int inRangeCount = 0;

        if (allEnemies != null)
        {
            foreach (Unit_AI_PathingDummy y in allEnemies)
            {
                distanceToTarget = Vector3.Distance(y.gameObject.transform.position, x.worldSpacePostion);

                if (distanceToTarget < exampleWeaponRange)
                {
                    inRangeCount++;
                }
            }
        }

        return inRangeCount++;
    }

    //based on our behaviorState choose by what to value each point
    void OrderPointsByBehavior()
    {
        EvaluatedPointsArray = pointsToEvaluate.ToArray();

        //for each behavior sort the list by the values we care about
        //then add to their unappeal score equal to their position, then pick the position with the lowest score
        //shortest distance to closest target, shortest average distance to enemies, number of enemies in los, number of enemies in range
        if (current_ai_PositionalPreferenceState == ai_PositionalPreferenceState.aggressive)
        {
            EvaluatedPointsArray = EvaluatedPointsArray.OrderBy(x => x.numberOfExposedDetectionNodes).ToArray(); addUnappealAfterSorting(10);
            EvaluatedPointsArray = EvaluatedPointsArray.OrderByDescending(x => x.numberOfEnemiesInRange).ToArray(); addUnappealAfterSorting(2);
            EvaluatedPointsArray = EvaluatedPointsArray.OrderBy(x => x.distanceToClosestTarget).ToArray(); addUnappealAfterSorting(1);
            EvaluatedPointsArray = EvaluatedPointsArray.OrderBy(x => x.averageDistanceToEnemies).ToArray(); addUnappealAfterSorting(1);
            EvaluatedPointsArray = EvaluatedPointsArray.OrderByDescending(x => x.numberOfEnemiesInLOS).ToArray(); addUnappealAfterSorting(1);
        }
        if (current_ai_PositionalPreferenceState == ai_PositionalPreferenceState.defensive)
        {
            EvaluatedPointsArray = EvaluatedPointsArray.OrderBy(x => x.numberOfExposedDetectionNodes).ToArray(); addUnappealAfterSorting(10);
            EvaluatedPointsArray = EvaluatedPointsArray.OrderBy(x => x.numberOfEnemiesInLOS).ToArray(); addUnappealAfterSorting(1);
        }
        if (current_ai_PositionalPreferenceState == ai_PositionalPreferenceState.regrouping)
        {
            EvaluatedPointsArray = EvaluatedPointsArray.OrderBy(x => x.averageDistanceToAllies).ToArray(); addUnappealAfterSorting(2);
            EvaluatedPointsArray = EvaluatedPointsArray.OrderByDescending(x => x.numberOfAlliesInLOS).ToArray(); addUnappealAfterSorting(1);
            EvaluatedPointsArray = EvaluatedPointsArray.OrderBy(x => x.numberOfExposedDetectionNodes).ToArray(); addUnappealAfterSorting(3);
        }
        if (current_ai_PositionalPreferenceState == ai_PositionalPreferenceState.searching)
        {
            EvaluatedPointsArray = EvaluatedPointsArray.OrderByDescending(x => x.averageDistanceToAllies).ToArray(); addUnappealAfterSorting(10);
            EvaluatedPointsArray = EvaluatedPointsArray.OrderBy(x => x.numberOfAlliesInLOS).ToArray(); addUnappealAfterSorting(1);
        }

        SetTargetPosition();
    }
    //set target position and related variables 
    void SetTargetPosition()
    {
        EvaluatedPointsArray = EvaluatedPointsArray.OrderBy(x => x.unappealScore).ToArray();
        targetPosition = EvaluatedPointsArray[0];
        Debug.DrawLine(transform.position, targetPosition.worldSpacePostion, Color.blue, 5f);
        hasTargetPosition = true;
        targetPositionVerticalOffsetDifference = Mathf.Abs(transform.position.y - targetPosition.worldSpacePostion.y);
    }
    //add to the unappeal of each position
    void addUnappealAfterSorting(int weight)
    {
        for (int i = 0; i < EvaluatedPointsArray.Length; i++)
        {
            EvaluatedPointsArray[i].unappealScore = EvaluatedPointsArray[i].unappealScore + (i*weight);
        }
    }
    //stops moving if we're at the target position within a certain tolerance
    bool checkIfAtTargetPosition()
    {
        if ((targetPositionCheckTolerance > Mathf.Abs(transform.position.x - targetPosition.worldSpacePostion.x)) && 
            (targetPositionCheckTolerance + targetPositionVerticalOffsetDifference > Mathf.Abs(transform.position.y - targetPosition.worldSpacePostion.y)) && 
            (targetPositionCheckTolerance > Mathf.Abs(transform.position.z - targetPosition.worldSpacePostion.z)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //stops moving if we failed to reach the target location in time
    bool MovementFailsafeCheck()
    {
        movementFailsafeCurrentTime += Time.deltaTime;
        if (movementFailsafeCurrentTime >= movementFailsafeWaitTime)
        {
            return true;
        }
        return false;
    }
    //changes the positional preference based on our status
    void UpdatePositionalPreferenceState()
    {
        //make some calculations about what preference state to 
        //choose when i dont want to blow my fucking brains out

        //if (testingValueCount > 0)
        if (allEnemies.Count > 0)
        {
            //Debug.Log(allEnemies.Count);
            if ((float)(currentMorale/maxMorale) >= dangerValueMorale)
            {
                current_ai_PositionalPreferenceState = ai_PositionalPreferenceState.aggressive;
            }
            else
            {
                if ((float)(currentHealth / maxHealth) >= dangerValueHealth)
                {
                    current_ai_PositionalPreferenceState = ai_PositionalPreferenceState.defensive;
                }
                else
                {
                    current_ai_PositionalPreferenceState = ai_PositionalPreferenceState.regrouping;
                }
            }
        }
        else
        {
            current_ai_PositionalPreferenceState = ai_PositionalPreferenceState.searching;
        }

        // aggressive // enemies in los, high morale, high health
        // defensive // enemies in los, low morale, high health
        // regrouping //enemies in los, low morale, low health
        // searching //no enemies in los, morale high, health high

        //current_ai_PositionalPreferenceState = ai_PositionalPreferenceState.aggressive;
        //current_ai_PositionalPreferenceState = ai_PositionalPreferenceState.defensive;
        //current_ai_PositionalPreferenceState = ai_PositionalPreferenceState.regrouping;
        //current_ai_PositionalPreferenceState = ai_PositionalPreferenceState.searching;
    }
    //calculates how many movement points were spent when making a movement
    //when the unit reaches the target position
    void UpdateMovementPoints()
    {
        float x = Vector3.Distance(startingPosition, transform.position);
        currentMovementPoints = currentMovementPoints - x;
    }
} 