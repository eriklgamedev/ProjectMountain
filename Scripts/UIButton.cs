using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using OVR;

public class UIButton : MonoBehaviour
{
    [HideInInspector]
    public GameObject mainMenu;
    [HideInInspector]
    public GameObject credits;
	[SerializeField]
    private GameObject player;

	public GameObject options;
	[SerializeField]
	private AudioMixerGroup soundMixer, musicMixer;

	public SoundFXRef blipSFX;

    [Range(-80, 20)]
    private int soundVolume = 0, musicVolume = 0;

    public LoadController loadController;

    private bool levelSelect = false;

    [SerializeField]
    private GameObject[] actionButtons = new GameObject[6];

    private void Start()
    {
        foreach(GameObject button in actionButtons)
        {
            button.SetActive(false);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Credits()
    {
        if (mainMenu != null && credits != null&&options != null)
        {
            Debug.Log("Credit");
            mainMenu.SetActive(!mainMenu.activeSelf);
            //options.SetActive(!options.activeSelf);
            credits.SetActive(!credits.activeSelf);
        }
        
    }

    public void OptionsBack()
    {
        if (mainMenu != null && options != null)
        {
            mainMenu.SetActive(!mainMenu.activeSelf);
            options.SetActive(!options.activeSelf);
        }

    }

    public void LoadGame()
    {
        Debug.Log("Game Loading...");

        SceneManager.LoadScene(StoredData.current.level);

        loadController.OnGameLoad();
    }

    public void Resume()
    {
        player.GetComponent<Pause>().FlipTimeScale();
    }

    public void Return()
    {
        SceneManager.LoadScene(0);
    }

	public void Options()
	{
        if (mainMenu != null && options != null)
        {

            //Debug.Log("Options");
            mainMenu.SetActive(!mainMenu.activeSelf);
            options.SetActive(!options.activeSelf);
        }
	}

    public void LevelSelect()
    {
        levelSelect = !levelSelect;

        if (levelSelect == true)
        {
            //set buttons to active
            foreach(GameObject button in actionButtons)
            {
                button.SetActive(true);
            }
        }
        else
        {
            //set buttons to inactive
            foreach(GameObject button in actionButtons)
            {
                button.SetActive(false);
            }
        }
    }

    public void Balance()
    {
        SceneManager.LoadScene(3);
    }

    public void Climbing()
    {
        SceneManager.LoadScene(4);
    }

    public void Enemies()
    {
        SceneManager.LoadScene(5);
    }

    public void Flying()
    {
        SceneManager.LoadScene(6);
    }

    public void Gardening()
    {
        SceneManager.LoadScene(7);
    }

    public void Generation()
    {
        SceneManager.LoadScene(8);
    }

	//quits the game
	public void Quit()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        SaveSystem.Save(StoredData.current);

        Application.Quit();
#endif

    }

	public void MusicDown() {
        //Debug.Log("Hit");
        musicVolume --;
		musicMixer.audioMixer.SetFloat("MusicVolume", Mathf.Clamp(musicVolume, -80, 20));
		//blipSFX.PlaySoundAt(transform.position);
	}

	public void MusicUp() {

        //Debug.Log("Hit");
        musicVolume ++;
        musicMixer.audioMixer.SetFloat("MusicVolume", Mathf.Clamp(musicVolume, -80, 20));
		//blipSFX.PlaySoundAt(transform.position);
	}

	public void SFXDown() {

        //Debug.Log(soundVolume);
        soundVolume--;
        soundMixer.audioMixer.SetFloat("SoundVolume", Mathf.Clamp(soundVolume, -80, 20));
		//blipSFX.PlaySoundAt(transform.position);
	}

	public void SFXUp() {

        //Debug.Log(soundVolume);
        soundVolume++;
        soundMixer.audioMixer.SetFloat("SoundVolume", Mathf.Clamp(soundVolume, -80, 20));
		//blipSFX.PlaySoundAt(transform.position);
	}
}
