using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public void Awake()
    {
        Manager.Instance.enemyManager = this;
    }

    public List<EnemyController> enemies = new List<EnemyController>();

    public EnemyController CheckEnemyInTile(Vector3Int tile)
    {
        return enemies.Find(n => n.enemy.tilePos == tile);
    }

    public Vector3Int MaxRangePos(Enemy enemy)
    {
        return Vector3Int.one * enemy.size;
    }
}
