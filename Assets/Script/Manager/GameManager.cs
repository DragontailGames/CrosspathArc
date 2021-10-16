using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

/// <summary>
/// Game manager geral
/// </summary>
public class GameManager : MonoBehaviour
{
    public Tilemap tilemap;

    public Tilemap elevationTM, collisionTM;

    private CharacterController controller;

    public CinemachineVirtualCamera cinemachineVirtualCamera;

    public GameObject campfire;

    public List<CenarioEntity> cenarioEntities = new List<CenarioEntity>();

    public List<CenarioEntity> cenarioEntitiesMouseOn = new List<CenarioEntity>();

    private void Awake()
    {
        Manager.Instance.gameManager = this;

        if (!PlayerPrefs.HasKey("PLAYER"))
        {
            Manager.Instance.sceneLoadManager.GotoMenu();
        }
        else
        {
            if (PlayerPrefs.GetString("PLAYER") == "1")
            {
                GameObject main = GameObject.Find("Hero_1");
                Manager.Instance.characterController = main.GetComponent<CharacterController>();
                cinemachineVirtualCamera.Follow = main.transform;
                GameObject.Find("Hero_2")?.SetActive(false);
            }
            if (PlayerPrefs.GetString("PLAYER") == "2")
            {
                GameObject main = GameObject.Find("Hero_2");
                Manager.Instance.characterController = main.GetComponent<CharacterController>();
                cinemachineVirtualCamera.Follow = main.transform;
                GameObject.Find("Hero_1")?.SetActive(false);
            }
        }
        Manager.Instance.canvasManager = GameObject.Find("Canvas").GetComponent<CanvasManager>();
    }

    public Transform particleClick;

    private bool inPause = false;

    public List<CreatureController> creatures = new List<CreatureController>();

    public int currentCreatureIndex = 0;

    public GameObject player01;
    public GameObject player02;

    public int turnCount = 0;

    bool[,] tilesmap;
    PathFind.Grid grid;
    public PathFind.Grid Grid{get { return grid; }}

    public bool InPause { get => this.inPause; }

    int width;
    int height;

    void Start()
    {
        CalculateGrid();
        int indexPlayer = creatures.IndexOf(creatures.Find(n => n.GetType() == typeof(CharacterController)));
        if (indexPlayer == -1)
        {
            SceneManager.MoveGameObjectToScene(Manager.Instance.characterController.gameObject, SceneManager.GetActiveScene());
            SceneManager.MoveGameObjectToScene(Manager.Instance.canvasManager.gameObject, SceneManager.GetActiveScene());
            Manager.Instance.canvasManager.ReStart();
            Manager.Instance.characterController.Awake();
            indexPlayer = creatures.IndexOf(creatures.Find(n => n.GetType() == typeof(CharacterController)));
        }
        CreatureController firstCC = creatures[0];
        creatures[0] = creatures[indexPlayer];
        creatures[indexPlayer] = firstCC;
        
        TeleportManager.Instance.TestTeleport();
    }

    /// <summary>
    /// Carrega a particula na posição do mouse
    /// </summary>
    /// <param name="position">posição do mouse</param>
    public void MoveParticle(Vector2 position)
    {
        particleClick.transform.position = position;
        particleClick.GetComponent<ParticleSystem>().Play();
    }

    string lastDirection = "S";

    public string GetDirection(Vector3Int startIndex, Vector3Int destinationIndex)
    {
        Vector3Int index = MathfCustom.Sign(destinationIndex - startIndex);

        if (index == new Vector3Int(1, 1, 0)) lastDirection = "N";
        if (index == new Vector3Int(1, 0, 0)) lastDirection = "NE";
        if (index == new Vector3Int(1, -1, 0)) lastDirection = "E";
        if (index == new Vector3Int(0, -1, 0)) lastDirection = "SE";
        if (index == new Vector3Int(-1, -1, 0)) lastDirection = "S";
        if (index == new Vector3Int(-1, 0, 0)) lastDirection = "SW";
        if (index == new Vector3Int(-1, 1, 0)) lastDirection = "W";
        if (index == new Vector3Int(0, 1, 0)) lastDirection = "NW";

        return lastDirection;
    }

