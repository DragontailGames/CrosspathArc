using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigManager : MonoBehaviour
{
    private void Awake()
    {
        Manager.Instance.configManager = this;
    }

    public int tilesWithoutEnemyForRest = 40;

    public int dayTurns = 40;

    public int nigthTurns = 30;

    public int healthRestPerTurn = 5;

    public int manaRestPerTurn = 5;

    public int minRangeRestEnemySpawn = 2;

    public int maxRangeRestEnemySpawn = 4;

    public int chanceToSpawnInRest = 20;
}
