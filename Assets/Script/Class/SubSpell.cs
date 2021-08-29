using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SubSpell
{
    public int minValue;

    public int maxValue;

    public int fixedValue = 0;

    public EnumCustom.CastTarget castTarget;

    public EnumCustom.SpellType spellType;

    public int area;

    public List<BuffDebuff> buffDebuff;

    public List<AttributeInfluence> attributeInfluence;

    public EnumCustom.SpecialEffect specialEffect;

    public int duration = 0;

    public int invokeLimit = 0;

    public int GetValue(CreatureController creatureController)
    {
        int value = 0;

        if (fixedValue == 0)
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

    public void Cast(CreatureController caster, CreatureController target, Spell originalSpell)
    {
        if(castTarget == EnumCustom.CastTarget.Caster)
        {
            target = caster;
        }

        switch(spellType)
        {
            case EnumCustom.SpellType.Special:
                {
                    ParserCustom.SpellSpecialParser(new SpecialSpell(duration, GetValue(caster), caster, target, specialEffect));
                    break;
                }
            case EnumCustom.SpellType.Buff:
                {
                    originalSpell.CastBuff(target);
                    break;
                }
        }
    }
}
