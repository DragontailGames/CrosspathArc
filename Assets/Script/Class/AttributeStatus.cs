using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

/// <summary>
/// Classe para tratar os atributos (str, win, ...) e os status (crit, hpRegen, ...)
/// </summary>
[System.Serializable]
public class AttributeStatus
{
    public Attributes[] attributes; //= new Attributes[Enum.GetNames(typeof(EnumCustom.Attribute)).Length];//array com os attributos

    public Status[] status;//= new Status[Enum.GetNames(typeof(EnumCustom.Status)).Length];//array com os status

    public float walkDelay = 0.5f;

    public float combatDelay = 1.0f;

    public List<AttributeModifier> attributeModifiers = new List<AttributeModifier>();

    public List<StatusModifier> statusModifiers = new List<StatusModifier>();

    public int fakeLife = 0;

    public int baseHp = 0;

    public int hpExtra, mpExtra;

    /// <summary>
    /// Construtor da classe com level
    /// </summary>
    public void SetupAttributeStatus(int _level)
    {
        int level = _level;
        for (int i = 0; i < 8; i++)
        {
            attributes[i] = new Attributes { attribute = (EnumCustom.Attribute)i, value = attributes[i].value != 0 ? attributes[i].value: 1 };//inicializa as variaveis com valor proporicional
            status[i] = new Status { status = (EnumCustom.Status)i, value = status[i].value != 0 ? status[i].value : 1 };//inicializa as variaveis com 1 de valor
        }

        if (_level <= 0) return;
        int distributeToLevel = Mathf.FloorToInt(8 / level);

        int countWhile = 0;
        while (level > 0)
        {
            attributes[countWhile].value += 1;
            status[countWhile].value += 1;
            if (countWhile<attributes.Length-1)
            {
                countWhile++;
            }
            else
            {
                countWhile = 0;
            }
            level--;
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

        foreach (var aux in attributeModifiers.FindAll(n => n.attribute == enumAttribute))
        {
            value += aux.value;
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

        foreach (var aux in statusModifiers.FindAll(n => n.status == enumStatus))
        {
            value += aux.value;
        }

        //verifica qual o status que esta sendo verificado e aplica a formula
        switch(enumStatus)
        {
            case EnumCustom.Status.Armor:
                {
                    value += MathfCustom.CalculateStatusByPoints(GetValue(EnumCustom.Attribute.Agi), 4);//Corrige o atributo baseado na formula
                    break;
                }
            case EnumCustom.Status.CriticalHit:
                {
                    value += GetValue(EnumCustom.Attribute.Dex);//Corrige o atributo baseado na formula
                    break;
                }
            case EnumCustom.Status.Dodge:
                {
                    value += MathfCustom.CalculateStatusByPoints(GetValue(EnumCustom.Attribute.Agi),3);//Corrige o atributo baseado na formula
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
        int maxHp = 10 +
            MathfCustom.CalculateStatusByPoints(level, 2) +
            MathfCustom.CalculateStatusByPoints(GetValue(EnumCustom.Attribute.Str), 2) +
            (GetValue(EnumCustom.Attribute.Con) * 3) +
            fakeLife + baseHp + hpExtra;

        return maxHp;
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
            (GetValue(EnumCustom.Attribute.Foc) * 4) + mpExtra;
    }

    public void StartNewTurn()
    {
        foreach(var aux in attributeModifiers.ToList())
        {
            aux.count--;
            if (aux.count <= 0)
            {
                Manager.Instance.canvasManager.RemoveLogText(aux.spellName);
                attributeModifiers.Remove(aux);
            }
        }
        foreach (var aux in statusModifiers.ToList())
        {
            aux.count--;
            if (aux.count <= 0)
            {
                Manager.Instance.canvasManager.RemoveLogText(aux.spellName);
                statusModifiers.Remove(aux);
            }
        }
    }

    public void AddModifier(AttributeModifier attributeModifier, StatusModifier statusModifier)
    {
        if(attributeModifier != null)
        {
            var mod = attributeModifiers.Find(n => n.spellName == attributeModifier.spellName && n.attribute == attributeModifier.attribute);
            if (mod != null)
            {
                mod.value = attributeModifier.value;
                mod.count = attributeModifier.count;
            }
            else
            {
                attributeModifiers.Add(attributeModifier);
            }
        }
        if(statusModifier != null)
        {
            var mod = statusModifiers.Find(n => n.spellName == statusModifier.spellName && n.status == statusModifier.status);
            if (mod != null)
            {
                mod.value = statusModifier.value;
                mod.count = statusModifier.count;
            }
            else
            {
                statusModifiers.Add(statusModifier);
            }
        }
    }

    public bool HasBuff(string spellName)
    {
        if(attributeModifiers.Find(n => n.spellName == spellName) != null || statusModifiers.Find(n => n.spellName == spellName) != null)
        {
            return true;
        }
        return false;
    }
}
