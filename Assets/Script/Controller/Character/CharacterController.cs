using System;
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
    public string animationName = "";

    public bool myTurn = true;
    private bool delay = false;

    public bool isRest = false;

    public void Awake()
    {
        Manager.Instance.characterController = this;

        characterCombat = this.GetComponent<CharacterCombat>();
        characterInterface = this.GetComponent<CharacterInterface>();
        characterInventory = this.GetComponent<CharacterInventory>();
        characterMoveTileIsometric = this.GetComponent<CharacterMoveTileIsometric>();
        characterStatus = this.GetComponent<CharacterStatus>();

        Animator = this.transform.GetComponentInChildren<Animator>();
        animator.speed = 0.6f;

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

            if (enemyInTile != null && enemyInTile.hp>0)
            {
                StartCoroutine(StartDelay());
                characterCombat.TryHit(enemyInTile, mousePos, characterMoveTileIsometric.CurrentTileIndex);
            }
            else if(characterCombat.selectedSpell != null)
            {
                StartCoroutine(StartDelay());
                direction = Manager.Instance.gameManager.GetDirection(CharacterMoveTileIsometric.CurrentTileIndex, mousePos);
                characterCombat.CastSpell(null, mousePos);
            }
            else
            {
                characterMoveTileIsometric.Mouse = CharacterMousePosition(mousePos);
            }
            if(isRest)
            {
                StopRest();
            }
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            gameManager.EndMyTurn(this);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isRest)
            {
                StopRest();
            }
        }
    }

    public IEnumerator waitRest;

    public IEnumerator StartMyTurn()
    {
        gameManager.StartNewTurn();
        yield return new WaitForSeconds(0.1f);
        myTurn = true;
        characterCombat.invisibilityDuration--;

        if(isRest)
        {
            characterStatus.Hp += Manager.Instance.configManager.healthRestPerTurn;
            characterStatus.Mp += Manager.Instance.configManager.manaRestPerTurn;
            if (gameManager.EndTurnRest())
            {
                yield return new WaitForSeconds(1.0f);
                gameManager.EndMyTurn(this);
            }
            else
            {
                StopRest();
            }
        }
        CharacterStatus.attributeStatus.StartNewTurn();
        Manager.Instance.canvasManager.UpdateStatus();
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

    public void SetupAnimation(string animName)
    {
        animationName = animName;
        this.transform.GetChild(0).gameObject.SetActive(false);
        this.transform.GetChild(1).gameObject.SetActive(false);
        var animatorObject = this.transform.Find(animName);

        animatorObject.gameObject.SetActive(true);
        Animator = animatorObject.GetComponent<Animator>();
    }

    public void Rest()
    {
        isRest = true;
        gameManager.restCount = 0;
        characterMoveTileIsometric.PlayAnimation(animationName + "_Idle_S");
        gameManager.campfire.transform.position = this.transform.position;
        gameManager.campfire.SetActive(true);
        gameManager.EndMyTurn(this);
    }

    public void StopRest()
    {
        StopCoroutine(waitRest);
        isRest = false;
        gameManager.campfire.SetActive(false);
    }
}
