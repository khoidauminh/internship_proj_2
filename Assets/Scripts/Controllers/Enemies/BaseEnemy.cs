using UnityEngine;

public class BaseEnemy : BaseUnitController
{
    void Start()
    {
        EnemyManager enemyManager = FindAnyObjectByType<EnemyManager>();

        if (enemyManager == null)
        {
            return;
        }

        Debug.Log("Found Enemey Manager");

        enemyManager.Adopt(this);
    }
}
