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

    public List<GameObject> enemiesCanBeCreated = new List<GameObject>(); 

    public EnemyController CheckEnemyInTile(Vector3Int tile)
    {
        return enemies.Find(n => n.currentTileIndex == tile && n.Hp > 0);
    }

    public Vector3Int MaxRangePos(Enemy enemy)
    {
        return Vector3Int.one * enemy.size;
    }

    public void CreateEnemies()
    {
        int enemiesCount = Random.Range(2, 5);
        List<Vector3Int> createEnemies = new List<Vector3Int>();
        for (int i = 0; i < enemiesCount; i++)
        {
            Vector3Int characterIndex = Manager.Instance.characterController.CharacterMoveTileIsometric.CurrentTileIndex;
            int min = Manager.Instance.configManager.minRangeRestEnemySpawn;
            int max = Manager.Instance.configManager.maxRangeRestEnemySpawn;
            Vector3Int spawnPos = new Vector3Int(0,0,0);

            spawnPos.x = Random.value > 0.5f ?
                Random.Range(-min, -max) :
                Random.Range(min, max);
            spawnPos.y = Random.value > 0.5f ?
                Random.Range(-min, -max) :
                Random.Range(min, max);

            spawnPos += characterIndex;

            if (Manager.Instance.gameManager.tilemap .HasTile(spawnPos) &&
                !Manager.Instance.gameManager.collisionTM.HasTile(spawnPos) && 
                !Manager.Instance.gameManager.elevationTM.HasTile(spawnPos) &&
                !CheckEnemyInTile(spawnPos) && createEnemies.Find(n => n==spawnPos) != null)
            {
                GameObject enemie = Instantiate(enemiesCanBeCreated[Random.Range(0, enemiesCanBeCreated.Count)], Manager.Instance.gameManager.tilemap.GetCellCenterLocal(spawnPos) + new Vector3(0, 0, 0.5f), Quaternion.identity);
                createEnemies.Add(spawnPos);
            }
            else
            {
                i--;
            }
        }
    }
}
