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
        float fullValue = baseValue += value;
        if (multiplier > 0)
        {
            fullValue *= (float)multiplier;
        }
        else if(multiplier<0)
        {
            fullValue /= Mathf.Abs((float)multiplier);
        }

        return Mathf.CeilToInt(fullValue);
    }
}
