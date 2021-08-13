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
    public float movementSpeed = 10;

    protected bool isDead;

    public string mainAnimation;

    public CreatureController target;

    public CreatureController forceTarget;

    public List<Spell> spells;

    public int range = 1;

    public bool meleeAndRanged = false;

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

        /*if (Manager.Instance.gameManager.GetCreatureInTile(dest))
        {
            return;
        }*/

        CreatureController creatureInNextTile = Manager.Instance.gameManager.GetCreatureInTile(dest);

        direction = gameManager.GetDirection(currentTileIndex, dest);

        if (creatureInNextTile != null)
        {
            var destPath = gameManager.GetPathWithCustom(currentTileIndex, playerPos);
            if (destPath.Count > 0)
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

        animator.SetBool("Walk", true);
        PlayAnimation("Walk",direction);
        
        currentTileIndex = dest;

        movePosition = gameManager.tilemap.GetCellCenterWorld(currentTileIndex) + offsetPosition;
    }


    public void Attack(CreatureController creatureController)
    {
        if (isDead)
        {
            return;
        }

        Vector3Int destTileIndex = creatureController.currentTileIndex;
        direction = gameManager.GetDirection(currentTileIndex, destTileIndex);

        int offsetDiagonal = (creatureController.currentTileIndex.x != currentTileIndex.x && creatureController.currentTileIndex.y != currentTileIndex.y) ? 2 : 1;

        Spell selectedSpell = null;
        if (spells.Count > 0)
        {
            selectedSpell = spells[UnityEngine.Random.Range(0, spells.Count)];
        }

        if (spells.Count>0 && (!meleeAndRanged || meleeAndRanged && Vector3.Distance(creatureController.currentTileIndex, currentTileIndex) > offsetDiagonal) && !attributeStatus.HasBuff(selectedSpell.spellName))
        {
            selectedSpell.Cast(null,this, creatureController, creatureController.currentTileIndex, null);
        }
        else
        {
            int hitChance = attributeStatus.GetValue(EnumCustom.Status.HitChance);
            int str = attributeStatus.GetValue(EnumCustom.Attribute.Str);
            int dodge = creatureController.attributeStatus.GetValue(EnumCustom.Status.Dodge);

            if (Combat.TryHit(hitChance, str, dodge, creatureController.nickname))
            {
                creatureController.ReceiveHit(this, str, str.ToString());
            }
        }

        PlayAnimation("Attack", direction);
    }

    public override IEnumerator StartMyTurn()
    {
        if (isDead || !target || !canMove)
        {
            gameManager.EndMyTurn(this);
            yield break;
        }
        yield return base.StartMyTurn();

        CharacterController characterController;

        //yield return new WaitForSeconds(0.2f);

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
        yield return new WaitForSeconds(0.3f);

        int offsetDiagonal = (targetTileIndex.x != currentTileIndex.x && targetTileIndex.y != currentTileIndex.y) ? range + 1 : range;
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
            PlayAnimation("Dead", gameManager.GetDirection(currentTileIndex, currentTileIndex));
            isDead = true;
            gameManager.creatures.Remove(this);
            //gameManager.currentCreature--;
            //gameManager.EndMyTurn();
        }
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

    public CreatureController GetTarget(Type exept, Type only, int limit)
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
