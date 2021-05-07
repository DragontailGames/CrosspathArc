﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Arc/Spell")]
public class Spell : ScriptableObject
{
    public string spellName;

    public string description;

    public EnumCustom.SpellType spellType;

    public int availableAt = 1;

    public int minValue;

    public int maxValue;

    public int fixedValue = 0;

    public int area;

    public List<BuffDebuff> buffDebuff;

    public List<EnumCustom.Attribute> attributeInfluence;

    public Sprite icon;

    public GameObject spellCastObject;

    public int manaCost;

    public EnumCustom.SpecialEffect specialEffect;

    public int specialEffectDuration = 0;

    public int GetValue()
    {
        int value = 0;

        if(fixedValue == 0)
            value = UnityEngine.Random.Range(minValue, maxValue + 1);
        else
            value = fixedValue;

        return value;
    }

    public void CastBuff(CharacterController controller)
    {
        if (spellType == EnumCustom.SpellType.Buff)
        {
            foreach (var aux in this.buffDebuff)
            {
                if (aux.buffDebuffType == EnumCustom.BuffDebuffType.Attribute)
                {
                    controller.CharacterStatus.attributeStatus.AddModifier(new AttributeModifier()
                    {
                        spellName = spellName,
                        attribute = aux.attribute,
                        count = aux.turnDuration,
                        value = aux.value
                    }, null);
                }
                if (aux.buffDebuffType == EnumCustom.BuffDebuffType.Status)
                {
                    controller.CharacterStatus.attributeStatus.AddModifier(null, new StatusModifier()
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
        controller.CharacterStatus.Mp -= manaCost;
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

    public void AnimateCastAreaSpell(Vector3 position)
    {
        GameObject spellCreated = Instantiate(spellCastObject, position + Vector3.up * 0.25f, Quaternion.identity);
        Destroy(spellCreated, 1.0f);
    }
}
