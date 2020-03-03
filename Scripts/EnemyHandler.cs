using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    public EnemyType enemyType;

    public EnemyData enemyData;

    public void Start()
    {
        enemyData.enemyType = enemyType;
        StoredData.current.enemies.Add(enemyData);

        enemyData.position = transform.position;
        enemyData.rotation = transform.rotation;
    }

    private void Update()
    {
        enemyData.position = transform.position;
        enemyData.rotation = transform.rotation;
    }

    void DestroyMe()
    {
        Destroy(gameObject);
    }
}
