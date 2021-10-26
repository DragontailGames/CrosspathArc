using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SupportStatus
{
    public string name;

    public EnumCustom.Status status;

    public EnumCustom.SpecialEffect effect;

    public float valuePerLevel;

    public void StartRound(CreatureController controller, Skill skill)
    {
        if (effect == EnumCustom.SpecialEffect.None)
        {
            float value = 0.1f + (valuePerLevel * skill.level);
            controller.attributeStatus.AddModifier(null, new StatusModifier() { status = status, count = 150, spellName = name, value = Mathf.FloorToInt(value) });
        }
    }

    public void IncreaseLevel(CreatureController controller, Skill skill)
    {
        StartRound(controller, skill);
        if (effect == EnumCustom.SpecialEffect.Aggro)
        {
            controller.Aggro -= Mathf.FloorToInt(valuePerLevel * skill.level );
        }
    }
}
