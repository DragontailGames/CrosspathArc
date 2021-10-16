using System.Collections;
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
    public float movementSpeed = 1.25f;

    protected bool isDead;

    public string mainAnimation;

    public CreatureController target;

    public CreatureController forceTarget;

    public int range = 1;

    public bool meleeAndRanged = false;

    public int size;

    public int exp;

    public virtual void Start()
    {
        gameManager = Manager.Instance.gameManager;

        maxHp = attributeStatus.GetMaxHP(level);
        if (hp <= 0)
        {
            Hp = maxHp;
        }
        HpBar = this.transform.Find("HealthBar").GetChild(0);

        this.transform.position = Manager.Instance.gameManager.tilemap.GetCellCenterWorld(currentTileIndex) + offsetPosition;
        movePosition = this.transform.position;

        animator = this.GetComponentInChildren<Animator>();
        animator.speed = 0.7f;

        if(!this.GetComponent<BotMultipleTile>() && size>1)
        {
            botMultipleTile = gameObject.AddComponent(typeof(BotMultipleTile)) as BotMultipleTile;
            botMultipleTile.botController = this;
        }
    }

    public virtual void Setup(BotController botController)
    {
        this.attributeStatus = botController.attributeStatus;
        this.myTurn = botController.myTurn;
        this.inCombat = botController.inCombat;
        this.animationName = botController.animationName;
        this.direction = botController.direction;
        this.offsetSpell = botController.offsetSpell;
        this.level = botController.level;
        this.nickname = botController.nickname;
        this.hp = botController.hp;
        this.mp = botController.mp;
        this.movePosition = botController.movePosition;
        this.canMove = botController.canMove;
        this.currentTileIndex = botController.currentTileIndex;
        this.botMultipleTile = botController.botMultipleTile;
        this.spells = botController.spells;
        this.startTurnActions = botController.startTurnActions;
        this.aggro = botController.aggro;
        this.specialSpell = botController.specialSpell;
        this.offsetPosition = botController.offsetPosition;
        this.movementSpeed = botController.movementSpeed;
        this.mainAnimation = botController.mainAnimation;
        this.range = botController.range;
        this.meleeAndRanged = botController.meleeAndRanged;
        this.size = botController.size;
        this.exp = botController.exp;

        //Start();
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

    public override IEnumerator StartMyTurn(bool canStartTurn = true)
    {
        if (isDead || target == null || !canMove)
        {
            canStartTurn = false;
            yield return base.StartMyTurn(canStartTurn);
            yield break;
        }

        yield return base.StartMyTurn();
        yield return new WaitForSeconds(0.2f);

        Vector3Int targetTileIndex = target.currentTileIndex;
        List<PathFind.Point> path = gameManager.GetPathForLOS(currentTileIndex, targetTileIndex);

        if (gameManager.DetectLOS(path))
        {
            hasTarget = false;
            target.inCombat = false;
            this.inCombat = false;
            gameManager.EndMyTurn(this);
            yield break;
        }

        hasTarget = true;

        if (Cast(target)) { }
        else if (Attack(target)) { }
        else
        {
            Walk(targetTileIndex, path);
        }
        yield return new WaitForSeconds(0.2f);
        gameManager.EndMyTurn(this);
    }

    public void Walk(Vector3Int playerPos, List<PathFind.Point> path)
    {
        if (isDead)
        {
            return;
        }

        Vector3Int dest = new Vector3Int(path[0].x, path[0].y, 0);

        /*if (Manager.Instance.gameManager.GetCreatureInTile(dest))
        {
            return;
        }*/

        CreatureController creatureInNextTile = Manager.Instance.gameManager.GetCreatureInTile(dest);

        direction = gameManager.GetDirection(currentTileIndex, dest);

        bool canWalk = botMultipleTile ? botMultipleTile.Walk(dest) : true;

        if (creatureInNextTile != null || !canWalk)
        {
            var destPath = gameManager.GetPathWithCustom(currentTileIndex, playerPos, this);
            if (destPath.Count > 0 && (creatureInNextTile != null && !creatureInNextTile.GetComponent<CharacterController>()) || !canWalk)
            {
                dest = new Vector3Int(destPath[0].x, destPath[0].y, 0);
                direction = gameManager.GetDirection(currentTileIndex, dest);
            }
            else
            {
                dest = currentTileIndex;
                direction = gameManager.GetDirection(currentTileIndex, playerPos);
            }
        }

        canWalk = botMultipleTile ? botMultipleTile.Walk(dest) : true;
        if (botMultipleTile == null || canWalk)
        {
            animator.SetBool("Walk", true);
            PlayAnimation("Walk", direction);

            currentTileIndex = dest;

            movePosition = gameManager.tilemap.GetCellCenterWorld(currentTileIndex) + offsetPosition;
        }
        else
        {
            int count = 10;
            List<Vector3Int> exceptList = new List<Vector3Int>();
            exceptList.Add(dest);

            while(count>0)
            {
                var destPath = gameManager.GetPathWithExcept(currentTileIndex, playerPos, this, exceptList);

                if (destPath.Count > 0 )
                {
                    dest = new Vector3Int(destPath[0].x, destPath[0].y, 0);
                    direction = gameManager.GetDirection(currentTileIndex, dest);

                    canWalk = botMultipleTile.Walk(dest);

                    if (botMultipleTile == null || canWalk)
                    {
                        animator.SetBool("Walk", true);
                        PlayAnimation("Walk", direction);

                        currentTileIndex = dest;

                        movePosition = gameManager.tilemap.GetCellCenterWorld(currentTileIndex) + offsetPosition;
                        count = 0;
                    }
                    else
                    {
                        exceptList.Add(dest);
                    }
                }
                count--;
            }
        }
    }

    public bool Attack(CreatureController creatureController)
    {
        CharacterController characterController = target.GetComponent<CharacterController>();
        Vector3Int targetTileIndex = target.currentTileIndex;
        int offsetDiagonal = (targetTileIndex.x != currentTileIndex.x && targetTileIndex.y != currentTileIndex.y) ? range + 1 : range;
        if (!(Vector3.Distance(targetTileIndex, currentTileIndex) <= offsetDiagonal && !CheckMinionAndPlayer(characterController)))
        {
            return false;
        }
        if (isDead || creatureController == null)
        {
            return false;
        }

        direction = gameManager.GetDirection(currentTileIndex, creatureController.currentTileIndex);

        
        int hitChance = attributeStatus.GetValue(EnumCustom.Status.HitChance);
        int str = attributeStatus.GetValue(EnumCustom.Attribute.Str);
        int dodge = creatureController.attributeStatus.GetValue(EnumCustom.Status.Dodge);

        if (Combat.TryHit(hitChance, str, dodge, creatureController.nickname))
        {
            creatureController.ReceiveHit(this, str, str.ToString());
        }

        PlayAnimation("Attack", direction);
        return true;
    }

    public bool Cast(CreatureController creatureController)
    {
        Spell selectedSpell = null;
        direction = gameManager.GetDirection(currentTileIndex, creatureController.currentTileIndex);
        if (spells.Count > 0)
        {
            selectedSpell = spells[UnityEngine.Random.Range(0, spells.Count)];
        }
        else
        {
            return false;
        }

        int randValue = UnityEngine.Random.Range(0, 100);
        if(selectedSpell.probabilityToCast < randValue)
        {
            return false; 
        }
        var existingSpecial = this?.specialSpell.Find(n => n.effect == selectedSpell.specialEffect);
        if (existingSpecial != null)
        {
            return false;
        }
        if (!attributeStatus.HasBuff(selectedSpell.spellName) && selectedSpell.cooldown<=0)
        {
            PlayAnimation("Attack", direction);
            selectedSpell.Cast(null, this, creatureController, creatureController.currentTileIndex, null);
            return true;
        }
        return false;
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
        }
    }

    public override void Defeat()
    {
        base.Defeat();

        Hp = 0;
        if (!isDead)
        {
            Manager.Instance.characterController.inCombat = false;
            PlayAnimation("Dead", gameManager.GetDirection(currentTileIndex, currentTileIndex));
            isDead = true;
            gameManager.creatures.Remove(this);
            Invoke("TurnOffSprite", 1f);
            //gameManager.currentCreature--;
            //gameManager.EndMyTurn();
        }
    }

    public void TurnOffSprite()
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void PlayAnimation(string animationName, string dir)
    {
        string ani = "";
        if (!string.IsNullOrEmpty(mainAnimation))
        {
            ani = mainAnimation + "_" + animationName + "_" + dir;
        }
        else
        {
            ani = animationName + "_" + dir;
        }
        animator.Play(ani);
    }

    public CreatureController GetTarget(Type exept, Type only)
    {
        List<CreatureController> creaturesWithoutEnemy = new List<CreatureController>();
        if (exept != null)
        {
            creaturesWithoutEnemy = gameManager.creatures.FindAll(n => n.GetType() != exept);
        }
        else if(only != null)
        {
            creaturesWithoutEnemy = gameManager.creatures.FindAll(n => n.GetType() == only);
        }

        var shortestDistance = Mathf.Infinity;
        CreatureController smallCreatureDistance = null;
        foreach (var aux in creaturesWithoutEnemy)
        {
            if (aux.specialSpell.Find(n => n.CheckType<Invisibility>())?.duration > 0)
            {
                return null;
            }
            var distance = Vector3.Distance(aux.transform.position, this.transform.position);

            int limit = aux.aggro;

            if (Vector3Int.Distance(aux.currentTileIndex, currentTileIndex) < limit)
            {
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    smallCreatureDistance = aux;
                }
            }
        }

        return smallCreatureDistance;
    }
}
