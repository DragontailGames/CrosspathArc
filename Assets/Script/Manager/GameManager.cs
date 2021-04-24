using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Game manager geral
/// </summary>
public class GameManager : MonoBehaviour
{
    public Tilemap tilemap;

    public Tilemap elevationTM, collisionTM;

    private CharacterController controller;

    public CinemachineVirtualCamera cinemachineVirtualCamera;

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
                GameObject.Find("Hero_2").SetActive(false);
            }
            if (PlayerPrefs.GetString("PLAYER") == "2")
            {
                GameObject main = GameObject.Find("Hero_2");
                Manager.Instance.characterController = main.GetComponent<CharacterController>();
                cinemachineVirtualCamera.Follow = main.transform;
                GameObject.Find("Hero_1").SetActive(false);
            }
        }
    }

    public Transform particleClick;

    private bool inPause = false;

    public List<GameObject> creatures = new List<GameObject>();

    public int currenteCreature = 0;
    public GameObject player01;
    public GameObject player02;


    bool[,] tilesmap;
    PathFind.Grid grid;
    public PathFind.Grid Grid{get { return grid; }}
    int width;
    int height;

    void Start()
    {
        CalculateGrid();
    }

    public bool InPause { get => this.inPause; set => this.inPause = value; }//pausa o jogo quando abrir o menu

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

    public void EndMyTurn(CharacterController cController = null)
    {
        currenteCreature++;
        if (currenteCreature >= creatures.Count)
        {
            currenteCreature = 0;
        }

        if(cController != null)
            cController.myTurn = false;

        if (creatures[currenteCreature] == null)
            EndMyTurn();

        if (creatures[currenteCreature].GetComponent<CharacterController>())
        {
            CharacterController playerController = creatures[currenteCreature].GetComponent<CharacterController>();

            if (playerController.CharacterStatus.Hp > 0)
                StartCoroutine(playerController.StartMyTurn());
        }
        else
        {
            EnemyController enemyController = creatures[currenteCreature].GetComponent<EnemyController>();

            if(enemyController.enemy.hp > 0)
                StartCoroutine(enemyController.StartMyTurn());
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
                bool pathEnable = tilemap.HasTile(pos);
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
}
