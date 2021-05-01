using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Arc/Spell")]
public class Spell : ScriptableObject
{
    public string spellName;

    public int availableAt = 1;

    public int minDamage;

    public int maxDamage;

    public Sprite icon;

    public GameObject spellCastObject;

    public int manaCost;
}
