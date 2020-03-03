using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCircle : MonoBehaviour
{
    [HideInInspector]
    public float elevation = 0;
    [HideInInspector]
    public float maxElevation = 0;
    [HideInInspector]
    public float minElevation = 0;
    [HideInInspector]
    public float prevElevation = 0;
    [HideInInspector]
    public float nextElevation = 0;

    public Player player;
    
    public float riskPercentage;
    private float storedRiskPercentage;
    [SerializeField]
    private float deactivationTime = 10f,
        high = 0.4f, medium = -1f, low = -15;
    private float maxRange = 25f;
    private string riskLevel;

    // Start is called before the first frame update
    void Start()
    {
        if (player != null) {
            high = player.high;
            medium = player.medium;
            low = player.low;
        }
        GetComponent<SpriteRenderer>().enabled = false;
        //GetComponent<SphereCollider>().enabled = false;
        if (riskPercentage == 0)
        {
            GetRiskPercentage();
        }
        SetColor();
        InvokeRepeating("LookAtPlayer", 0, 1);
    }
    
   
    private void LookAtPlayer() {
        if (GetComponent<SpriteRenderer>().enabled == true)
        {
            transform.LookAt(player.transform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (player != null) {
            PlayerMovement playerMovement = player.gameObject.GetComponent<PlayerMovement>();
            if (other.transform.root.gameObject.name == "Player")
            {
            
                if (riskLevel != playerMovement.riskLevel&&playerMovement.activeHand)
                {
                    Debug.Log("Got player");
                    CheckRiskLevel(playerMovement);
                    playerMovement.SetTimerForRisk();
                }
            }
            if (other.name == "DamageVolume_SporeCloud")
            {
                storedRiskPercentage = riskPercentage;
                riskPercentage = 0.7f;
                SetColor();
                CheckRiskLevel(playerMovement);
                playerMovement.SetTimerForRisk();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "DamageVolume_SporeCloud")
        {
            riskPercentage = storedRiskPercentage;
            SetColor();
        }
    }

    void CheckRiskLevel(PlayerMovement playerMovement)
    {
        if (riskPercentage >= high)
        {//Too high
            SetPlayerRiskLevel(playerMovement, 3f);
        }
        else if (riskPercentage >= medium)
        {//Just right
            SetPlayerRiskLevel(playerMovement, 25f); //25 is null, the setter will ignore anything above 20
        }
        else if (riskPercentage >= low)
        {//Too low
            SetPlayerRiskLevel(playerMovement, 10f);
        }
    }

    void SetPlayerRiskLevel(PlayerMovement playerMovement, float time)
    {
        if (time < 20)
        {
            playerMovement.colourTransitionTime = time;
        }
        playerMovement.imbalanceActive = true;
        playerMovement.prevRiskLevel = playerMovement.riskLevel;
        playerMovement.riskLevel = riskLevel;
    }

    public void StartDeactivation()
    {
        Invoke("DeactivateSelf", deactivationTime);
    }

    private void DeactivateSelf() {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    void GetRiskPercentage() {
        float differenceAverage = (((elevation - prevElevation) + (elevation - nextElevation)) / 2);
        riskPercentage = differenceAverage / ((maxElevation - minElevation)/100);
        //Debug.Log(percentage);
        SetColor();
    }

    void SetColor()
    {
        if (riskPercentage >= high)
        {//Too high
            riskLevel = "high";
            GetComponent<SpriteRenderer>().color = Color.red;
        }
        else if (riskPercentage >= medium)
        {//Just right
            riskLevel = "medium";
            GetComponent<SpriteRenderer>().color = Color.blue;
        }
        else if (riskPercentage >= low)
        {//Too low
            riskLevel = "low";
            GetComponent<SpriteRenderer>().color = Color.yellow;
        }
    }
}
