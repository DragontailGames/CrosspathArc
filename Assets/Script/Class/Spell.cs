using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe das spells do jogador
/// </summary>
[System.Serializable]
public class Spell
{
    public string name;

    public int minDamage;

    public int maxDamage;

    public EnumCustom.MagicSchool magicSchool;

    public Sprite icon;

    public GameObject spellObject;

    public int manaCost;
}
