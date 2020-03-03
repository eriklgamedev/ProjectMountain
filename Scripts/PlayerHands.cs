using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OVR;

/// <summary>
/// This script handles the hand collision and detection.
/// </summary>
public class PlayerHands : MonoBehaviour
{
    // bools that control check if the player can preform a hop
	[HideInInspector]
    public bool bothHandGrab;

    public bool left;
    [HideInInspector]
    public GameObject rightFlyingTool, leftFlyingTool, rightSeedShooter, leftSeedShooter;
    
	//[SerializeField]
    private PlayerMovement playerMovement;
	LineRenderer lineRenderer;

	public SoundFXRef grabSFX;
	public SoundFXRef flowerCollectSFX;


	[SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private Renderer handRenderer, wristRenderer;
    [SerializeField]
    private Text countdownTimer;

    private GameObject goalFlower;
    private bool flyingToolActive, seedToolActive, goalFlowerActivate, isHopping, isHoldingTool;


	private void Start()
    {
        playerMovement = transform.root.GetComponent<PlayerMovement>();
        playerMovement.InitializeHand(gameObject.GetComponent<PlayerHands>(), rightFlyingTool, leftFlyingTool, rightSeedShooter, leftSeedShooter);
        
        if (left == true)
        {
            playerMovement.leftHand = this;
        }
        else
        {
            playerMovement.rightHand = this;
        }

        lineRenderer = GetComponent<LineRenderer>();
        countdownTimer.enabled = false;
        MaterialPropertyBlock colorBlock = new MaterialPropertyBlock();
        colorBlock.SetColor("_BaseColor", Color.green);
        handRenderer.SetPropertyBlock(colorBlock);
        wristRenderer.SetPropertyBlock(colorBlock);
        InvokeRepeating("RiskLevelCheck", 0, 0.5f);
        InvokeRepeating("CountdownTimer", 0, 0.5f);
        InvokeRepeating("WristColorChanges", 0, 0.2f);
        InvokeRepeating("CheckHoldingTool", 0, 1);
    }


    private void Update()
    {
        InvokingActivations();
        HopCheck();

        if (playerMovement.gameObject.GetComponent<Player>().canGrab) {
            ClimbGrab();
        }

        ReleaseCheck();
        ToolRelease();
        
    }

    private void FixedUpdate()
    {
        playerMovement.GetHandTranslation();
    }

    void CheckHoldingTool() {
        if (left == true)
        {
            if (leftFlyingTool.activeInHierarchy || leftSeedShooter.activeInHierarchy) {
                isHoldingTool = true;
            } else if (!leftFlyingTool.activeInHierarchy && !leftSeedShooter.activeInHierarchy) {
                isHoldingTool = false;
            }
        }
        else
        {
            if (rightFlyingTool.activeInHierarchy || rightSeedShooter.activeInHierarchy) {
                isHoldingTool = true;
            } else if (!rightFlyingTool.activeInHierarchy && !rightSeedShooter.activeInHierarchy) {
                isHoldingTool = false;
            }
        }

    }



    void CountdownTimer()
    {
        countdownTimer.text = playerMovement.currentTimer.ToString();

        if (playerMovement.activeHand == null)
        {
            countdownTimer.enabled = false;
        }
    }



    void RiskLevelCheck()
    {
        ColorCheck();
    }

    void ColorCheck()
    {
        switch (playerMovement.riskLevel)
        {
            case "high":
                ColorChanges(Color.red, handRenderer);
                break;
            case "medium":
                ColorChanges(Color.green, handRenderer);
                break;
            case "low":
                ColorChanges(Color.yellow, handRenderer);
                break;
            default:
                ColorChanges(Color.green, handRenderer);
                break;

        }
    }

    void WristColorChanges() {
        if (playerMovement.hopCount > 0)
        {

            ColorChanges(Color.green, wristRenderer);
        }
        else {

            ColorChanges(Color.red, wristRenderer);
        }
    }

    void ColorChanges(Color color, Renderer renderer)
    {
        MaterialPropertyBlock colorBlock = new MaterialPropertyBlock();
        colorBlock.SetColor("_BaseColor", color);
        renderer.SetPropertyBlock(colorBlock);
    }

    public void ColorChangesHand(Color color) {
        ColorChanges(color, handRenderer);
    }

    void InvokingActivations() {
        if (flyingToolActive == true) {
            FlyingToolActivation();
        }
        if (seedToolActive == true) {
            SeedToolActivation();
        }
        if (goalFlowerActivate == true) {
            if (goalFlower != null) {
                GoalFlower(goalFlower);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag) {
            case "FlyingToolSocket":
                flyingToolActive = true;
                break;
            case "SeedToolSocket":
                seedToolActive = true;
                break;
            case "GoalFlower":
                goalFlowerActivate = true;
                goalFlower = other.gameObject;
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "FlyingToolSocket":
                flyingToolActive = false;
                break;
            case "SeedToolSocket":
                seedToolActive = false;
                break;
            case "GoalFlower":
                goalFlowerActivate = false;
                goalFlower = null;
                break;
        }
    }

    void FlyingToolActivation() {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger) && left == true)
        {
            if (!leftSeedShooter.activeInHierarchy && !rightFlyingTool.activeInHierarchy)
            {
                leftFlyingTool.SetActive(true);//Active left flying

                //playerMovement.SetActiveHand(gameObject.GetComponent<PlayerHands>());
            }

        }

        if (OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger) && left == false)
        {
            //checks to see if the right tool1 is not active
            if (!rightSeedShooter.activeInHierarchy && !leftFlyingTool.activeInHierarchy)
            {
                rightFlyingTool.SetActive(true);//Active right tool 2

                //playerMovement.SetActiveHand(gameObject.GetComponent<PlayerHands>());
            }
        }
    }

    void SeedToolActivation() {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger) && left == true)
        {
            if (!leftFlyingTool.activeInHierarchy && !rightSeedShooter.activeInHierarchy)
            {

                leftSeedShooter.SetActive(true);//Active left tool 2

                //playerMovement.SetActiveHand(gameObject.GetComponent<PlayerHands>());

            }

        }

        if (OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger) && left == false)
        {
            //checks to see if the right tool1 is not active
            if (!rightFlyingTool.activeInHierarchy && !leftSeedShooter.activeInHierarchy)
            {

                rightSeedShooter.SetActive(true);//Active right tool 2

                //playerMovement.SetActiveHand(gameObject.GetComponent<PlayerHands>());


            }
        }
    }

    void GoalFlower(GameObject flower) {
        if ((OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger)&&left == true) || (OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger)&&left == false))
        {
            flowerCollectSFX.PlaySoundAt(transform.position);

            gameManager.GetComponent<GameManager>().flowerCount++;

            Destroy(flower.gameObject);
        }
    }

    /*OnTriggerStay
    void OnTriggerStay(Collider other)
    {
        
        //checks if the player is using the Left trigger
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger) && left == true)
        {

            //Debug.Log("Flying ToolLeft");
            if (other.CompareTag("FlyingToolSocket"))
            {

                //checks to see if the left seed and right flying is not active
                if (!leftSeedShooter.activeInHierarchy && !rightFlyingTool.activeInHierarchy)
                {
                    leftFlyingTool.SetActive(true);//Active left flying

                    playerMovement.SetActiveHand(gameObject.GetComponent<PlayerHands>());
                }
            }
            if (other.CompareTag("SeedToolSocket"))
            {

                //Debug.Log("Seed Tool Left");
                if (!leftFlyingTool.activeInHierarchy && !rightSeedShooter.activeInHierarchy)
                {

                    leftSeedShooter.SetActive(true);//Active left tool 2

                    playerMovement.SetActiveHand(gameObject.GetComponent<PlayerHands>());

                }
            }
        }

        if (OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger) && left == false)
        {

            //Debug.Log("Flying Tool Right");
            if (other.CompareTag("FlyingToolSocket"))
            {

                Debug.Log("Flying Tool Right");
                //checks to see if the right tool1 is not active
                if (!rightSeedShooter.activeInHierarchy && !leftFlyingTool.activeInHierarchy)
                {
                    rightFlyingTool.SetActive(true);//Active right tool 2

                    playerMovement.SetActiveHand(gameObject.GetComponent<PlayerHands>());
                }
            }
            if (other.CompareTag("SeedToolSocket"))
            {

                //Debug.Log("Seed Tool Right");
                if (!rightFlyingTool.activeInHierarchy && !leftSeedShooter.activeInHierarchy)
                {

                    rightSeedShooter.SetActive(true);//Active right tool 2

                    playerMovement.SetActiveHand(gameObject.GetComponent<PlayerHands>());


                }
            }
        }

        if (other.gameObject.tag == "GoalFlower")
        {
			if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger) && left == true)
			{
				flowerCollectSFX.PlaySoundAt(transform.position);

				gameManager.GetComponent<GameManager>().flowerCount++;

				Destroy(other.gameObject);
			}
			else if(OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger)&&left == false)
			{
				flowerCollectSFX.PlaySoundAt(transform.position);

				gameManager.GetComponent<GameManager>().flowerCount++;

				Destroy(other.gameObject);
			}
        }

    }
    */

    void CLimbingActivation() {
        playerMovement.SetActiveHand(gameObject.GetComponent<PlayerHands>());
        playerMovement.GravityActivation(false);
        if (playerMovement.recovered == true)
        {
            //playerMovement.SetTimerForRisk();
            playerMovement.recovered = false;
        }
        countdownTimer.enabled = true;
        Invoke("EndHapticsLeft", .2f);
        grabSFX.PlaySoundAt(transform.position);
        isHopping = false;
        CancelInvoke("HopReset");
    }


    private void ClimbGrab() //Needs to be cleaned
    {
		RaycastHit hit;
		if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up + transform.forward * .1f), out hit))
		{
			if (hit.distance < playerMovement.grabDistance)
			{
                
				lineRenderer.SetPosition(0, transform.position);
				lineRenderer.SetPosition(1, -transform.up * 1 + transform.forward * .5f + transform.position);
                
				if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger) && left == true && isHoldingTool == false
                    && flyingToolActive == false && seedToolActive == false)
				{
                    lineRenderer.startColor = Color.green;
                    lineRenderer.endColor = Color.white;
                    OVRInput.SetControllerVibration(.5f, .1f, OVRInput.Controller.LTouch);
                    CLimbingActivation();
                }

				if (OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger) && left == false && isHoldingTool == false
                    && flyingToolActive == false && seedToolActive == false)
                {
                    lineRenderer.startColor = Color.green;
                    lineRenderer.endColor = Color.white;
                    OVRInput.SetControllerVibration(.5f, .1f, OVRInput.Controller.RTouch);
                    CLimbingActivation();


                }

                //Both grab
				if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) && OVRInput.Get(OVRInput.Button.SecondaryHandTrigger)&&isHoldingTool == false)
				{
                    //playerMovement.SetTimerForRisk();
                    lineRenderer.startColor = Color.green;
                    lineRenderer.endColor = Color.white;
                    countdownTimer.enabled = true;
                    bothHandGrab = true;
                    isHopping = false;
                }
				else {
					bothHandGrab = false;
				}
				

            }
			else
			{
				lineRenderer.SetPosition(0, transform.position);
				lineRenderer.SetPosition(1, transform.position);//(1, -transform.up * .1f + transform.forward * .01f + transform.position);
			}
		}
        Vector3 forward = transform.TransformDirection(-Vector3.up + transform.forward * .1f) * 1f;
		//Debug.DrawRay(transform.position, forward, Color.green); 
	}

    void ReleaseCheck() {
        if (playerMovement.activeHand != this&&bothHandGrab == false) {
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.white;
        }

        if (!OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) && !OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
        {
            //Debug.Log("BothRelesed");
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.white;
            playerMovement.GravityActivation(true);
            playerMovement.EmptyHand();
            playerMovement.ResetTimer();
        }
    }

	void EndHapticsLeft()
	{
		OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
	}
	void EndHapticsRight()
	{
		OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
	}

    // Function that controls the Hop action 
    private void HopCheck()
    {
        //  Debug.Log("Left is " + leftHandGrab);
        //  Debug.Log("Right is " + rightHandGrab);

       
		//Debug.Log("Hop is " + hopReady);
		if (bothHandGrab == true)
        {
            
            if (OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch).magnitude > 2.5f)
            {
                playerMovement.SetTimerForRisk();
                playerMovement.GravityActivation(false);
                playerMovement.HopClimb();
                isHopping = true;
                
            }
         
        }
    }


    private void ToolCheck(GameObject leftTool_1, GameObject leftTool_2, GameObject rightTool_1, GameObject rightTool_2)
    {
        //checks if the player is using the Left trigger
        if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
        {
            left = true;
            //checks to see if the left tool 1 is not active
            if (!leftTool_1.activeInHierarchy)
            {
                leftTool_2.SetActive(true);//Active left tool 2

                //playerMovement.SetActiveHand(gameObject.GetComponent<PlayerHands>());
            }
        }
        //checks if the player is using the right trigger
        if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
        {
            left = false;
            //checks to see if the right tool1 is not active
            if (rightTool_1.activeInHierarchy)
            {
                rightTool_2.SetActive(true);//Active right tool 2

                //playerMovement.SetActiveHand(gameObject.GetComponent<PlayerHands>());
            }
        }
        if (!OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) && !OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
        {
            playerMovement.EmptyHand();
        }
    }


    private void ToolRelease()
    {
        if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger))
        {
            ToolDeactivate(leftFlyingTool, leftSeedShooter);

        }

        if (OVRInput.GetUp(OVRInput.Button.SecondaryHandTrigger))
        {
            ToolDeactivate(rightFlyingTool, rightSeedShooter);

        }

        if (!OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) && !OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
        {
            playerMovement.EmptyHand();
        }
    }


    private void ToolDeactivate(GameObject tool_1, GameObject tool_2)
    {
        tool_1.SetActive(false);
        tool_2.SetActive(false);
    }
}