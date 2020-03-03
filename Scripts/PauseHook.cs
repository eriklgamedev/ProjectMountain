using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseHook : MonoBehaviour
{
    [SerializeField]
    private GameObject pointer, menu, handle;
        

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (transform.root.GetComponent<Pause>().playerIsActive == true) {
            
            menu.SetActive(false);
            handle.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

            //handle.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name != "Player")
        {
            //pointer.transform.root.GetComponent<Pause>().PlayerPause();
            pointer.SetActive(true);
            menu.SetActive(true);
            handle.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}
