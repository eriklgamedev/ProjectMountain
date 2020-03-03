using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

public class SeedShooter : MonoBehaviour
{
    private bool holding = true;
    [HideInInspector]
    public GameObject seedPrefab = null;
    [HideInInspector]
    public GameObject rightSeedShooter, leftSeedShooter;

    private PlayerMovement playerMovement;
    private Pause pause;

    [SerializeField]
    private Transform spawnLocation;
	// Start is called before the first frame update

	public SoundFXRef shootSFX;
	
    void Start()
    {
        playerMovement = transform.root.GetComponent<PlayerMovement>();
        playerMovement.InitializeSeedTool(gameObject.GetComponent<SeedShooter>(), seedPrefab, rightSeedShooter, leftSeedShooter);
        pause = transform.root.GetComponent<Pause>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pause.playerIsActive == true)
        {
            InputCheck();
        }
            
    }
    void InputCheck() {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            if (leftSeedShooter.activeInHierarchy)
            {
				OVRInput.SetControllerVibration(.75f, .5f, OVRInput.Controller.LTouch);
				GameObject seed = Instantiate(seedPrefab);
                seed.transform.position = spawnLocation.position;
                seed.transform.rotation = spawnLocation.rotation;
                Physics.IgnoreCollision(seed.GetComponent<Collider>(), playerMovement.body.GetComponent<Collider>(), true);
                seed.GetComponent<SeedScript>().playerMovement = playerMovement;
				shootSFX.PlaySoundAt(transform.position);
				Invoke("EndHapticsLeft", .2f);
			}
		}


        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            if (rightSeedShooter.activeInHierarchy)
            {
				OVRInput.SetControllerVibration(.75f, .5f, OVRInput.Controller.RTouch);
				GameObject seed = Instantiate(seedPrefab);
                seed.transform.position = spawnLocation.position;
                seed.transform.rotation = spawnLocation.rotation;
                Physics.IgnoreCollision(seed.GetComponent<Collider>(), playerMovement.body.GetComponent<Collider>(), true);
                seed.GetComponent<SeedScript>().playerMovement = playerMovement;
				shootSFX.PlaySoundAt(transform.position);
				Invoke("EndHapticsRight", .2f);
			}
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
}
