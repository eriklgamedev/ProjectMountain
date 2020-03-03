using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathVolume : MonoBehaviour
{
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
        if (other.name == "Body")
        {
            other.transform.root.GetComponent<Player>().Death();
            //other.transform.root.GetComponent<PlayerMovement>().ch
        }
    }
}
