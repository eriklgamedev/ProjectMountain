using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChecker : MonoBehaviour
{
    public Mountain currentMountain, previousMountain;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.gameObject.name == "Player")
        {
            currentMountain.ActivateEnemiesForEachFace();
            currentMountain.ActivateCirclesForEachFace();

            if (previousMountain != null) {
                previousMountain.DeactivateEnemiesForEachFace();
                previousMountain.DeactivateCirclesForEachFace();
            }
        }
    }
}
