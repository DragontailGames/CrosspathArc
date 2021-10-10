using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SpellAfterDelay
{
    public int turnsRemain = 0;

    public UnityAction action;

    public CharacterCombat characterCombat;

    public SpellAfterDelay(int _turnsTotal, UnityAction _action, CharacterCombat _characterCombat)
    {
        this.turnsRemain = _turnsTotal;
        this.action = _action;
        this.characterCombat = _characterCombat;
        this.characterCombat.spellAfterDelays.Add(this);
    }

    public void EndTurn()
    {
        turnsRemain--;
        if(turnsRemain<=0)
        {
            action?.Invoke();
            this.characterCombat.spellAfterDelays.Remove(this);
        }
    }
}
