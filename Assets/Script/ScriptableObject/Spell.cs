using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Arc/Spell")]
public class Spell : ScriptableObject
{
    public string spellName;

    public string description;

    public Sprite icon;

    public EnumCustom.CastTarget castTarget;

    public int availableAt = 1;

    public int minValue;

    public int maxValue;

    public int fixedValue = 0;

    public int manaCost;

    public EnumCustom.SpellType spellType;

    public int area;

    public List<BuffDebuff> buffDebuff;

    public List<AttributeInfluence> attributeInfluence;

    public GameObject spellCastObject;

    public EnumCustom.SpecialEffect specialEffect;

    public int duration = 0;

    public int invokeLimit = 0;

    public List<SubSpell> subSpell = new List<SubSpell>();

    public int GetValue(CreatureController creatureController)
    {
        int value = 0;

        if(fixedValue == 0)
            value = UnityEngine.Random.Range(minValue, maxValue + 1);
        else
            value = fixedValue;

        foreach (var aux in attributeInfluence)
        {
            var auxAttribute = aux.GetValue(creatureController);
            value += auxAttribute;
        }

        return value;
    }

    public void Cast(CreatureController caster, CreatureController target)
    {
        //PEDRO MELHORA ISSO E COLOCA TUDO AQUI
        foreach (var aux in subSpell)
        {
            aux.Cast(caster, target);
        }
    }

    public void CastBuff(CharacterController controller)
    {
        if (spellType == EnumCustom.SpellType.Buff)
        {
            foreach (var aux in this.buffDebuff)
            {
                if (aux.buffDebuffType == EnumCustom.BuffDebuffType.Attribute)
                {
                    controller.attributeStatus.AddModifier(new AttributeModifier()
                    {
                        spellName = spellName,
                        attribute = aux.attribute,
                        count = aux.turnDuration,
                        value = aux.value
                    }, null);
                }
                if (aux.buffDebuffType == EnumCustom.BuffDebuffType.Status)
                {
                    controller.attributeStatus.AddModifier(null, new StatusModifier()
                    {
                        spellName = spellName,
                        status = aux.status,
                        count = aux.turnDuration,
                        value = aux.value
                    });
                }
                GameObject objectSpell = Instantiate(spellCastObject, controller.transform);
                Destroy(objectSpell, 1.0f);
            }
        }

        controller.Mp -= manaCost;
    }

    public void CastSpecial(CreatureController controller, CreatureController attacker)
    {
        ParserCustom.SpellSpecialParser(new SpecialSpell(duration, GetValue(attacker), controller, specialEffect));
    }

    /// <summary>
    /// Animação da spell
    /// </summary>
    /// <param name="targetPos">posição do inimigo</param>
    /// <param name="character"></param>
    /// <param name="hitAction">Ação apos o hit</param>
    /// <returns></returns>
    public void AnimateCastProjectileSpell(Vector3 targetPos, Transform character, UnityEngine.Events.UnityAction hitAction)
    {
        //Corrige a rotação da spell
        Vector3 diff = targetPos - character.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

        GameObject spellCreated = Instantiate(spellCastObject, character.position + Vector3.up * 0.5f, Quaternion.Euler(0f, 0f, rot_z - 180));
        spellCreated.gameObject.AddComponent(typeof(SpellProjectileController));
        spellCreated.GetComponent<SpellProjectileController>().StartHit(targetPos, hitAction);
    }

    public void AnimateCastAreaSpell(Vector3 position, Vector3Int index)
    {
        GameObject spellCreated = Instantiate(spellCastObject, position + Vector3.up * 0.25f, Quaternion.identity);
        spellCreated.transform.rotation = Quaternion.Euler(new Vector3(0, 0, UnityEngine.Random.Range(0, 360)));
        if(spellType == EnumCustom.SpellType.Area_Hazard)
        {
            SpellAreaHazard spellAreaHazard = spellCreated.GetComponent<SpellAreaHazard>();
            spellAreaHazard.duration = duration;
            spellAreaHazard.damage = fixedValue != 0 ? fixedValue : UnityEngine.Random.Range(minValue, maxValue);
            spellAreaHazard.position = index;
        }
        else
        {
            Destroy(spellCreated, 1.2f);
        }
    }

    public GameObject InvokeCreature(Vector3 position)
    {
        GameObject spellCreated = Instantiate(spellCastObject, position + Vector3.up * 0.25f, Quaternion.identity);
        spellCreated.GetComponent<MinionController>().duration = duration;
        return spellCreated;
    }
    
    public void CureDesease(CreatureController creatureController)
    {
        if(specialEffect != EnumCustom.SpecialEffect.None)
        {
            if (specialEffect != EnumCustom.SpecialEffect.All)
            {
                creatureController.specialSpell.Find(n => n.effect == specialEffect).EndOfDuration(creatureController);
            }
            else
            {
                var auxSpecialSpell = creatureController.specialSpell.FindAll(n => n.effect == EnumCustom.SpecialEffect.Cannot_Walk || 
                n.effect == EnumCustom.SpecialEffect.Paralyze || 
                n.effect == EnumCustom.SpecialEffect.Poison || 
                n.effect == EnumCustom.SpecialEffect.Sleep);
                auxSpecialSpell[UnityEngine.Random.Range(0, auxSpecialSpell.Count)].EndOfDuration(creatureController);
            }
        }
        foreach(var bfAux in buffDebuff)
        {
            if (bfAux.value < 0)
            {
                if (bfAux.buffDebuffType == EnumCustom.BuffDebuffType.Status)
                {
                    var statusToRemove = creatureController.attributeStatus.statusModifiers.Find(n => n.status == bfAux.status);    
                    Manager.Instance.canvasManager.RemoveLogText(statusToRemove.spellName);
                    creatureController.attributeStatus.statusModifiers.Remove(statusToRemove);
                }
                if(bfAux.buffDebuffType == EnumCustom.BuffDebuffType.Attribute)
                {
                    var attribute = creatureController.attributeStatus.attributeModifiers.Find(n => n.attribute == bfAux.attribute);
                    Manager.Instance.canvasManager.RemoveLogText(attribute.spellName);
                    creatureController.attributeStatus.attributeModifiers.Remove(attribute);
                }
            }
        }
    }
}
