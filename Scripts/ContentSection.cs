using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentSection : MonoBehaviour
{
    [HideInInspector]
    public Player player;
    private float maxRange = 80f;
    [HideInInspector]
    public bool isEnemy, isCircle;
    private bool isActive;
    private Vector3 centerPoint;
    private Renderer renderer;
	// Start is called before the first frame update
	void Start()
    {
        renderer = transform.parent.GetComponent<Renderer>();

        if (isEnemy == true)
        {
            maxRange = 40f;

            InvokeRepeating("CheckDistance", 1, 5f);
        }
        else if (isCircle == true)
        {
            maxRange = 35f;

            InvokeRepeating("CheckDistance", 1, 3f);
        }
        else {
            maxRange = 80f;

            InvokeRepeating("CheckDistance", 1, 10f);
        }
        
        centerPoint = calculateCentroid();
        //InvokeRepeating("CheckEnemyDeactivation", 0, 1f);
    }

    void CheckDistance() {
        //Debug.Log(centerPoint);
        if (player != null) {
            if (player.deactivateEnemies == false || isEnemy == false)
            {
                ActivateAndDeactivate();
            }
            else if (player.deactivateCircles == false || isCircle == false)
            {
                ActivateAndDeactivate();
            }
            else {
                ActivateAndDeactivate();
            }
        }
        
        
    }

    void ActivateAndDeactivate() {
        if (Vector3.Distance(centerPoint, player.gameObject.transform.position) <= maxRange)
        {
            //Debug.Log(Vector3.Distance(renderer.bounds.center, player.gameObject.transform.position));
            if (isActive != true)
            {
                ActivateChilds();
            }
        }
        else
        {
            if (isActive != false)
            {
                DeactivateChilds();
            }
        }
    }

    Vector3 calculateCentroid()
    {
       
            Vector3 centroid = Vector3.zero;

            if (transform.childCount > 0)
            {
                foreach (Transform child in transform)
                {
                    centroid += child.transform.position;
                }
                centroid /= (transform.childCount);
            }
        return centroid;
        
    }

    void CheckEnemyDeactivation() {
        if (player != null) {
            if (player.deactivateEnemies == true && isEnemy == true)
            {
                if (isActive != false)
                {
                    DeactivateChilds();
                }
            }
            if (player.deactivateCircles == true && isCircle == true)
            {
                if (isActive != false)
                {
                    DeactivateChilds();
                }
            }
            if (player.deactivateCircles == false && isCircle == true)
            {
                if (isActive != false)
                {
                    ActivateChilds();
                }
            }
        }
        
    }

    private void ActivateChilds() {
        foreach (Transform child in transform) {
            child.gameObject.SetActive(true);
        }
        isActive = true;
    }

    private void DeactivateChilds() {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        isActive = false;
    }


}
