using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Classe para tratar os atributos (str, win, ...) e os status (crit, hpRegen, ...)
/// </summary>
[System.Serializable]
public class AttributeStatus
{
    public Attributes[] attributes = new Attributes[Enum.GetNames(typeof(EnumCustom.Attribute)).Length];//array com os attributos

    public Status[] status = new Status[Enum.GetNames(typeof(EnumCustom.Status)).Length];//array com os status

    public float walkDelay = 0.5f;

    public float combatDelay = 1.0f;

    public List<Status> modifier = new List<Status>();

    /// <summary>
    /// Construtor da classe
    /// </summary>
    public AttributeStatus()
    {
        for (int i = 0; i < 8; i++)
        {
            attributes[i] = new Attributes {name=((EnumCustom.Attribute)i).ToString(), attribute = (EnumCustom.Attribute)i, value = 1 };//inicializa as variaveis com 1 de valor
            status[i] = new Status { status = (EnumCustom.Status)i, value = 1 };//inicializa as variaveis com 1 de valor
        }
    }

    /// <summary>
    /// Retorna o valor do attributo atual com suas variaveis
    /// </summary>
    /// <param name="attribute">Attributo a ser verificado</param>
    /// <returns>valor total do atributo</returns>
    public int GetValue(EnumCustom.Attribute enumAttribute)
    {
        Attributes currentAttribute = Array.Find(attributes, n => n.attribute == enumAttribute);
        float value = currentAttribute.value;
        foreach(var aux in currentAttribute.modifier)
        {
            value += aux;
        }
        return Mathf.RoundToInt(value);
    }

    /// <summary>
    /// Retorna o valor do status atual com suas variaveis
    /// </summary>
    /// <param name="status">Status a ser verificado</param>
    /// <returns>valor total do status</returns>
    public int GetValue(EnumCustom.Status enumStatus)
    {
        Status currentStatus = Array.Find(status, n => n.status == enumStatus);
        int value = currentStatus.value;
        List<Status> modifierTemp = modifier.FindAll(n => n.status == currentStatus.status);

        foreach (var aux in modifierTemp)
        {
            value += aux.value;
        }

        //verifica qual o status que esta sendo verificado e aplica a formula
        switch(enumStatus)
        {
            case EnumCustom.Status.Armor:
                {
                    value += MathfCustom.CalculateStatusByPoints(GetValue(EnumCustom.Attribute.Agi), 2);//Corrige o atributo baseado na formula
                    break;
                }
            case EnumCustom.Status.CriticalHit:
                {
                    value += GetValue(EnumCustom.Attribute.Dex);//Corrige o atributo baseado na formula
                    break;
                }
            case EnumCustom.Status.Dodge:
                {
                    value += GetValue(EnumCustom.Attribute.Agi);//Corrige o atributo baseado na formula
                    break;
                }
            case EnumCustom.Status.HitChance:
                {
                    value += GetValue(EnumCustom.Attribute.Dex);//Corrige o atributo baseado na formula
                    break;
                }
            case EnumCustom.Status.HpRegen:
                {
                    value += MathfCustom.CalculateStatusByPoints(GetValue(EnumCustom.Attribute.Con), 2);//Corrige o atributo baseado na formula
                    break;
                }
            case EnumCustom.Status.MpRegen:
                {
                    value += GetValue(EnumCustom.Attribute.Foc);//Corrige o atributo baseado na formula
                    break;
                }
            case EnumCustom.Status.SpellDodge:
                {
                    value += GetValue(EnumCustom.Attribute.Wis);//Corrige o atributo baseado na formula
                    break;
                }
            case EnumCustom.Status.SpellHit:
                {
                    value += GetValue(EnumCustom.Attribute.Int);//Corrige o atributo baseado na formula
                    break;
                }
        }
        return value;
    }

    /// <summary>
    /// Calcula o max de hp
    /// </summary>
    /// <param name="level">Level atual do jogador</param>
    /// <returns>Retorna o maximo do hp</returns>
    public int GetMaxHP(int level)
    {
        return 10 +
            MathfCustom.CalculateStatusByPoints(level, 2) + 
            MathfCustom.CalculateStatusByPoints(GetValue(EnumCustom.Attribute.Str),2) +
            (GetValue(EnumCustom.Attribute.Con) * 3);
    }


    /// <summary>
    /// Calcula o max de mp
    /// </summary>
    /// <param name="level">Level atual do jogador</param>
    /// <returns>Retorna o maximo do mp</returns>
    public int GetMaxMP(int level)
    {
        return 10 + 
            MathfCustom.CalculateStatusByPoints(level, 2) +
            (GetValue(EnumCustom.Attribute.Foc) * 4);
    }
}
