using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedEnemy : MonoBehaviour
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
        enemySpawner.Destroyed(spawnerIndex);
    }
}
