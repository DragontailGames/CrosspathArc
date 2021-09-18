using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttributeInfluence
{
    public EnumCustom.Attribute attribute;

    public string levelOfSkill;

    public int value;

    [Tooltip("Usar valor negativo para dividido")]
    public int multiplier;

    public int GetValue(CreatureController controller)
    {
        if (value == 0)
            return 0;

        int baseValue = 0;

        if (string.IsNullOrEmpty(levelOfSkill) )
        {
            baseValue = controller.attributeStatus.GetValue(attribute);
        }
        else
        {
            baseValue = (controller as CharacterController).CharacterCombat.skills.Find(n => n.skill.skillName == levelOfSkill).level;
        }

        float fullValue = 0;
        if (multiplier > 0)
        {
            fullValue += baseValue * (float)multiplier;
        }
        else if (multiplier < 0)
        {
            fullValue += baseValue / Mathf.Abs((float)multiplier);
        }
        if (value > 0)
        {
            fullValue += baseValue + value;
        }
        else if (value < 0)
        {
            fullValue += baseValue - value;
        }


        Debug.Log("baseValue2 " + fullValue + " - " + baseValue);
        return Mathf.CeilToInt(fullValue != 0 ? fullValue : baseValue);
    }
}
