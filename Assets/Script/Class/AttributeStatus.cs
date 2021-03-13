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
    public int GetValue(Attributes attribute)
    {
        float value = attribute.value;
        foreach(var aux in attribute.modifier)
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
    public int GetValue(Status status)
    {
        int value = status.value;
        foreach (var aux in status.modifier)
        {
            value += aux;
        }

        //verifica qual o status que esta sendo verificado e aplica a formula
        switch(status.status)
        {
            case EnumCustom.Status.Armor:
                {
                    value += MathfCustom.CalculateStatusByPoints(GetAttribute(EnumCustom.Attribute.Agi), 2);//Corrige o atributo baseado na formula
                    break;
                }
            case EnumCustom.Status.CriticalHit:
                {
                    value += GetAttribute(EnumCustom.Attribute.Dex);//Corrige o atributo baseado na formula
                    break;
                }
            case EnumCustom.Status.Dodge:
                {
                    value += GetAttribute(EnumCustom.Attribute.Agi);//Corrige o atributo baseado na formula
                    break;
                }
            case EnumCustom.Status.HitChance:
                {
                    value += GetAttribute(EnumCustom.Attribute.Dex);//Corrige o atributo baseado na formula
                    break;
                }
            case EnumCustom.Status.HpRegen:
                {
                    value += MathfCustom.CalculateStatusByPoints(GetAttribute(EnumCustom.Attribute.Con), 4);//Corrige o atributo baseado na formula
                    break;
                }
            case EnumCustom.Status.MpRegen:
                {
                    value += MathfCustom.CalculateStatusByPoints(GetAttribute(EnumCustom.Attribute.Foc), 4);//Corrige o atributo baseado na formula
                    break;
                }
            case EnumCustom.Status.SpellDodge:
                {
                    value += GetAttribute(EnumCustom.Attribute.Wis);//Corrige o atributo baseado na formula
                    break;
                }
            case EnumCustom.Status.SpellHit:
                {
                    value += GetAttribute(EnumCustom.Attribute.Int);//Corrige o atributo baseado na formula
                    break;
                }
        }
        return value;
    }

    /// <summary>
    /// Retorna o valor do atributo passando o tipo dele
    /// </summary>
    /// <param name="attribute">Atributo a ser verificado</param>
    /// <returns>Valor do atributo</returns>
    public int GetAttribute(EnumCustom.Attribute attribute)
    {
        return Array.Find(attributes, n => n.attribute == attribute).value;
    }

    /// <summary>
    /// Retorna o status passando o tipo
    /// </summary>
    /// <param name="stat">tipo do status</param>
    /// <returns>Retorna o status</returns>
    public Status GetStatus(EnumCustom.Status stat)
    {
        return Array.Find(status, n => n.status == stat);
    }

    /// <summary>
    /// Calcula o max de hp
    /// </summary>
    /// <param name="level">Level atual do jogador</param>
    /// <returns>Retorna o maximo do hp</returns>
    public int GetMaxHP(int level)
    {
        return 10 +
            MathfCustom.CalculateStatusByPoints(level, 4) + 
            MathfCustom.CalculateStatusByPoints(GetAttribute(EnumCustom.Attribute.Str),4) +
            (GetAttribute(EnumCustom.Attribute.Con) * 2);
    }


    /// <summary>
    /// Calcula o max de mp
    /// </summary>
    /// <param name="level">Level atual do jogador</param>
    /// <returns>Retorna o maximo do mp</returns>
    public int GetMaxMP(int level)
    {
        return 10 + 
            MathfCustom.CalculateStatusByPoints(level, 4) +
            (GetAttribute(EnumCustom.Attribute.Foc) * 2);
    }
}
