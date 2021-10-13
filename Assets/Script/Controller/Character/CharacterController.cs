﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using System.Linq;

/// <summary>
/// Controlador geral do jogador para fazer relação entre as classes
/// </summary>
public class CharacterController : CreatureController
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

    #endregion

    public Animator animator;

    private EnemyManager enemyManager;

    public bool delay = false;

    public bool isRest = false;

    public override void Awake()
    {
        base.Awake();
        Manager.Instance.characterController = this;

        characterCombat = this.GetComponent<CharacterCombat>();
        characterCombat.controller = this;
        characterInterface = this.GetComponent<CharacterInterface>();
        characterInterface.controller = this;
        characterInventory = this.GetComponent<CharacterInventory>();
        characterInventory.controller = this;
        characterMoveTileIsometric = this.GetComponent<CharacterMoveTileIsometric>();
        characterMoveTileIsometric.controller = this;
        characterStatus = this.GetComponent<CharacterStatus>();
        characterStatus.controller = this;

        animator = this.transform.GetComponentInChildren<Animator>();
        animator.speed = 0.6f;

        enemyManager = Manager.Instance.enemyManager;
    }

    public void Update()
    {
        if(!myTurn)
        {
            return;
        }
        if (MouseOn() && !gameManager.InPause && !delay)//Detecta o click do jogador
        {
            StartCoroutine(StartDelay());

            if (EventSystem.current.IsPointerOverGameObject() || gameManager.cenarioEntitiesMouseOn.Count>0) return;

            Vector3Int mousePos = MousePosition();

            if (gameManager.cenarioEntities.Find(n => n.currentTileIndex == mousePos) != null) return;

            EnemyController enemyInTile = enemyManager.CheckEnemyInTile(mousePos);

            if ((characterCombat.selectedSpell == null || characterCombat.selectedSpell?.castTarget == EnumCustom.CastTarget.Enemy) && enemyInTile != null && enemyInTile.Hp>0)
            {
                foreach (var aux in specialSpell.ToList())
                {
                    aux.HandleAttack(this);
                }
                characterCombat.TryHit(enemyInTile, mousePos, characterMoveTileIsometric.controller.currentTileIndex);
            }
            else if(characterCombat.selectedSpell != null)
            {
                direction = Manager.Instance.gameManager.GetDirection(CharacterMoveTileIsometric.controller.currentTileIndex, mousePos);
                foreach (var aux in specialSpell.ToList())
                {
                    aux.HandleAttack(this);
                }
                if (characterCombat.selectedSpell.castTarget == EnumCustom.CastTarget.Target)
                {
                    characterCombat.CastSpell(gameManager.GetCreatureInTile(mousePos), mousePos);
                }
                else
                {
                    characterCombat.CastSpell(null, mousePos);
                }
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

    public bool MouseOn()
    {
        return inCombat ? Input.GetMouseButtonDown(0) : Input.GetMouseButton(0);
    }

    public override IEnumerator StartMyTurn(bool canStartTurn = true)
    {
        gameManager.StartNewTurn();

        yield return base.StartMyTurn(canMove);

        characterStatus.StartTurn();

        foreach(var aux in characterCombat.spellAfterDelays.ToList())
        {
            aux.EndTurn();
        }

        if (isRest)
        {
            Hp += Manager.Instance.configManager.healthRestPerTurn;
            Mp += Manager.Instance.configManager.manaRestPerTurn;
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
        pos -= characterMoveTileIsometric.controller.currentTileIndex;

        return MathfCustom.Sign(pos);//Retorna o sinal do valor(-1,+1 ou 0)
    }

    public void SetupAnimation(string animName)
    {
        animationName = animName;
        this.transform.GetChild(0).gameObject.SetActive(false);
        this.transform.GetChild(1).gameObject.SetActive(false);
        var animatorObject = this.transform.Find(animName);

        animatorObject.gameObject.SetActive(true);
        animator = animatorObject.GetComponent<Animator>();
    }

    public IEnumerator StartDelay()
    {
        delay = true;
        yield return new WaitForSeconds(0.3f);
        delay = false;
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
        isRest = false;
        gameManager.campfire.SetActive(false);
    }

    public override void ReceiveHit(CreatureController attacker, int damage, string damageText = "", bool ignoreArmor = false)
    {
        direction = Manager.Instance.gameManager.GetDirection(CharacterMoveTileIsometric.controller.currentTileIndex, attacker.currentTileIndex);
        animator.Play(animationName + "_GetHit_" + direction);
        base.ReceiveHit(attacker, damage, damageText, ignoreArmor);
    }

    public override void Defeat()
    {
        string dieAnimationName = animationName + "_Die_" + direction;
        animator.Play(dieAnimationName);
        Manager.Instance.gameManager.SetupPause(true);
        Manager.Instance.gameManager.creatures.Remove(this);
    }
}
