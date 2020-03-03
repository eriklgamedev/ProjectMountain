using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleActivationZone : MonoBehaviour
{
    

    private float maxRange = 50f;
    private void Update()
    {
        if (GetComponent<SphereCollider>().radius < maxRange)
        {
            GetComponent<SphereCollider>().radius += 3.3f;
        }
        else if(GetComponent<SphereCollider>().radius >= maxRange) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.tag);
        if (other.tag == "Circle") {
            other.GetComponent<SpriteRenderer>().enabled = true;
            other.GetComponent<TerrainCircle>().StartDeactivation();

        }
    }
}
