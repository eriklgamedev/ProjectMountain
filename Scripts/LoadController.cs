using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadController : MonoBehaviour
{
    [SerializeField]
    private GameManager manager;

    [SerializeField]
    private Player player;

    public void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void OnGameLoad()
    {
        StoredData.current = (StoredData)SaveSystem.Load(Application.persistentDataPath + "/saves/Data.save");

        manager.flowerCount = StoredData.current.flowerCount;
        manager.flowersRequired = StoredData.current.flowersRequired;
        player.transform.position = StoredData.current.spawnPosition;
        player.transform.rotation = StoredData.current.spawnRotation;
    }
}
