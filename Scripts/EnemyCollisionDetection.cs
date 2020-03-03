using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisionDetection : MonoBehaviour
{
    [SerializeField]
    private GameObject debugMenu;
    //The purpose of this script is to check that the head of the plant collides with the player, causing damage to the player.

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (debugMenu.GetComponent<DebugMenu>().canDie == true)
        {
            if (other.gameObject.name == "Body")
            {
                Player player = other.transform.root.GetComponent<Player>();

                //other.GetComponent<Rigidbody>().velocity = other.GetComponent<Rigidbody>().velocity * 0.1f;
                player.Damage();

            }
        }
        else
        {
            Debug.Log("Player is Immortal");
        }
    }
    
}
