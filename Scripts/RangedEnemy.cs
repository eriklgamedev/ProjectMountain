using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : StationaryEnemy
{
    // Variables that control the projectile and their spawn point
    public GameObject projectile = null;
    public GameObject projectilePoint  = null;

    // Variables that control the Ranged enemies movements 
    public GameObject BaseJoint = null;
    public GameObject BodyJoint = null;
    public GameObject Head = null;

    void Start()
    {
        // sets the animator and rigidbody in the script to the prefabs components
        anim = gameObject.GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody>();

        // tracks the player every 0.1 second
        InvokeRepeating("GetPlayer", 0, 0.1f);
    }

    // function that controls the projectile of the ranged enemey
    private void ShootProjectile()
    {
        Instantiate(projectile, projectilePoint.transform.position, projectilePoint.transform.rotation);
    }

    // Overiding parent function that adjusts 3 object's transform instead of one
    public override void SetLocation(Vector3 newlocation)
    {
        Vector3 BasePosition = new Vector3(newlocation.x, -newlocation.y, newlocation.z);
        BaseJoint.transform.LookAt(BasePosition);

        Vector3 BodyPostition = new Vector3(-newlocation.x, newlocation.y, -newlocation.z);
        BodyJoint.transform.LookAt(BodyPostition);

        Vector3 headPosition = new Vector3(newlocation.x, newlocation.y, newlocation.z);
        Head.transform.LookAt(headPosition);
    }


    private void OnTriggerEnter(Collider other)
    {
        // Detection for the players body entering the trigger volume
        if (other.gameObject.name == "Body")
        {
            // sets the player variable to the player & sets the attack bool true
            player = other.gameObject;
            anim.SetBool("Attack", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Detection for the players body exiting the trigger volume & sets the attack bool false
        if (other.gameObject.name == "Body")
        {
            // sets the player variable to null
            player = null;
            anim.SetBool("Attack", false);
        }
    }

}
