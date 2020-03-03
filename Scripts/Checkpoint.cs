using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Transform spawnPointTransform;
    [SerializeField]
    public GameObject player;

    public GameManager manager;

    // Start is called before the first frame update
    void Start()
    {
        spawnPointTransform = transform.GetChild(0);
        //Debug.Log(transform.GetChild(0).name);

        //Debug.Log(spawnPointTransform.localPosition);
    }

    public void Activate()
    {
        player.GetComponent<Player>().spawnPointPosition = spawnPointTransform.position;
        player.GetComponent<Player>().spawnPointRotation = spawnPointTransform.rotation;

        StoredData.current.spawnPosition = spawnPointTransform.position;
        StoredData.current.spawnRotation = spawnPointTransform.rotation;
        StoredData.current.flowersRequired = manager.flowersRequired;
        StoredData.current.flowerCount = manager.flowerCount;
        StoredData.current.level = manager.sceneIndex;

        Debug.LogError(StoredData.current.level);

        SaveSystem.Save(StoredData.current);

        Debug.Log("Checkpoint Activated!");
    }
}
