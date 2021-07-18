using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : CreatureController
{
    public Transform HpBar;
    private int maxHp;

    public bool hasTarget = false;
    public Animator animator;

    public Vector3Int currentTileIndex;

    public Vector3 offsetPosition = new Vector3(0,0.5f,0);

    private Vector3 movePosition;
    public float movementSpeed = 10;

    protected bool isDead;

    public string mainAnimation;

    public Transform target;

    public Transform forceTarget;

    public virtual void Start()
    {
        gameManager = Manager.Instance.gameManager;

        maxHp = attributeStatus.GetMaxHP(level);
        Hp = maxHp;
        HpBar = this.transform.Find("HealthBar").GetChild(0);

        currentTileIndex = gameManager.tilemap.WorldToCell(this.transform.position);
        currentTileIndex.z = 0;

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


    /// <summary>
    /// Executa quando morrer
    /// </summary>
    public virtual void Defeat()
    {
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

    public void Attack(CharacterController characterController = null, BotController botController = null)
    {
        if (isDead)
        {
            return;
        }

        CharacterCombat characterCombat = null;
        if(characterController != null)
        {
            characterCombat = characterController.CharacterCombat;
        }

        int hitChance = attributeStatus.GetValue(EnumCustom.Status.HitChance);
        int str = attributeStatus.GetValue(EnumCustom.Attribute.Str);
        int dodge = characterCombat != null ? characterController.attributeStatus.GetValue(EnumCustom.Status.Dodge):
            botController.attributeStatus.GetValue(EnumCustom.Status.Dodge);

        Vector3Int destTileIndex = characterCombat != null ? characterController.CharacterMoveTileIsometric.CurrentTileIndex :
            botController.currentTileIndex;

        PlayAnimation("Attack", gameManager.GetDirection(currentTileIndex, destTileIndex));

        if (!Combat.TryHit(hitChance, str, dodge, characterController?.CharacterStatus.nickname))
        {
            return;
        }

        if (characterCombat)
            characterCombat.GetHit(str, this);
        else
            botController.ReceiveHit(this, str, str.ToString());
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
        BotController botController;

        if (!target || !canMove)
        {
            gameManager.EndMyTurn(this);
            yield break;
        }

        target.TryGetComponent(out characterController);
        target.TryGetComponent(out botController);

        Vector3Int targetTileIndex = botController==null?characterController.CharacterMoveTileIsometric.CurrentTileIndex:botController.currentTileIndex;
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
            Attack(characterController, botController);
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
    public virtual void ReceiveHit(CreatureController attacker, int damage, string damageText = "", bool ignoreArmor = false)
    {
        int armor = attributeStatus.GetValue(EnumCustom.Status.Armor);

        int trueDamage = ignoreArmor ? damage : Mathf.Clamp(damage - armor, 0, damage);
        Hp -= trueDamage;

        if (!ignoreArmor)
        {
            Manager.Instance.canvasManager.LogMessage(name + " sofreu " + damageText + " - " + armor + " = <color=red>" + trueDamage + "</color> de dano");//Manda mensagem do dano que o inimigo recebeu
        }
        else
        {
            Manager.Instance.canvasManager.LogMessage(name + " sofreu <color=red>" + damageText + "</color> de dano direto");//Manda mensagem do dano que o inimigo recebeu direto
        }

        if (target == null)
        {
            forceTarget = Manager.Instance.characterController.transform;
            target = Manager.Instance.characterController.transform;
            gameManager.creatures.Add(this);
        }

        if (Hp <= 0)
        {
            Defeat();//mata o inimigo
        }

        if(trueDamage>0)
            base.ReceiveDamage(attacker);
    }


    /// <summary>
    /// Inimigo sofrendo dano de speel
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="damageText"></param>
    public void ReceiveSpell(CreatureController attacker, int damage, string damageText, Spell spell)
    {
        if (spell.spellType == EnumCustom.SpellType.Special)
        {
            spell.CastSpecial(this);
        }
        else if (spell.spellType == EnumCustom.SpellType.Buff)
        {
            foreach (var aux in spell.buffDebuff)
            {
                if (aux.buffDebuffType == EnumCustom.BuffDebuffType.Attribute)
                {
                    this.attributeStatus.AddModifier(new AttributeModifier()
                    {
                        spellName = spell.spellName,
                        attribute = aux.attribute,
                        count = aux.turnDuration,
                        value = aux.value
                    }, null);
                }
                if (aux.buffDebuffType == EnumCustom.BuffDebuffType.Status)
                {
                    this.attributeStatus.AddModifier(null, new StatusModifier()
                    {
                        spellName = spell.spellName,
                        status = aux.status,
                        count = aux.turnDuration,
                        value = aux.value
                    });
                }
            }
        }
        else
        {
            Hp -= damage;
            Manager.Instance.canvasManager.LogMessage(name + " sofreu " + damageText + " = <color=red>" + damage + "</color> de dano");//Manda mensagem do dano que o inimigo recebeu
        }

        if (Hp <= 0)
        {
            Defeat();//mata o inimigo
        }

        if (target == null)
        {
            forceTarget = Manager.Instance.characterController.transform;
            target = Manager.Instance.characterController.transform;
            gameManager.creatures.Add(this);
        }

        if (damage > 0)
            base.ReceiveDamage(attacker);
    }

    public void PlayAnimation(string animationName, string dir)
    {
        string ani = mainAnimation + "_" + animationName + "_" + dir;
        animator.Play(ani);
    }
}
