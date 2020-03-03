using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;


/// <summary>
/// Handles the players primary movement capabilities
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Character Movement")]
    private CharacterController characterController;
    private float speed = 5;
    private bool readyToSnapTurn = true; // Set to true when a snap turn has occurred, code requires one frame of centered thumbstick to enable another snap turn.
    private Vector3 currentAngle;
    private Vector3 finalAngle;
    [SerializeField]
    private Transform cameraTransform, trackingSpaceTransform;
    [SerializeField]
    private float rotationMultiplier, rotationRatchet;

    [Header("Balance System")]
    [HideInInspector]
    public PlayerHands activeHand, leftHand, rightHand;
    [SerializeField]
    private Transform headTransform;
    [HideInInspector]
    public float colourTransitionTime;
    [HideInInspector]
    public int currentTimer;
    [HideInInspector]
    public bool imbalanceActive, resetTimer, setTimer, aligned, recovered,outSideZone;
    [HideInInspector]
    public string riskLevel, prevRiskLevel = "medium";
    private Vector3 handVelocity;
    [SerializeField]
    private GameObject balanceCenterPrefab;
    private GameObject balanceCenter;

    [Header("Climbing Variables")]
    public float grabDistance = 0.5f;
    [SerializeField]
    private Rigidbody r;
    [SerializeField]
    SphereCollider bottomSphere;
    [SerializeField]
    private float hopStrength = 16f, hopCooldown = 1f, hopTime = 1f;
    [SerializeField]
    private int minDrag = 3, maxDrag = 5, maxHopCount = 1;
    [HideInInspector]
    public int hopCount = 0;
    public SoundFXRef hopSFX;
    private bool playHopSound;

    [Header("Tool Variables")]
    public bool seedShooterActive;
    //Flying Tool
    [SerializeField]
    private float flyingForce;
   
    //Seed Shooter
    [SerializeField]
    private GameObject seedPrefab;
    public GameObject rightFlyingTool, leftFlyingTool, rightSeedShooter, leftSeedShooter;


    [Header("Body and Ground Check")]
    public SphereCollider body;
    private float distToGround;
    private bool isGrounded;

    //public enum MovementSystem {climbing, flying, walking}
    //public MovementSystem movementSystem;

    private void Start()
    {
        distToGround = body.radius / 2 + 1f;
        characterController = GetComponent<CharacterController>();
        hopCount = maxHopCount;
        balanceCenter = Instantiate(balanceCenterPrefab, transform.position, transform.rotation);
        balanceCenter.SetActive(false);
        //InvokeRepeating("RiskLevelCheck", 0, 1);
    }

    void Update()
    {
        //Debug.Log("Velocity: " + trackingSpaceTransform.TransformPoint(OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch)));
        //Debug.Log(riskLevel);
        CheckOffGround();
        ActivateWalk();
        //SnapRotation();

        //Debug.Log("prev: " + prevRiskLevel + " current: " + riskLevel);
    }

    private void FixedUpdate()
    {
        if (activeHand == null )
        {
            if (balanceCenter != null) {
                balanceCenter.SetActive(false);
            }
            //bottomSphere.enabled = true;
            if (isGrounded == true)
            {
                if (CheckMoveableTerrain(transform.position, -Vector3.up, distToGround))
                {
                    Walking();
                }
                
             
            }
        }
        if (activeHand != null && isGrounded == false)
        {
            //bottomSphere.enabled = false;
        }
    }

    public void SetTimerForRisk()
    {

        if (activeHand != null && prevRiskLevel != riskLevel)
        {
            ResetTimer();
            //if (resetTimer == false) {
            //Debug.Log("different zone");
            switch (riskLevel)
            {
                case "high":
                    RiskLevelCheck();

                    break;
                case "medium":
                    StopTimer();
                    currentTimer = 0;

                    break;
                case "low":
                    RiskLevelCheck();

                    break;
                default:
                    StopTimer();
                    currentTimer = 0;

                    break;
                    // }
                    // resetTimer = true;
            }

        }
    }


    public void ResetTimer() {
        if (activeHand == null) {
            imbalanceActive = false;
            currentTimer = 0;
            riskLevel = "medium";
            StopTimer();
            CancelInvoke("LoseGrip");
        }
    }

    void RiskLevelCheck()
    {
        setTimer = true;
        SetTimer((int)colourTransitionTime);
        if (imbalanceActive == true)
        {
            RunTimer(2);
        }
        else if (imbalanceActive == false)
        {
            StopTimer();
        }
    }

    public void SetTimer(int time)
    {
        if (setTimer == true)
        {
            currentTimer = time;
            setTimer = false;
        }
    }


    void CountdownTimerStartBalance()
    {

        //Debug.Log("CountDownBalance");
        if (currentTimer > 0)
        {
            //Debug.Log(currentTimer);
            currentTimer--;
        }
        else
        {

            //LoseGrip();
            CheckImbalance();
            CancelInvoke("CountdownTimerStartBalance");
        }
    }

    void CountdownTimer()
    {
        //Debug.Log("CountDown");
        if (currentTimer > 0)
        {
            //Debug.Log(currentTimer);
            currentTimer--;
        }
        else
        {
            LoseGrip();
            CancelInvoke("CountdownTimer");
        }
    }

    void CountdownRecovery() {

        //Debug.Log("CountDownRecovery");
        if (currentTimer > 0)
        {
            //Debug.Log(currentTimer);
            currentTimer--;
        }
        else
        {
            Debug.Log("Recovered");
            Recover();

        }
    }

    void Recover() {
        //recovered = false;
        currentTimer = 0;
        CancelInvoke();
        leftHand.ColorChangesHand(Color.green);
        rightHand.ColorChangesHand(Color.green);
        balanceCenter.SetActive(false);
    }

    void RunTimer(int recovering)//0 is fall, 1 is recover, 2 balance
    {
        if (recovering == 0)
        {
            if (!IsInvoking("CountdownTimer"))
            {
                InvokeRepeating("CountdownTimer", 0, 1);
            }
        }
        else if(recovering == 1)
        {
            if (!IsInvoking("CountdownRecovery")) {
                InvokeRepeating("CountdownRecovery", 0, 1);
            }
        }
        else if (recovering == 2)
        {
            if (!IsInvoking("CountdownTimerStartBalance"))
            {
                InvokeRepeating("CountdownTimerStartBalance", 0, 1);
            }
        }

    }

    void StopTimer()
    {
        CancelInvoke("CountdownTimer");
        CancelInvoke("CountdownRecovery");
        CancelInvoke("CountdownTimerStartBalance");
    }
    /// <summary>
    /// Imbalance events
    /// If the player is not holding with both hands, they fall
    /// If they are, balancing event starts
    /// They will need to place their body close to the balanceCenter
    /// They have 3 seconds to do this
    /// If they fail, they fall
    /// 
    /// Once they enter the balance zoon, they need to stay for two seconds
    /// Then they are rebalanced and all counter stops 
    /// Until they grab again
    /// 
    /// If they move outside the zoon before the countdown ends, they will fall
    /// 
    /// 
    /// </summary>
    void CheckImbalance()
    {
        if (activeHand != null) {
            
            if (activeHand.bothHandGrab == true)
            {
                if (aligned == false)
                {
                    //Debug.Log("StartBalance");
                    setTimer = true;
                    SetTimer(3);
                    RunTimer(0);
                    SpawnBalanceCenter();
                    InvokeRepeating("CheckForAlignment", 0, 0.1f);
                    
                }
                else
                {
                    currentTimer = 0;
                    CancelInvoke();
                }
            }
            else
            {
                StopTimer();
                LoseGrip();
            }
        }
        
    }


    void SpawnBalanceCenter()
    {
        Vector3 leftHandPosition = leftHand.gameObject.transform.position;
        Vector3 rightHandPosition = rightHand.gameObject.transform.position;
        Vector3 higherHand, lowerHand;
        float increment = 0.15f;
        if (leftHandPosition.y > rightHandPosition.y)
        {
            higherHand = leftHandPosition;
            lowerHand = rightHandPosition;
        }
        else
        {
            higherHand = rightHandPosition;
            lowerHand = leftHandPosition;
        }
        float heightDifferece = higherHand.y = lowerHand.y;

        Vector3 centerPoint = (leftHandPosition + rightHandPosition) / 2;
        Vector3 spawnPoint = Vector3.Lerp(centerPoint, higherHand, heightDifferece * increment);
        if (balanceCenter != null)
        {
            balanceCenter.SetActive(true);
            balanceCenter.transform.position = spawnPoint;
           
        }
    }

    void CheckForAlignment()
    {
        //Check if the body/head is aligned with the balance center
        //Distance check between the body and the balanceCenter
        if (balanceCenter != null)
        {
            MaterialPropertyBlock colorBlock = new MaterialPropertyBlock();
            //Debug.Log(Vector3.Distance(headTransform.position, balanceCenter.transform.position));
            if (Vector3.Distance(headTransform.position, balanceCenter.transform.position) <= 0.4f)//if player body is in range
            {
                if (recovered == false) {
                    setTimer = true;
                    SetTimer(2);
                    RunTimer(1);
                    recovered = true;
                }

                colorBlock.SetColor("_BaseColor", Color.green);
                //Start countdown for rebalance
            }
            else
            {
                if (outSideZone == false) {
                    setTimer = true;
                    SetTimer(3);
                    RunTimer(0);
                    outSideZone = true;
                }
                colorBlock.SetColor("_BaseColor", Color.red);
                //Start countdown for fall
            }

            balanceCenter.GetComponent<Renderer>().SetPropertyBlock(colorBlock);
        }

    }



    void LoseGrip()
    {
        
        Debug.Log("Lost Grip");
        gameObject.GetComponent<Player>().Damage();
        imbalanceActive = false;
        balanceCenter.SetActive(false);
        currentTimer = 0;
        riskLevel = "medium";
        StopTimer();
        outSideZone = false;
        CancelInvoke("LoseGrip");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Physics.Raycast(body.transform.position, -Vector3.up, distToGround))
        {
            isGrounded = true;

            //Debug.Log("Grounded");
        }
        Debug.DrawRay(body.transform.position, -Vector3.up, Color.red, 10f);

    }

    void CheckOffGround()
    {
        RaycastHit hit;
        if (!Physics.SphereCast(body.transform.position, body.radius, -transform.up, out hit, distToGround))
        {
            isGrounded = false;
        }

        //Debug.Log("Grounded:" + isGrounded);
    }

    public void InitializeHand(PlayerHands hand, GameObject rightFlyingTool, GameObject leftFlyingTool, GameObject rightSeedShooter, GameObject leftSeedShooter)
    {
        hand.rightFlyingTool = this.rightFlyingTool;
        hand.leftFlyingTool = this.leftFlyingTool;
        hand.rightSeedShooter = this.rightSeedShooter;
        hand.leftSeedShooter = this.leftSeedShooter;
    }

    public void InitializeFlyingTool(FlyingTool flyingTool, float force, Rigidbody r)
    {
        flyingTool.force = flyingForce;
        flyingTool.r = this.r;
        
    }

    public void InitializeSeedTool(SeedShooter seedShooter, GameObject seedPrefab, GameObject rightSeedShotter, GameObject leftSeedShooter)
    {
        seedShooter.seedPrefab = this.seedPrefab;
        seedShooter.rightSeedShooter = this.rightSeedShooter;
        seedShooter.leftSeedShooter = this.leftSeedShooter;
    }

    public void SetActiveHand(PlayerHands hand)
    {
        activeHand = hand;
    }


    private void ActivateWalk()
    {
        //if is not grabbing, and is grounded, and the charaterController is not enabled
        if (activeHand == null && isGrounded == true)
        {
            //If is not grabbing, and is not grunded (walk off edge), and the characterController is already enabled
        }
        else if (activeHand == null && isGrounded == false)
        {
            r.isKinematic = false;
        }
        else if (activeHand != null)// if is grabbing
        {
            r.isKinematic = false;
        }
    }

    void Walking()
    {
        Camera camera = Camera.main;
        Vector3 headDirection = camera.transform.forward;
        Vector3 headRight = camera.transform.right;
        //Debug.Log(headDirection);
        //Debug.Log("Right: " + headRight);
        Vector2 moveAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        //Debug.Log(moveAxis);
        headDirection.y = 0f;
        headRight.y = 0f;
        headDirection.Normalize();
        headRight.Normalize();

        var moveDirection = headDirection * moveAxis.y + headRight * moveAxis.x;
        transform.Translate(moveDirection * speed * Time.deltaTime);
   
    }

    bool CheckMoveableTerrain(Vector3 position, Vector3 desiredDirection, float distance)
    {
        Ray myRay = new Ray(position, desiredDirection); 
        RaycastHit hit;

        if (Physics.Raycast(myRay, out hit, distance))
        {
           
            float slopeAngle = Mathf.Deg2Rad * Vector3.Angle(Vector3.up, hit.normal); 
            float radius = Mathf.Abs(distToGround / Mathf.Sin(slopeAngle));
           
            if (slopeAngle >= 35 * Mathf.Deg2Rad) 
            {
                if (hit.distance - body.radius > Mathf.Abs(Mathf.Cos(slopeAngle) * radius) + 0.1)             
                {
                    return true; // return true if we are still far away from the slope
                }

                return false; // return false if we are very near / on the slope && the slope is steep
            }
            return true; // return true if the slope is not steep
        }
        return true;
    }




    // Climbing functionality
    public void GetHandTranslation()
    {
        if (activeHand != null&&gameObject.GetComponent<Player>().canGrab == true)
        {
            if (activeHand.left == true)
            {
                //r.AddForce(-trackingSpaceTransform.TransformPoint(OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch)) * 0.05f, ForceMode.Impulse);
                r.AddForce(-OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch) * 0.05f, ForceMode.Impulse);
                //Debug.Log("velocity: " + activeHand.GetComponent<Rigidbody>().velocity);
            }
            else if (activeHand.left == false)
            {
                //r.AddForce(-trackingSpaceTransform.TransformPoint(OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch)) * 0.05f, ForceMode.Impulse);
                r.AddForce(-OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch) * 0.05f, ForceMode.Impulse);
                //Debug.Log("velocity: " + activeHand.GetComponent<Rigidbody>().velocity);
            }
        }
    }


    // state that controls the hop while the player is climbing 
    public void HopClimb()
    {
        //Debug.Log(r.velocity.magnitude);
        if (r.velocity.magnitude < hopStrength&& gameObject.GetComponent<Player>().canGrab == true)
        {
            if (hopCount > 0) {
                r.AddForce(-OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch) + -OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch) * 0.01f, ForceMode.Impulse);
                if (playHopSound == false) {
                    hopSFX.PlaySoundAt(transform.position);
                    playHopSound = true;
                }
                if (!IsInvoking("HopCooldown")) {
                    Invoke("HopCooldown", hopTime);
                }
            }
        }
    }



    void HopCooldown() {
        hopCount--;
        GravityActivation(true);
        activeHand = null;
        playHopSound = false;
        Invoke("RegainHop", hopCooldown);
    }

    void RegainHop() {
        hopCount = maxHopCount;
    }

    public void GravityActivation(bool useGravity)
    {
        r.useGravity = useGravity;
        if (r.useGravity == true && activeHand != null)
        {
            if (activeHand.bothHandGrab == false)
            {

                r.drag = minDrag;

            }
        }
        else if (r.useGravity == false)
        {
            r.drag = maxDrag;
        }
    }


    void SnapRotation()
    {
        Vector3 euler = cameraTransform.rotation.eulerAngles;

        if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft))
        {
            if (readyToSnapTurn)
            {
                euler.y -= rotationRatchet;

                readyToSnapTurn = false;
            }
        }
        else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight))
        {
            if (readyToSnapTurn)
            {
                euler.y += rotationRatchet;
                readyToSnapTurn = false;
            }
        }
        else
        {
            readyToSnapTurn = true;
        }
        cameraTransform.rotation = Quaternion.Euler(euler);
    }

    public void EmptyHand()
    {
        activeHand = null;
    }
}
