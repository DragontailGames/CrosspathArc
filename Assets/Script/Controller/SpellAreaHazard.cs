using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpellAreaHazard : MonoBehaviour
{
    public int duration = 0;

    public int value = 0;

    public EnumCustom.SpellType spellType;

    public EnumCustom.CastEffect castEffect;

    public EnumCustom.SpecialEffect specialEffect;

    public List<BuffDebuff> buffDebuff;

    public string spellName;

    public Vector3Int position;

    public UnityAction action;

    public CreatureController caster;

    public SpellSO spell;

    public void Start()
    {
        action = () => StartRound();
        Manager.Instance.timeManager.startNewTurnAction += action;
    }

    public void StartRound()
    {
        duration--;
        if(duration<=0)
        {
            Manager.Instance.timeManager.startNewTurnAction -= action;
            Destroy(this.gameObject);
            return;
        }
        if(spellType == EnumCustom.SpellType.Hit)
        {
            EnemyController enemy = Manager.Instance.enemyManager.CheckEnemyInTile(position);
            enemy?.ReceiveHit(null, value, value.ToString(),true);
        }
        else if(spellType == EnumCustom.SpellType.Buff)
        {
            CreatureController controller =  Manager.Instance.gameManager.creatures.Find(n => n.currentTileIndex == position);
            if(controller != null)
            {
                if (CheckController(castEffect, controller))
                {
                    return;
                }
                foreach (var aux in buffDebuff)
                {
                    if (aux.buffDebuffType == EnumCustom.BuffDebuffType.Attribute)
                    {
                        controller.attributeStatus.AddModifier(new AttributeModifier()
                        {
                            spellName = spellName,
                            attribute = aux.attribute,
                            count = aux.turnDuration,
                            value = aux.value + aux.attributeInfluence.GetValue(caster)
                        }, null);
                    }
                    if (aux.buffDebuffType == EnumCustom.BuffDebuffType.Status)
                    {
                        controller.attributeStatus.AddModifier(null, new StatusModifier()
                        {
                            spellName = spellName,
                            status = aux.status,
                            count = aux.turnDuration,
                            value = aux.value + aux.attributeInfluence.GetValue(caster)
                        });
                    }
                }
            }
        }
        else if(spellType == EnumCustom.SpellType.Debuff)
        {
            CreatureController controller = Manager.Instance.gameManager.creatures.Find(n => n.currentTileIndex == position);
            if (controller != null)
            {
                if(!CheckController(castEffect, controller))
                {
                    return;
                }
                foreach (var aux in buffDebuff)
                {
                    if (aux.buffDebuffType == EnumCustom.BuffDebuffType.Attribute)
                    {
                        controller.attributeStatus.AddModifier(new AttributeModifier()
                        {
                            spellName = spellName,
                            attribute = aux.attribute,
                            count = aux.turnDuration,
                            value = -(aux.value + aux.attributeInfluence.GetValue(caster))
                        }, null);
                    }
                    if (aux.buffDebuffType == EnumCustom.BuffDebuffType.Status)
                    {
                        controller.attributeStatus.AddModifier(null, new StatusModifier()
                        {
                            spellName = spellName,
                            status = aux.status,
                            count = aux.turnDuration,
                            value = -(aux.value + aux.attributeInfluence.GetValue(caster))
                        });
                    }
                }
            }
        }
        else if (spellType == EnumCustom.SpellType.Special)
        {
            CreatureController controller = Manager.Instance.gameManager.creatures.Find(n => n.currentTileIndex == position);
            if (controller != null)
            {
                if (!CheckController(castEffect, controller))
                {
                    return;
                }

                if (specialEffect != EnumCustom.SpecialEffect.None)
                {
                    ParserCustom.SpellSpecialParser(new SpecialSpell(1, value, caster, controller, specialEffect, spellName));
                }
                else
                {
                    if (spell.subSpell.Count > 0)
                    {
                        foreach (var spell in spell.subSpell)
                        {
                            if (spell.castTarget == EnumCustom.CastTarget.Area_Hazard)
                            {
                                if (spell.spellType == EnumCustom.SpellType.Special)
                                {
                                    ParserCustom.SpellSpecialParser(new SpecialSpell(spell.duration, spell.GetValue(caster), caster, controller, spell.specialEffect, spellName));
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public bool CheckController(EnumCustom.CastEffect castEffect, CreatureController controller)
    {
        if(castEffect == EnumCustom.CastEffect.Enemy)
        {
            if(controller as EnemyController)
            {
                return true;
            }
        }
        else if(castEffect == EnumCustom.CastEffect.Player)
        {
            if (controller as CharacterController)
            {
                return true;
            }
        }
        else if (castEffect == EnumCustom.CastEffect.Allies)
        {
            if (controller as CharacterController || controller as MinionController)
            {
                return true;
            }
        }
        return false;
    }
}
