using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttributeInfluence
{
    public EnumCustom.Attribute attribute;

    public int value;

    [Tooltip("Usar valor negativo para dividido")]
    public int multiplier;

    public int GetValue(int baseValue)
    {
        float fullValue = 0;
        if (multiplier > 0)
        {
            fullValue += baseValue * (float)multiplier;
        }
        else if(multiplier<0)
        {
            fullValue += baseValue / Mathf.Abs((float)multiplier);
        }
        if(value>0)
        {
            fullValue += baseValue + value;
        }
        else if(value<0)
        {
            fullValue += baseValue - value;
        }

        return Mathf.CeilToInt(fullValue);
    }
}
