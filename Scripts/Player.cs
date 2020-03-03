using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [HideInInspector]
    public Vector3 spawnPointPosition;
    [HideInInspector]
    public Quaternion spawnPointRotation;
    [SerializeField]
    private GameObject debugMenu, circleActivationZone;
    private bool canActivateCircle;
	public SoundFXRef backgroundSFX;
	public SoundFXRef respawnSFX;
	public SoundFXRef pingSFX;

    [SerializeField]
    private GameObject flyingToolLeft, flyingToolRight;

    public float high = 0.4f, medium = -1f, low = -15;
    [SerializeField]
    private float damageTime = 1f;
    [HideInInspector]
    public bool canGrab;
    [HideInInspector]
    public bool deactivateEnemies, deactivateCircles;
    // Start is called before the first frame update
    void Start()
    {
        canGrab = true;
        //Setting player's position and rotation
        spawnPointPosition = transform.position + new Vector3(0, 4);
        spawnPointRotation = transform.rotation;

		backgroundSFX.PlaySoundAt(transform.position);
	}

	// Update is called once per frame
	void Update()
    {

		///Debug.Log(spawnPointPosition);

		//Debug.Log(Input.GetKeyDown(KeyCode.BackQuote));
		if (Input.GetKeyDown(KeyCode.BackQuote))
		{
			debugMenu.GetComponent<DebugMenu>().canDebug = !debugMenu.GetComponent<DebugMenu>().canDebug;
			debugMenu.SetActive(!debugMenu.activeSelf);
		}

		if (OVRInput.GetDown(OVRInput.Button.Two)&&canActivateCircle == false)
        {
			//Debug.Log("circle activation pressed");
			pingSFX.PlaySoundAt(transform.position);
			Instantiate(circleActivationZone, gameObject.transform.position, gameObject.transform.rotation);
            Invoke("ReenableCircle", 1f);
            canActivateCircle = true;
        }

        if (debugMenu.activeSelf == true)
        {
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                deactivateEnemies = !deactivateEnemies;
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                deactivateCircles = !deactivateCircles;
            }
        }
    }

    public void ActivateFlyingTools() {
        flyingToolLeft.SetActive(true);
        flyingToolRight.SetActive(true);
    }

    private void ReenableCircle() {
        canActivateCircle = false;
    }

    public void Damage()
    {
        //Debug.Log("Player is taking damage!");
        //GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, -transform.position.z) * 8, ForceMode.Impulse);
        GetComponent<PlayerMovement>().activeHand = null;
        GetComponent<PlayerMovement>().GravityActivation(true);
        canGrab = false;
        Invoke("ReactivateGrab", damageTime);
    }

    void ReactivateGrab() {
        canGrab = true;
    }

    public void Death()
    {
        OVRScreenFade screenFade = transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<OVRScreenFade>();
        screenFade.FadeOut();

        Invoke("RespawnPlayer", screenFade.fadeTime + 1f);
    }

    private void RespawnPlayer()
    {

        transform.position = spawnPointPosition;
        //Debug.Log(transform.position);
        transform.rotation = spawnPointRotation;

		respawnSFX.PlaySoundAt(transform.position);

		//GetComponent<Rigidbody>().velocity= new Vector3(0, 0, 0);
	}
}
