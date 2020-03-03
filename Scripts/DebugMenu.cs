using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DebugMenu : MonoBehaviour
{
    [SerializeField]
    private Text immortalityText, riskText, checkpointText;
    public bool canDebug = false, canDie = true, canFall = true;
    private GameObject player;
    [SerializeField]
    private GameObject[] mountains, checkpoints;
    private int i = 0;

    // Start is called before the first frame update
    void Start()
    {
        player = transform.root.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (canDebug == true)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0)) //Makes player immortal
            {
                canDie = !canDie;
                ChangeColour(canDie, "immortality");
            }

            if (Input.GetKeyDown(KeyCode.Alpha1)) //Turns off Risk System
            {
                canFall = !canFall;
                ChangeColour(canFall, "risk");
            }

            if (Input.GetKeyDown(KeyCode.Alpha2)) //Damages player
            {
                player.GetComponent<Player>().Damage();
            }

            if (Input.GetKeyDown(KeyCode.Alpha3)) //Kills player
            {
                player.GetComponent<Player>().Death();
            }

            if (Input.GetKeyDown(KeyCode.Alpha5)) //Teleports to Next Checkpoint
            {
                if (checkpoints.Length > 0)
                {
                    if (i < checkpoints.Length - 1)
                    {
                        TeleportPlayer();
                        i++;
                    }
                    else if (i >= checkpoints.Length - 1)
                    {
                        TeleportPlayer();
                        i = 0;
                    }
                }
            }
            else if (checkpoints.Length <= 0)
            {
                checkpointText.color = Color.red;
            }

            if (Input.GetKeyDown(KeyCode.Alpha6)) //Regenerates mountains and teleport to last checkpoint
            {
                player.GetComponent<Player>().Death();
                Invoke("RegenerateMountain", player.transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<OVRScreenFade>().fadeTime);
            }

            if (Input.GetKeyDown(KeyCode.Alpha7)) //Resets Level
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    void TeleportPlayer()
    {
        //player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = checkpoints[i].transform.position+ new Vector3(0, 2, 0);
        player.transform.rotation = checkpoints[i].transform.rotation;
        //player.GetComponent<CharacterController>().enabled = true;
    }

    void RegenerateMountain()
    {
        foreach (GameObject mountain in mountains)
        {
            mountain.GetComponent<Mountain>().GenerateSeed();
        }
    }

    void ChangeColour(bool canFunction, string function)
    {
        if (function == "immortality")
        {
            if (canFunction == false)
            {
                immortalityText.color = Color.green;
            }
            else
            {
                immortalityText.color = Color.white;
            }
        }
        else if (function == "risk")
        {
            if (canFunction == false)
            {
                riskText.color = Color.green;
            }
            else
            {
                riskText.color = Color.white;
            }
        }
    }
}
