using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hp_Regen : SpecialSpell
{
    public override void Cast(CreatureController creatureController, int value)
    {
        base.Cast(creatureController, value);
        creatureController.Hp += value;
    }


    public Hp_Regen(SpecialSpell specialSpell) : base(specialSpell.duration, specialSpell.value, specialSpell.caster, specialSpell.target, specialSpell.tile, specialSpell.effect, specialSpell.logName)
    {
        AddToSpecialSpellList(this);
    }
}
