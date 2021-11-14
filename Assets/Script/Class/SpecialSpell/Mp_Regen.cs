using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mp_Regen : SpecialSpell 
{
    public override void Cast(CreatureController creatureController, int value)
    {
        base.Cast(creatureController, value);
        creatureController.Mp += value;
    }

    public Mp_Regen(SpecialSpell specialSpell) : base(specialSpell.duration, specialSpell.value, specialSpell.caster, specialSpell.target, specialSpell.tile, specialSpell.effect, specialSpell.logName)
    {
        AddToSpecialSpellList(this);
    }
}
