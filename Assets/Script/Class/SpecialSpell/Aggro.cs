using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aggro : SpecialSpell
{
    public override void EndOfDuration(CreatureController creatureController)
    {
        base.EndOfDuration(creatureController);
        target.aggro += value;
    }

    public Aggro(int duration, int value, CreatureController caster, CreatureController target, EnumCustom.SpecialEffect effect) : base(duration, value, caster, target, effect)
    {
        AddToSpecialSpellList(this);
        target.aggro -= value;
    }

    public Aggro(SpecialSpell specialSpell) : base(specialSpell.duration, specialSpell.value, specialSpell.caster, specialSpell.target, specialSpell.effect)
    {
        AddToSpecialSpellList(this);
        target.aggro -= value;
    }
}
