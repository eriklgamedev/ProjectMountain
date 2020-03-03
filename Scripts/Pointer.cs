using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using OVR;

public class Pointer : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenu, credits, options;
    private RaycastHit hit;
    [SerializeField]
    private LayerMask rayCastCollidableLayers;
    private float distance = 500f;
    private LineRenderer lineRenderer;
    [SerializeField]
    private Transform hand;

	public SoundFXRef blipSFX;


	// Start is called before the first frame update
	void Start()
    {
        //rayCastCollidableLayers = LayerMask.NameToLayer("UI");
        lineRenderer = GetComponent<LineRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        ShootRaycast();
    }

    void ShootRaycast() {
        Ray ray = new Ray(hand.position, hand.forward*distance);
        
        //Debug.Log("Cast direction" + ray.direction);
        Physics.Raycast(ray, out hit, distance + 0.1f, rayCastCollidableLayers);
        Debug.DrawRay(hand.position, hand.forward * distance, Color.red, 10);
        lineRenderer.SetPosition(0, hand.position);
        lineRenderer.SetPosition(1, hand.forward * distance);

        
        if (hit.collider == null)
        {
            Debug.Log("Pointer did not hit");
        }
        else {

            if (hit.collider.gameObject != null)
            {
                UIButton button = hit.collider.gameObject.GetComponent<UIButton>();
                button.mainMenu = mainMenu;
                button.credits = credits;
                button.options = options;
                Debug.Log(hit.collider.gameObject.name);

                if (OVRInput.GetDown(OVRInput.Button.One))
                {

                    blipSFX.PlaySoundAt(transform.position);
                    if (button.name == "Start")
                    {
                        Debug.Log("Hit");
                        button.StartGame();
                    }
                    else if (button.name == "Continue")
                    {
                        Debug.Log("Hit");
                        button.LoadGame();
                    }
                    else if (button.name == "Credits")
                    {
                        Debug.Log("Hit");
                        button.Credits();
                    }
                    else if (button.name == "Quit")
                    {
                        Debug.Log("Hit");
                        button.Quit();
                    }
                    else if (button.name == "Level Select")
                    {
                        Debug.Log("Hit");
                        button.LevelSelect();
                    }
                    else if (button.name == "Back")
                    {
                        button.Credits();

                    }
                    else if (button.name == "BackOption")
                    {
                        button.OptionsBack();
                    }
                    else if (button.name == "Resume")
                    {
                        button.Resume();
                    }
                    else if (button.name == "Return")
                    {
                        button.Return();
                    }
                    else if (button.name == "Options")
                    {
                        button.Options();
                    }
                    else if (button.name == "AB-Balance")
                    {
                        button.Balance();
                    }
                    else if (button.name == "AB-Climbing")
                    {
                        button.Climbing();
                    }
                    else if (button.name == "AB-Enemies")
                    {
                        button.Enemies();
                    }
                    else if (button.name == "AB-Flying")
                    {
                        button.Flying();
                    }
                    else if (button.name == "AB-Gardening")
                    {
                        button.Gardening();
                    }
                    else if (button.name == "AB-Generation")
                    {
                        button.Generation();
                    }
                    else if (button.name == "Music<Down")
                    {

                        Debug.Log("BHit");
                        button.MusicDown();
                    }
                    else if (button.name == "Music>Up")
                    {

                        Debug.Log("BHit");
                        button.MusicUp();
                    }
                    else if (button.name == "SFX<Down")
                    {

                        Debug.Log("BHit");
                        button.SFXDown();
                    }
                    else if (button.name == "SFX>Up")
                    {
                        Debug.Log("BHit");
                        button.SFXUp();
                    }
                    //button.gameObject.GetComponent<Material>().SetColor("green", Color.green);
                }
            }
            else {
                //button.gameObject.GetComponent<Material>().SetColor("white", Color.white);
            }
        }
    }

    private void StartGame() {

        SceneManager.LoadScene(1);
    }

    public void Credits()
    {
        mainMenu.SetActive(!mainMenu.activeSelf);
        credits.SetActive(!credits.activeSelf);
    }

	public void Quit()
    {
        Application.Quit();
    }
}
