using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BotController
{
    private EnemyManager enemyManager;
    public Enemy enemy;

    private CharacterController player;

    public int range = 1;

    private int specialEffectDuration;
    private EnumCustom.SpecialEffect specialEffect;
    public int poisonDamage;

    public Transform forceTarget;

    public override void Start()
    {
        base.Start();

        player = Manager.Instance.characterController;

        enemyManager = Manager.Instance.enemyManager;
        enemyManager.enemies.Add(this);

        if (!gameManager.DetectLOS(gameManager.GetPath(currentTileIndex, player.CharacterMoveTileIsometric.CurrentTileIndex)))
        {
            hasTarget = true;
        }
    }

    public override void ReceiveHit(int damage, string damageText = "")
    {
        Debug.Log("DAMAGE " + damage);
        base.ReceiveHit(damage, damageText);
        int armor = attributeStatus.GetValue(EnumCustom.Status.Armor);
        int trueDamage = Mathf.Clamp(damage - armor, 0, damage);

        if(specialEffect == EnumCustom.SpecialEffect.Sleep)
        {
            specialEffect = EnumCustom.SpecialEffect.None;
        }

        Manager.Instance.canvasManager.LogMessage(enemy.name + " sofreu " + damageText + " - " + armor + " = <color=red>" + trueDamage + "</color> de dano");//Manda mensagem do dano que o inimigo recebeu

        if(target == null)
        {
            forceTarget = Manager.Instance.characterController.transform;
            target = Manager.Instance.characterController.transform;
            gameManager.creatures.Add(this.gameObject);
        }
    }

    /// <summary>
    /// Inimigo sofrendo dano de speel
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="damageText"></param>
    public void ReceiveSpell(int damage, string damageText, Spell spell, int _poisonDamage = 0)
    {
        if(spell.spellType == EnumCustom.SpellType.Special)
        {
            specialEffectDuration = spell.duration;
            specialEffect = spell.specialEffect;
            poisonDamage = _poisonDamage;
        }
        else if(spell.spellType == EnumCustom.SpellType.Buff)
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
            if (specialEffect == EnumCustom.SpecialEffect.Sleep)
            {
                specialEffect = EnumCustom.SpecialEffect.None;
            }
            hp -= damage;
            Manager.Instance.canvasManager.LogMessage(enemy.name + " sofreu " + damageText + " = <color=red>" + damage + "</color> de dano");//Manda mensagem do dano que o inimigo recebeu
        }

        if (hp <= 0)
        {
            Defeat();//mata o inimigo
        }

        if (target == null)
        {
            forceTarget = Manager.Instance.characterController.transform;
            target = Manager.Instance.characterController.transform;
            gameManager.creatures.Add(this.gameObject);
        }
    }

    public override void Update()
    {
        base.Update();
        if (Vector3Int.Distance(player.CharacterMoveTileIsometric.CurrentTileIndex, currentTileIndex) < 10)
        {
            target = GetTarget();
            if (!gameManager.creatures.Contains(this.gameObject))
            {
                gameManager.creatures.Add(this.gameObject);
            }
        }
        else if(forceTarget == null)
        {
            if (gameManager.creatures.Contains(this.gameObject))
            {
                gameManager.creatures.Remove(this.gameObject);
            }
        }
    }

    public override IEnumerator StartMyTurn(float waitTime, bool isEnemy = true)
    {
        if (specialEffect != EnumCustom.SpecialEffect.None)
        {
            if (specialEffectDuration <= 0)
            {
                specialEffect = EnumCustom.SpecialEffect.None;
            }
            specialEffectDuration--;
            if (specialEffect == EnumCustom.SpecialEffect.Sleep || specialEffect == EnumCustom.SpecialEffect.Paralyze)
            {
                gameManager.EndMyTurn();
                yield break;
            }
            if(specialEffect == EnumCustom.SpecialEffect.Poison)
            {
                hp -= poisonDamage;
                Manager.Instance.canvasManager.LogMessage($"{enemy.name} sofreu {poisonDamage} do veneno");//Manda mensagem do dano que o inimigo recebeu
                if(hp <= 0)
                {
                    Defeat();
                    Manager.Instance.gameManager.EndMyTurn();
                    yield break;
                }
            }
        }
        target = GetTarget();
        yield return new WaitForSeconds(0.2f);

        StartCoroutine(base.StartMyTurn(0.2f));
    }

    public Transform GetTarget()
    {
        var creaturesWithoutEnemy = gameManager.creatures.FindAll(n => n.GetComponent<EnemyController>() == null);
        var shortestDistance = Mathf.Infinity;
        Transform smallDistance = null; 
        foreach (var aux in creaturesWithoutEnemy)
        {
            if(aux.GetComponent<CharacterController>())
            {
                if(aux.GetComponent<CharacterController>().CharacterCombat.invisibilityDuration>0)
                {
                    return null;
                }
            }
            var distance = Vector3.Distance(aux.transform.position, this.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                smallDistance = aux.transform;
            }
        }
        return smallDistance;
    }

    public override void Defeat()
    {
        base.Defeat();

        Manager.Instance.characterController.CharacterStatus.AddExp(enemy.exp);
        Manager.Instance.canvasManager.LogMessage(enemy.name + " foi derrotado, <color=yellow>" + enemy.exp + "</color> exp ganha");
        this.transform.Find("HealthBar").gameObject.SetActive(false);
    }

}
