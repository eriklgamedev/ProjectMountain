using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoredData
{
    private static StoredData _current;
    public static StoredData current
    {
        get
        {
            if (_current == null)
            {
                _current = new StoredData();
            }
            return _current;
        }
        set
        {
            if(value != null)
            {
                _current = value;
            }
        }
    }

    public List<EnemyData> enemies;

    #region Game Manager Data
    public int level;
    public int flowerCount;
    public int flowersRequired;

    #endregion

    #region Player Data
    public Vector3 spawnPosition;
    public Quaternion spawnRotation;

    #endregion  
}
