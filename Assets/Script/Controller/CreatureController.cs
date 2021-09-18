using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CreatureController : MonoBehaviour
{
    public GameManager gameManager;

    public AttributeStatus attributeStatus;

    public bool myTurn = false;

    public bool inCombat = false;

    public string animationName = "";

    public string direction;

    public Vector2 offsetSpell;

    public int level = 1;//level atual

    public string nickname;//Apelido do jogador para ser exibido

    public int hp, mp;//quantidade de hp e mp

    public bool canMove = true;

    public Vector3Int currentTileIndex;

    public BotMultipleTile botMultipleTile;

    public int Hp { get => this.hp; set => this.hp = Mathf.Clamp(value, 0, attributeStatus.GetMaxHP(level)); }
    public int Mp { get => this.mp; set => this.mp = Mathf.Clamp(value, 0, attributeStatus.GetMaxMP(level)); }

    public List<SpecialSpell> specialSpell = new List<SpecialSpell>();

    public virtual void Awake()
    {
        gameManager = Manager.Instance.gameManager;

        currentTileIndex = gameManager.tilemap.WorldToCell(this.transform.position);
        currentTileIndex.z = 0;

        gameManager.creatures.Add(this);
    }

    public virtual IEnumerator StartMyTurn(bool canStartTurn = true)
    {
        foreach (var aux in specialSpell.ToList())
        {
            aux.StartNewTurn(this);
        }

        attributeStatus.StartNewTurn();

        //CharacterStatus.attributeStatus.StartNewTurn();
        Manager.Instance.canvasManager.UpdateStatus();

        if(canStartTurn)
        {
            myTurn = true;
            yield return null;
        }
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

    public virtual void ReceiveDamage(CreatureController attacker)
    {
        foreach (var aux in specialSpell.ToList())
        {
            aux.ReceiveHit(attacker, this);
        }
    }

    [ContextMenu("Deal fake damage")]
    public void DealFakeDamage()
    {
        hp -= 5;
    }

    public virtual void ReceiveHit(CreatureController attacker, int damage, string damageText = "", bool ignoreArmor = false)
    {
        int armor = attributeStatus.GetValue(EnumCustom.Status.Armor);

        int trueDamage = ignoreArmor ? damage : Mathf.Clamp(damage - armor, 0, damage);
        Hp -= trueDamage;

        if (!ignoreArmor)
        {
            Manager.Instance.canvasManager.LogMessage(nickname + " sofreu " + damageText + " - " + armor + " = <color=red>" + trueDamage + "</color> de dano");//Manda mensagem do dano que o inimigo recebeu
        }
        else
        {
            Manager.Instance.canvasManager.LogMessage(nickname + " sofreu <color=red>" + damageText + "</color> de dano direto");//Manda mensagem do dano que o inimigo recebeu direto
        }

        if (Hp <= 0)
        {
            Defeat();//mata o inimigo
        }

        if (trueDamage > 0)
            ReceiveDamage(attacker);
    }

    public virtual void ReceiveSpell(CreatureController caster, int damage, string damageText, Spell spell)
    {
        if (spell.spellType == EnumCustom.SpellType.Special)
        {
            spell.CastSpecial(this, caster);
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
                        value = aux.value + aux.attributeInfluence.GetValue(caster)
                    }, null);
                }
                if (aux.buffDebuffType == EnumCustom.BuffDebuffType.Status)
                {
                    this.attributeStatus.AddModifier(null, new StatusModifier()
                    {
                        spellName = spell.spellName,
                        status = aux.status,
                        count = aux.turnDuration,
                        value = aux.value + aux.attributeInfluence.GetValue(caster)
                    });
                }
            }
        }
        else if (spell.spellType == EnumCustom.SpellType.Cure_Disease)
        {
            spell.CureDesease(this);
        }
        else
        {
            Hp -= damage;
            Manager.Instance.canvasManager.LogMessage(nickname + " sofreu " + damageText + " = <color=red>" + damage + "</color> de dano");//Manda mensagem do dano que o inimigo recebeu
        }

        if (Hp <= 0)
        {
            direction = Manager.Instance.gameManager.GetDirection(currentTileIndex, caster.currentTileIndex);
            Defeat();//mata o inimigo
        }

        if (damage > 0)
            ReceiveDamage(caster);
    }

    /// <summary>
    /// Executa quando morrer
    /// </summary>
    public virtual void Defeat()
    {
    }
}