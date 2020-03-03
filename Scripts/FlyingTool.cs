using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

public class FlyingTool : MonoBehaviour
{
    //public Transform toolTransform;
    [HideInInspector]
    public float force;
	[HideInInspector]
    public Rigidbody r;
	
    private Pause pause;
    private PlayerMovement playerMovement;

	public SoundFXRef flySFX;

	// Start is called before the first frame update
	void Start()
    {
        playerMovement = transform.root.GetComponent<PlayerMovement>();
        playerMovement.InitializeFlyingTool(this, force, r);
        pause = transform.root.GetComponent<Pause>();
		r = transform.root.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (pause.playerIsActive == true)
		{
			if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
			{
				if (playerMovement.leftFlyingTool.activeInHierarchy)
				{
					r.AddForce(gameObject.transform.up * force, ForceMode.Impulse);
				}
			}

			if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
			{
				if (playerMovement.rightFlyingTool.activeInHierarchy)
				{
					r.AddForce(gameObject.transform.up * force, ForceMode.Impulse);
				}
			}
		}
		
	}

}
