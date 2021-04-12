using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

/// <summary>
/// Controlador geral do jogador para fazer relação entre as classes
/// </summary>
public class CharacterController : MonoBehaviour
{
    #region SetupCharacter
    CharacterCombat characterCombat;

    CharacterInterface characterInterface;

    CharacterInventory characterInventory;

    CharacterMoveTileIsometric characterMoveTileIsometric;

    CharacterStatus characterStatus;

    public CharacterCombat CharacterCombat { get => this.characterCombat; set => this.characterCombat = value; }
    public CharacterInterface CharacterInterface { get => this.characterInterface; set => this.characterInterface = value; }
    public CharacterInventory CharacterInventory { get => this.characterInventory; set => this.characterInventory = value; }
    public CharacterMoveTileIsometric CharacterMoveTileIsometric { get => this.characterMoveTileIsometric; set => this.characterMoveTileIsometric = value; }
    public CharacterStatus CharacterStatus { get => this.characterStatus; set => this.characterStatus = value; }
    public Animator Animator { get => this.animator; set => this.animator = value; }

    #endregion

    private GameManager gameManager;
    private EnemyManager enemyManager;
    private Animator animator;

    internal string direction;
    public readonly string animationName = "Male_Archer";

    public bool myTurn = true;
    private bool delay = false;

    public void Awake()
    {
        Manager.Instance.characterController = this;

        characterCombat = this.GetComponent<CharacterCombat>();
        characterInterface = this.GetComponent<CharacterInterface>();
        characterInventory = this.GetComponent<CharacterInventory>();
        characterMoveTileIsometric = this.GetComponent<CharacterMoveTileIsometric>();
        characterStatus = this.GetComponent<CharacterStatus>();

        Animator = this.GetComponentInChildren<Animator>();
        animator.speed = 0.7f;

        gameManager = Manager.Instance.gameManager;
        enemyManager = Manager.Instance.enemyManager;

        gameManager.creatures.Add(this.gameObject);
    }

    public void Update()
    {
        if (Input.GetMouseButton(0) && !gameManager.InPause && myTurn && !delay)//Detecta o click do jogador
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Vector3Int mousePos = MousePosition();

            EnemyController enemyInTile = enemyManager.CheckEnemyInTile(mousePos);

            if (enemyInTile != null && enemyInTile.enemy.hp>0)
            {
                StartCoroutine(StartDelay());
                characterCombat.TryHit(enemyInTile, mousePos, characterMoveTileIsometric.CurrentTileIndex);
            }
            else
            {
                characterMoveTileIsometric.Mouse = CharacterMousePosition(mousePos);
            }
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            gameManager.EndMyTurn(this);
        }
    }

    public IEnumerator StartMyTurn()
    {
        yield return new WaitForSeconds(0.1f);
        myTurn = true;
    }

    /// <summary>
    /// Função que retorna a posição do mouse corrigida
    /// </summary>
    public Vector3Int MousePosition()
    {
        Vector2 pos = Input.mousePosition;//Detecta a posição do mouse
        pos = Camera.main.ScreenToWorldPoint(pos);//Converte a posição com relação a camera
        Vector3Int gridPos = gameManager.tilemap.WorldToCell(pos);//pega o index do tile que foi clicado
        gameManager.MoveParticle(pos);//Efeito para identificar o click
        return gridPos;
    }

    public Vector3Int CharacterMousePosition(Vector3Int pos)
    {
        pos -= characterMoveTileIsometric.CurrentTileIndex;

        return MathfCustom.Sign(pos);//Retorna o sinal do valor(-1,+1 ou 0)
    }

    public string GetDirection(Vector3Int index)
    {
        if (index == new Vector3Int(1, 1, 0)) return "N";
        if (index == new Vector3Int(1, 0, 0)) return "NE";
        if (index == new Vector3Int(1, -1, 0)) return "E";
        if (index == new Vector3Int(0, -1, 0)) return "SE";
        if (index == new Vector3Int(-1, -1, 0)) return "S";
        if (index == new Vector3Int(-1, 0, 0)) return "SW";
        if (index == new Vector3Int(-1, 1, 0)) return "W";
        if (index == new Vector3Int(0, 1, 0)) return "NW";

        return "DirectionWrong";
    }

    public IEnumerator StartDelay()
    {
        delay = true;
        yield return new WaitForSeconds(0.2f);
        delay = false;
    }
}
