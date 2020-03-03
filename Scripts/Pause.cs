using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class Pause : MonoBehaviour
{
    public GameObject menu, menuCentre;
    private Rigidbody playerRigidbody;
    private bool isMainMenu = false, isPaused = false;
    public bool playerIsActive = true;
    [SerializeField]
    private GameObject uiHelper;

    [SerializeField]
    private AudioMixerGroup soundMixer, musicMixer;

    [Range(-80, 20)]
    private int soundVolume = 0, musicVolume = 0;
    //

    public void Start()
    {
        playerRigidbody = transform.root.GetComponent<Rigidbody>();
    }

    public void Update()
    {
        ///Debug.Log(Time.timeScale);
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("MainMenu"))
        {
            if (OVRInput.GetUp(OVRInput.Button.Start))
            {
                menu.SetActive(!menu.activeSelf);
                uiHelper.SetActive(!uiHelper.activeSelf);
            }
        }

        if (menu.activeSelf == true)
        {
            if (playerIsActive == true)
            {
                isPaused = false;
                Time.timeScale = 0f;
                playerRigidbody.constraints = RigidbodyConstraints.FreezeAll;
                //Vector3 euler = playerRigidbody.transform.eulerAngles;
                //menuCentre.transform.eulerAngles = new Vector3(0, euler.y, 0);
                playerIsActive = false;
            }

        }
        else
        {
            if (playerIsActive == false)
            {
                PlayerReactivate();
            }
        }

        if (isPaused == true)
        {
            Time.timeScale = 1f;
            menu.SetActive(false);
            uiHelper.SetActive(false);
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

            playerIsActive = true;
            isPaused = false;
        }
    }

    public void PlayerReactivate()
    {
        Time.timeScale = 1f;

        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

        playerIsActive = true;
    }

    public void FlipTimeScale()
    {
        isPaused = true;
    }

    public void MusicDown() {

        musicVolume--;
        musicMixer.audioMixer.SetFloat("MusicVolume", Mathf.Clamp(musicVolume, -80, 20));
    }

    public void MusicUp()
    {
        
        musicVolume++;
        musicMixer.audioMixer.SetFloat("MusicVolume", Mathf.Clamp(musicVolume, -80, 20));
        //blipSFX.PlaySoundAt(transform.position);
    }

    public void SFXDown()
    {
        
        soundVolume--;
        soundMixer.audioMixer.SetFloat("SoundVolume", Mathf.Clamp(soundVolume, -80, 20));
        //blipSFX.PlaySoundAt(transform.position);
    }

    public void SFXUp()
    {
        
        soundVolume++;
        soundMixer.audioMixer.SetFloat("SoundVolume", Mathf.Clamp(soundVolume, -80, 20));
        //blipSFX.PlaySoundAt(transform.position);
    }
}
