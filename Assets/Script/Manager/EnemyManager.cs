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

    public Vector3Int MaxRangePos(EnemyController enemy)
    {
        return Vector3Int.one * enemy.size;
    }

    public void CreateEnemies()
    {
        int enemiesCount = Random.Range(2, 5);
        List<Vector3Int> createEnemies = new List<Vector3Int>();

        int min = Manager.Instance.configManager.minRangeRestEnemySpawn;
        int max = Manager.Instance.configManager.maxRangeRestEnemySpawn;
        List<Vector2Int> positions = new List<Vector2Int>();
        Vector3Int characterIndex = Manager.Instance.characterController.CharacterMoveTileIsometric.controller.currentTileIndex;

        for(int x = 0; x <= max; x++)
        {
            for (int y = 0; y <= max; y++)
            {
                if (Vector3Int.Distance(Vector3Int.zero, new Vector3Int(x, y, 0)) > 2)
                {
                    positions.Add(new Vector2Int(x, y));
                }
                if (Vector3Int.Distance(Vector3Int.zero, new Vector3Int(-x, y, 0)) > 2)
                {
                    positions.Add(new Vector2Int(-x, y));
                }
                if (Vector3Int.Distance(Vector3Int.zero, new Vector3Int(x, -y, 0)) > 2)
                {
                    positions.Add(new Vector2Int(x, -y));
                }
                if (Vector3Int.Distance(Vector3Int.zero, new Vector3Int(-x, -y, 0)) > 2)
                {
                    positions.Add(new Vector2Int(-x, -y));
                }
            }
        }

        int tryLimit = 200;
        int limitTotal = 100;
        List<int> triedIndex = new List<int>();
        for (int i = 0; i < enemiesCount; i++)
        {
            List<Vector2Int> auxPosition = new List<Vector2Int>();
            foreach (var temp in positions)
            {
                if (!triedIndex.Contains(positions.IndexOf(temp)))
                {
                    auxPosition.Add(temp);
                }
            }
            int posIndex = Random.Range(0, auxPosition.Count);
            triedIndex.Add(posIndex);

            Vector3Int spawnPos = new Vector3Int(
                characterIndex.x + auxPosition[posIndex].x,
                characterIndex.y + auxPosition[posIndex].y,
                characterIndex.z);

            if (TestCanCreate(spawnPos, characterIndex))
            {
                Instantiate(enemiesCanBeCreated[Random.Range(0, enemiesCanBeCreated.Count)], Manager.Instance.gameManager.tilemap.GetCellCenterLocal(spawnPos) + new Vector3(0, 0.5f, 0.5f), Quaternion.identity);
                triedIndex = new List<int>();
            }
            else if (tryLimit>0)
            {
                tryLimit--;
                i--;
            }
            else if(limitTotal>0)
            {
                limitTotal--;
                Debug.Log("Limite de tentativas de spawn de lobos fantasma deixando mais proximo");
                positions.Clear();
                triedIndex = new List<int>();
                for (int x = 0; x <= max; x++)
                {
                    for (int y = 0; y <= max; y++)
                    {
                        positions.Add(new Vector2Int(x, y));
                        positions.Add(new Vector2Int(-x, y));
                        positions.Add(new Vector2Int(x, -y));
                        positions.Add(new Vector2Int(-x, -y));
                    }
                }

                i--;
            }
        }
    }

    public bool TestCanCreate(Vector3Int spawnPos, Vector3Int characterIndex)
    {
        GameManager gameManager = Manager.Instance.gameManager;
        List<PathFind.Point> path = Manager.Instance.gameManager.GetPath(spawnPos, characterIndex);
        bool returnValue = false;

        if(gameManager.tilemap.HasTile(spawnPos))
        {
            returnValue = true;
        }
        else
        {
            return false;
        }
        if(!gameManager.collisionTM.HasTile(spawnPos + new Vector3Int(1, 1, 0)))
        {
            returnValue = true;
        }
        else
        {
            return false;
        }
        if (!gameManager.elevationTM.HasTile(spawnPos + new Vector3Int(1, 1, 0)))
        {
            returnValue = true;
        }
        else
        {
            return false;
        }
        if (gameManager.GetCreatureInTile(spawnPos) == null)
        {
            returnValue = true;
        }
        else
        {
            return false;
        }
        if(path.Count>0)
        {
            returnValue = true;
        }
        else
        {
            return false;
        }

        return returnValue;
    }
}
