using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Spell
{
    public SpellSO configSpell;

    public int cooldown;

    public bool locked;

    public void Cast(UnityAction action, CreatureController caster, CreatureController target, Vector3Int tile, List<CharacterMinions> minionCounts, bool onlyCast = false)
    {
        int value = configSpell.cooldownTurns;

        foreach (var aux in configSpell.attributeInfluenceCooldownTurns)
        {
            value -= aux.GetValue(caster);
        }
        cooldown = value;
        configSpell.Cast(action, caster, target, tile, minionCounts, onlyCast);
    }
}
