using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenu, credits;
    [SerializeField]
    private string sceneName;

    public void StartGame()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void Credits()
    {
        mainMenu.SetActive(!mainMenu.activeSelf);
        credits.SetActive(!credits.activeSelf);
    }

    //quits the game
    public void Quit()
    {
        Application.Quit();
    }
}
