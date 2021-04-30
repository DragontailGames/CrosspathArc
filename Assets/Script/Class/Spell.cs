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

    public int availableAt = 1;

    public int minDamage;

    public int maxDamage;

    public Sprite icon;

    public GameObject spellObject;

    public int manaCost;

    public void Cast()
    {

    }
}
