using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GardenArea : MonoBehaviour
{
    public int flowerCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.transform.root.name == "Player") {

            //Debug.Log("Player ");
            other.transform.root.GetComponent<PlayerMovement>().seedShooterActive = true;
        }

        if (other.transform.root.tag == "Seed")
        {
            flowerCount++;
            //Debug.Log("FlowerCount: " + flowerCount);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.name == "Player") {

            other.transform.root.GetComponent<PlayerMovement>().seedShooterActive = false;
        }
        if (other.transform.root.tag == "Seed") {
            Destroy(other.gameObject);
            flowerCount--;
        }
    }
}
