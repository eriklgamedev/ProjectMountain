using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    // variable that contols the components of the EnemyProjectile
    public GameObject bullet;
    public Rigidbody rb;
    public float projectileSpeed = 5.0f;

    void Start()
    {
        // Sets the variables as the objects components 
        bullet = this.gameObject;
        rb = gameObject.GetComponent<Rigidbody>();

        rb.AddForce(-bullet.transform.forward * projectileSpeed, ForceMode.Impulse);
        Invoke("Despawn",3.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Checks if the bullet hits the player
        if(other.gameObject.name == "Player")
        {
            Despawn();
        }

        // checks if the bullet hits any object with a Enemy tag
        if (other.gameObject.tag == "Enemy")
        {
            // does nothing if the bullet collides with an enemy object
        }
        else
        {
            // despawns the bullet if the bullet hits anything else
            Despawn();
        }
    }

    // Function that controls the bullets 
    private void Despawn()
    {
        Destroy(bullet);
    }

}
