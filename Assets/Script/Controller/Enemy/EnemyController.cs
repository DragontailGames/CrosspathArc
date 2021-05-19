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
        base.ReceiveHit(damage, damageText);
        int armor = attributeStatus.GetValue(EnumCustom.Status.Armor);
        int trueDamage = Mathf.Clamp(damage - armor, 0, damage);

        Manager.Instance.canvasManager.LogMessage(enemy.name + " sofreu " + damageText + " - " + armor + " = <color=red>" + trueDamage + "</color> de dano");//Manda mensagem do dano que o inimigo recebeu
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
            specialEffectDuration = spell.specialEffectDuration;
            specialEffect = spell.specialEffect;
            poisonDamage = _poisonDamage;
        }
        else
        {
            hp -= damage;
            Manager.Instance.canvasManager.LogMessage(enemy.name + " sofreu " + damageText + " = <color=red>" + damage + "</color> de dano");//Manda mensagem do dano que o inimigo recebeu
        }

        if (hp <= 0)
        {
            Defeat();//mata o inimigo
        }
    }

    public override void Update()
    {
        if (Vector3Int.Distance(player.CharacterMoveTileIsometric.CurrentTileIndex, currentTileIndex) < 10)
        {
            if (!gameManager.creatures.Contains(this.gameObject))
            {
                gameManager.creatures.Add(this.gameObject);
            }
        }
        else
        {
            if (gameManager.creatures.Contains(this.gameObject))
            {
                gameManager.creatures.Remove(this.gameObject);
            }
        }
    }

    public override IEnumerator StartMyTurn(bool isEnemy = true)
    {
        if (specialEffect != EnumCustom.SpecialEffect.None)
        {
            if (specialEffectDuration <= 0)
            {
                specialEffect = EnumCustom.SpecialEffect.None;
            }
            specialEffectDuration--;
            if (specialEffect == EnumCustom.SpecialEffect.Sleep)
            {
                gameManager.EndMyTurn();
                yield break;
            }
            if(specialEffect == EnumCustom.SpecialEffect.Poison)
            {
                hp -= poisonDamage;
                Manager.Instance.canvasManager.LogMessage($"{enemy.name} sofreu {poisonDamage} do veneno");//Manda mensagem do dano que o inimigo recebeu
            }
        }
        target = GetTarget();
        yield return new WaitForSeconds(0.2f);

        StartCoroutine(base.StartMyTurn());
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
