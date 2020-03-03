using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeEnemy : StationaryEnemy
{
    public override void SetLocation(Vector3 newlocation)
    {
        //Overriding SetLocation in order to prevent this enemy from moving
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
