using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using OVR;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// This script handles the win condition
    /// </summary>
    ///
    [SerializeField]
    public int flowersRequired = 4;//Change this in the scene
    [HideInInspector]
    public int flowerCount = 0;
    [SerializeField]
    private GameObject wonText = null;

    [HideInInspector]
    public int sceneIndex;

	public SoundFXRef winSFX;

    public Player player;

    private void Awake()
    {
        flowerCount = 0;

        SceneManager.sceneLoaded += SceneSave;
    }

	// Update is called once per frame
	void Update()
    {
		//Debug.Log(flowerCount);
        if (flowerCount >= flowersRequired)
		{
			//Debug.Log("Won");
            if (SceneManager.GetActiveScene().name == "FirstScene")
            {
				//Debug.Log("SceneChecked");
				Won();
            }
            else
            {
				winSFX.PlaySound();
				SceneManager.LoadScene(2);
            }
        }
    }

    void Won()
	{
		winSFX.PlaySound();

		if (wonText != null) {
			wonText.SetActive(true);
		}

        player.ActivateFlyingTools();
        //Go back to main menu
    }

    private void BackToMainMenu() {
        SceneManager.LoadScene(0);
    }

    void SceneSave(Scene scene, LoadSceneMode load)
    {
        sceneIndex = scene.buildIndex;
    }
}
