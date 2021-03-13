using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe dos atributos
/// </summary>
[System.Serializable]
public class Attributes
{
    public string name;

    public EnumCustom.Attribute attribute;//tipo

    public int value;

    public List<int> modifier;//modificadores que são aplicados(buffs e debuff)
}
