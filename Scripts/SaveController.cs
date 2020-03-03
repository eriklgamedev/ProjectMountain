using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveController : MonoBehaviour
{
    public GameObject Continue;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            if (SaveSystem.hasFile == true)
            {
                Continue.SetActive(true);
            }
            else
            {
                Continue.SetActive(false);
            }
        }
    }
}
