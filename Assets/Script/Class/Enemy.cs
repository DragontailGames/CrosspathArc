using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe com os atributos do inimigo
/// </summary>
[System.Serializable]
public class Enemy
{
    public string name;

    public Vector3Int tilePos;

    public int size;

    public AttributeStatus attributeStatus;

    public int hp;

    public int exp;
}
