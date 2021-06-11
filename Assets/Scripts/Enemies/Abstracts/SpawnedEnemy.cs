using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedEnemy : OptimizedMonoBehaviour
{
    private RandomEnemySpawner enemySpawner;
    private int spawnerIndex;
    
    public void SetSpawner(RandomEnemySpawner toSet, int index)
    {
        enemySpawner = toSet;
        spawnerIndex = index;
    }

    private void OnDestroy()
    {
        if (!isQuitting)
        {
            try
            {
                if (enemySpawner != null)
                    enemySpawner.Destroyed(spawnerIndex);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Debug.Log("Wrong index " + spawnerIndex);
            }
        }
    }
}
