using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

public class StationaryEnemy : MonoBehaviour
{
    [HideInInspector]
    public Animator anim = null;

    [HideInInspector]
    public GameObject player;

    [HideInInspector]
    public Rigidbody rb = null;
    
    public int health = 3;
    [SerializeField]
    private Renderer body;

	public SoundFXRef attackSFX;
	public SoundFXRef damagedSFX;
	public SoundFXRef idleSFX;

	// Start is called before the first frame update
	void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody>();

        //idleSFX.PlaySoundAt(transform.position);

        InvokeRepeating("GetPlayer", 0, 0.5f);
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

    // Function that sets the calls SetLocation if player
    public void GetPlayer()
    {
        if (player != null)
        {
            SetLocation(player.transform.position);
        }
    }

    // Function that sets the new location of the base simulating the enemy moving towards the player
    public virtual void SetLocation(Vector3 newlocation)
    {
        Vector3 targetPostition = new Vector3(newlocation.x, transform.localPosition.y, newlocation.z);
        transform.LookAt(targetPostition);
    }
    
    // Function that makes the enemy take damage 
    public void TakeDamage()
    {
		health -= 1;
        damagedSFX.PlaySoundAt(transform.position);
        

        if (health <= 0)
        {
            Death();
        }
    }
    void ChangeColor() {
        MaterialPropertyBlock colorBlock = new MaterialPropertyBlock();
        colorBlock.SetColor("_BaseColor", Color.green);
        body.SetPropertyBlock(colorBlock);
    }

    // Function that sets the Enemy's Death bool active and calls DeactivateSelf function
    public void Death()
    {
        anim.SetBool("Death", true);

        Invoke("DeactivateSelf", 0.5f);
    }

    // Function that deactivates the enemy upon death
    public void DeactivateSelf() {
        gameObject.SetActive(false);
    }

    //Function that players the attack sound
    public void PlayAttackSound()
    {
        attackSFX.PlaySoundAt(new Vector3(transform.position.x, transform.position.y, transform.position.z));
    }
}
