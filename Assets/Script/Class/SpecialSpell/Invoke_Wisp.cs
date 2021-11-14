using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invoke_Wisp : SpecialSpell
{
    public override void Cast(CreatureController creatureController, int value)
    {
        base.ChangeValue(-value);
    }

    public Invoke_Wisp(SpecialSpell specialSpell) : base(specialSpell.duration, specialSpell.value, specialSpell.caster, specialSpell.target, specialSpell.tile, specialSpell.effect, specialSpell.logName)
    {
        AddToSpecialSpellList(this);
    }
}