    public void EndMyTurn(CreatureController lastController)
    {
        currentCreatureIndex++;
        if (currentCreatureIndex >= creatures.Count)
        {
            currentCreatureIndex = 0;
        }

        CreatureController currentCreature = creatures[currentCreatureIndex];

        if (lastController != null)
        {
            lastController.myTurn = false;
        }

        if (currentCreature == null)
        {
            EndMyTurn(null);
            return;
        }

        if(currentCreature.Hp <= 0 )
        {
            EndMyTurn(null);
            return;
        }

        if (Manager.Instance.characterController.hp > 0)
        {
            StartCoroutine(currentCreature.StartMyTurn());
        }
    }

    private void CalculateGrid()
    {
        width = tilemap.size.x;
        height = tilemap.size.y;

        tilesmap = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                bool pathEnable = Manager.Instance.gameManager.HasAvailableTile(pos) ;
                tilesmap[x, y] = pathEnable;
            }
        }

        grid = new PathFind.Grid(width, height, tilesmap);
    }

    public List<PathFind.Point> GetPath(Vector3Int startIndex, Vector3Int destIndex)
    {
        PathFind.Point _from = new PathFind.Point(startIndex.x, startIndex.y);
        PathFind.Point _to = new PathFind.Point(destIndex.x, destIndex.y);

        return PathFind.Pathfinding.FindPath(grid, _from, _to);
    }

    public List<PathFind.Point> GetPathForLOS(Vector3Int startIndex, Vector3Int destIndex)
    {
        width = tilemap.size.x;
        height = tilemap.size.y;

        tilesmap = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                tilesmap[x, y] = tilemap.HasTile(pos);
            }
        }

        var newGrid = new PathFind.Grid(width, height, tilesmap);

        PathFind.Point _from = new PathFind.Point(startIndex.x, startIndex.y);
        PathFind.Point _to = new PathFind.Point(destIndex.x, destIndex.y);

        return PathFind.Pathfinding.FindPath(newGrid, _from, _to);
    }

    public List<PathFind.Point> GetPathWithCustom(Vector3Int startIndex, Vector3Int destIndex, CreatureController owner)
    {
        width = 40;
        height = 40;

        tilesmap = new bool[startIndex.x + width, startIndex.y + height];

        for (int x = Mathf.Clamp(startIndex.x - 20,0, startIndex.x + width); x < startIndex.x + 40; x++)
        {
            for (int y = Mathf.Clamp(startIndex.y - 20, 0, startIndex.y + width); y < startIndex.y + 40; y++)
            {
                try
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    tilesmap[x, y] = HasAvailableTile(pos) && Manager.Instance.gameManager.GetBotInTile(pos, owner) == null;
                }
                catch(System.Exception e)
                {
                    Debug.Log("Tilesmap na posição" + new Vector2(x, y) + " esta vazio por isso o erro: " + e);
                }
            }
        }

        var gridCustom = new PathFind.Grid(startIndex.x + width, startIndex.y + height, tilesmap);

        PathFind.Point _from = new PathFind.Point(startIndex.x, startIndex.y);
        PathFind.Point _to = new PathFind.Point(destIndex.x, destIndex.y);

        var path = PathFind.Pathfinding.FindPath(gridCustom, _from, _to);

        return path;
    }

    public List<PathFind.Point> GetPathWithExcept(Vector3Int startIndex, Vector3Int destIndex, CreatureController owner, List<Vector3Int> exceptList)
    {
        width = 40;
        height = 40;

        tilesmap = new bool[startIndex.x + width, startIndex.y + height];

        for (int x = Mathf.Clamp(startIndex.x - 20, 0, startIndex.x + width); x < startIndex.x + 40; x++)
        {
            for (int y = Mathf.Clamp(startIndex.y - 20, 0, startIndex.y + width); y < startIndex.y + 40; y++)
            {
                try
                {
                    bool exception = true;
                    Vector3Int pos = new Vector3Int(x, y, 0);

                    foreach (var aux in exceptList)
                    {
                        //Debug.Log("Testando " + aux + " - " + pos);
                        if (aux == pos)
                        {
                            exception = false;
                        }
                    }
                    tilesmap[x, y] = HasAvailableTile(pos) && Manager.Instance.gameManager.GetBotInTile(pos, owner) == null && exception;
                }
                catch (System.Exception e)
                {
                    Debug.Log("Tilesmap na posição" + new Vector2(x, y) + " esta vazio por isso o erro: " + e);
                }
            }
        }

        var gridCustom = new PathFind.Grid(startIndex.x + width, startIndex.y + height, tilesmap);

        PathFind.Point _from = new PathFind.Point(startIndex.x, startIndex.y);
        PathFind.Point _to = new PathFind.Point(destIndex.x, destIndex.y);

        var path = PathFind.Pathfinding.FindPath(gridCustom, _from, _to);

        return path;
    }

    public bool DetectLOS(List<PathFind.Point> path)
    {
        foreach (var aux in path)
        {
            Vector3Int tempPath = new Vector3Int(aux.x, aux.y, 0);
            if (elevationTM.HasTile(tempPath + new Vector3Int(1, 1, 0)) || collisionTM.HasTile(tempPath + new Vector3Int(1, 1, 0)))
            {
                return true;
            }
        }
        return false;
    }

    public void Rest()
    {
        Vector3Int startTile = Manager.Instance.characterController.CharacterMoveTileIsometric.controller.currentTileIndex;
        int radius = Manager.Instance.configManager.tilesWithoutEnemyForRest;

        bool canRest = true;

        for (int i = startTile.x - radius; i <= startTile.x + radius; i++)
        {
            for (int j = startTile.y - radius; j <= startTile.y + radius; j++)
            {
                Vector3Int checkTile = new Vector3Int(i, j, 0);
                if (tilemap.HasTile(checkTile))
                {
                    if(Manager.Instance.enemyManager.CheckEnemyInTile(checkTile))
                    {
                        Debug.Log("Não pode dormir inimigo em " + checkTile);
                        canRest = false;
                        break;
                    }
                }
            }
            if(!canRest)
            {
                break;
            }
        }
        if (canRest)
        {
            Manager.Instance.characterController.Rest();
        }
        else
        {
            Manager.Instance.canvasManager.LogMessage("Não pode dormir com inimigos proximos");
        }
    }

    public void StartNewTurn()
    {
        turnCount++;
        Manager.Instance.timeManager.StartNewTurn();
    }

    public int restCount = 0;

    public bool EndTurnRest()
    {
        int rand = Random.Range(0, 100);
        if (restCount * Manager.Instance.configManager.chanceToSpawnInRest > rand)
        {
            Manager.Instance.enemyManager.CreateEnemies();
            restCount = 0;
            return false;
        }
        restCount++;
        return true;
    }

    public BotController CheckHasBotInTile(Vector3Int tile)
    {
        List<BotController> bots = new List<BotController>();
        foreach(var aux in creatures)
        {
            BotController newBot;
            if(aux.TryGetComponent<BotController>(out newBot))
            {
                bots.Add(newBot);
            }
        }
        return bots.Find(n => n.currentTileIndex == tile && n.Hp>0);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            if (inPause == false)
            {
                Rest();
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (inPause == false)
            {
                Manager.Instance.canvasManager.spellbookManager.OpenSpellbook();
            }
            else
            {
                Manager.Instance.canvasManager.spellbookManager.Close();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && Manager.Instance.canvasManager.spellbookManager.isOpen)
        {
            Manager.Instance.canvasManager.spellbookManager.Close();
        }
    }

    public CreatureController GetCreatureInTile(Vector3Int tile)
    { 
        return creatures.Find(n => (n.currentTileIndex == tile || (n.botMultipleTile != null && n.botMultipleTile.HasBotInTile(tile))) && n.Hp > 0);
    }

    public CreatureController GetBotInTile(Vector3Int tile, CreatureController except)
    {
        return creatures.Find(n => (n.currentTileIndex == tile || (n.botMultipleTile != null && n.botMultipleTile.HasBotInTile(tile))) && n.Hp > 0 && n.GetComponent<BotController>() && n != except);
    }

    public bool HasAvailableTile(Vector3Int dest)
    {
        return !elevationTM.HasTile(dest + new Vector3Int(1, 1, 0)) &&
                  (!collisionTM.HasTile(dest + new Vector3Int(1, 1, 0))) &&
                  (tilemap.HasTile(dest) &&
                  cenarioEntities.Find(n => n.currentTileIndex == dest && n.tileBlock == true) == null);
    }

    public void SetupPause(bool _inPause)
    {
        StartCoroutine(PauseDelay(_inPause));
    }

    private IEnumerator PauseDelay(bool _inPause)
    {
        yield return new WaitForSeconds(0.2f);
        this.inPause = _inPause;
    }
}
