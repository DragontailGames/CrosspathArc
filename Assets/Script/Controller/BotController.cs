﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BotController : CreatureController
{
    public Transform HpBar;
    private int maxHp;

    public bool hasTarget = false;
    public Animator animator;

    public Vector3 offsetPosition = new Vector3(0,0.5f,0);

    private Vector3 movePosition;
    public float movementSpeed = 10;

    protected bool isDead;

    public string mainAnimation;

    public CreatureController target;

    public CreatureController forceTarget;

    public virtual void Start()
    {
        gameManager = Manager.Instance.gameManager;

        maxHp = attributeStatus.GetMaxHP(level);
        Hp = maxHp;
        HpBar = this.transform.Find("HealthBar").GetChild(0);

        this.transform.position = Manager.Instance.gameManager.tilemap.GetCellCenterWorld(currentTileIndex) + offsetPosition;
        movePosition = this.transform.position;

        animator = this.GetComponentInChildren<Animator>();
        animator.speed = 0.7f;
    }

    public virtual void Update()
    {
        var scale = HpBar.localScale;
        scale.x = Mathf.Clamp((float)Hp / (float)maxHp, 0, 1);//Animação da barra de Hp
        HpBar.localScale = scale;
    }

    public void FixedUpdate()
    {
        if (Hp <= 0)//Correção temporaria
            return;

        this.transform.position = Vector3.MoveTowards(this.transform.position, movePosition, movementSpeed * Time.deltaTime);
        animator.SetBool("Walk", Vector3.Distance(this.transform.position, movePosition) > 0.05f && Hp > 0);
    }

    public void Walk(Vector3Int playerPos, List<PathFind.Point> path)
    {
        if (isDead)
        {
            return;
        }

        Vector3Int dest = new Vector3Int(path[0].x, path[0].y, 0);

        if (Manager.Instance.gameManager.CheckHasBotInTile(dest))
        {
            var destPath = gameManager.GetPathWithCustom(currentTileIndex, playerPos);
            dest = new Vector3Int(destPath[0].x, destPath[0].y, 0);
        }

        animator.SetBool("Walk", true);
        PlayAnimation("Walk", gameManager.GetDirection(currentTileIndex, dest));
        
        currentTileIndex = dest;

        movePosition = gameManager.tilemap.GetCellCenterWorld(currentTileIndex) + offsetPosition;
    }


    public void Attack(CreatureController creatureController)
    {
        if (isDead)
        {
            return;
        }

        int hitChance = attributeStatus.GetValue(EnumCustom.Status.HitChance);
        int str = attributeStatus.GetValue(EnumCustom.Attribute.Str);
        int dodge = creatureController.attributeStatus.GetValue(EnumCustom.Status.Dodge);

        Vector3Int destTileIndex = creatureController.currentTileIndex;

        PlayAnimation("Attack", gameManager.GetDirection(currentTileIndex, destTileIndex));

        if (!Combat.TryHit(hitChance, str, dodge, creatureController.nickname))
        {
            return;
        }

        creatureController.ReceiveHit(this, str, str.ToString());
    }

    public override IEnumerator StartMyTurn()
    {
        yield return base.StartMyTurn();

        if (isDead)
        {
            gameManager.EndMyTurn(this);
            yield break;
        }

        yield return new WaitForSeconds(0.2f);

        CharacterController characterController;

        if (!target || !canMove)
        {
            gameManager.EndMyTurn(this);
            yield break;
        }

        target.TryGetComponent(out characterController);

        Vector3Int targetTileIndex = target.currentTileIndex;
        List<PathFind.Point> path = gameManager.GetPath(currentTileIndex, targetTileIndex);

        if (gameManager.DetectLOS(path))
        {
            hasTarget = false;
            gameManager.EndMyTurn(this);
            yield break;
        }

        hasTarget = true;
        yield return new WaitForSeconds(0.2f * 2);

        int offsetDiagonal = (targetTileIndex.x != currentTileIndex.x && targetTileIndex.y != currentTileIndex.y) ? 2 : 1;
        if (Vector3.Distance(targetTileIndex, currentTileIndex) <= offsetDiagonal && !CheckMinionAndPlayer(characterController))
        {
            Attack(target);
        }
        else
        {
            Walk(targetTileIndex, path);
        }
        gameManager.EndMyTurn(this);
    }

    public bool CheckMinionAndPlayer(CharacterController characterController)
    {
        bool isMinion = this.GetType() == typeof(MinionController);
        if(characterController && isMinion)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Inimigo sofrendo dano de hit
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="damageText"></param>
    public override void ReceiveHit(CreatureController attacker, int damage, string damageText = "", bool ignoreArmor = false)
    {
        base.ReceiveHit(attacker, damage, damageText, ignoreArmor);

        if (target == null)
        {
            forceTarget = Manager.Instance.characterController;
            target = Manager.Instance.characterController;
            gameManager.creatures.Add(this);
        }
    }


    /// <summary>
    /// Inimigo sofrendo dano de speel
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="damageText"></param>
    public override void ReceiveSpell(CreatureController attacker, int damage, string damageText, Spell spell)
    {
        base.ReceiveSpell(attacker, damage, damageText, spell);

        if (target == null)
        {
            forceTarget = Manager.Instance.characterController;
            target = Manager.Instance.characterController;
            gameManager.creatures.Add(this);
        }
    }

    public override void Defeat()
    {
        base.Defeat();

        Hp = 0;
        if (!isDead)
        {
            PlayAnimation("Dead", gameManager.GetDirection(currentTileIndex, currentTileIndex));
            isDead = true;
            gameManager.creatures.Remove(this);
            //gameManager.currentCreature--;
            //gameManager.EndMyTurn();
        }
    }

    public void PlayAnimation(string animationName, string dir)
    {
        string ani = mainAnimation + "_" + animationName + "_" + dir;
        animator.Play(ani);
    }

    public CreatureController GetTarget(Type exept)
    {
        var creaturesWithoutEnemy = gameManager.creatures.FindAll(n => n.GetType() != exept);
        var shortestDistance = Mathf.Infinity;
        Transform smallDistance = null;
        foreach (var aux in creaturesWithoutEnemy)
        {
            if (aux.specialSpell.Find(n => n.CheckType<Invisibility>())?.duration > 0)
            {
                return null;
            }
            var distance = Vector3.Distance(aux.transform.position, this.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                smallDistance = aux.transform;
            }
        }

        return gameManager.GetCreatureInTile(gameManager.tilemap.WorldToCell(smallDistance.position));
    }
}
