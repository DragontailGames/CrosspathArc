using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuffDebuff
{
    public EnumCustom.Attribute attribute;

    public EnumCustom.Status status;

    public EnumCustom.SpecialEffect specialEffect;

    public EnumCustom.BuffDebuffType buffDebuffType;

    public int value;

    public int turnDuration;
}
