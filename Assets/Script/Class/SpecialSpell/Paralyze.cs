using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralyze : SpecialSpell
{
    public Paralyze(int duration, int value, CreatureController caster, CreatureController target, EnumCustom.SpecialEffect effect) : base(duration, value, caster, target, effect)
    {
        AddToSpecialSpellList(this);
        SetupSpell();
    }
    public Paralyze(SpecialSpell specialSpell) : base(specialSpell.duration, specialSpell.value, specialSpell.caster, specialSpell.target, specialSpell.effect)
    {
        AddToSpecialSpellList(this);
        SetupSpell();
    }

    public override void SetupSpell()
    {
        base.SetupSpell();
        target.canMove = false;
    }

    public override void EndOfDuration(CreatureController creatureController)
    {
        base.EndOfDuration(creatureController);
        target.canMove = true;
    }
}
