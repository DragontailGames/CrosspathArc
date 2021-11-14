using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aggro : SpecialSpell
{
    public override void EndOfDuration(CreatureController creatureController)
    {
        base.EndOfDuration(creatureController);
        target.Aggro += value;
    }

    public Aggro(SpecialSpell specialSpell) : base(specialSpell.duration, specialSpell.value, specialSpell.caster, specialSpell.target, specialSpell.tile, specialSpell.effect, specialSpell.logName)
    {
        AddToSpecialSpellList(this);
        target.Aggro -= value;
    }
}
