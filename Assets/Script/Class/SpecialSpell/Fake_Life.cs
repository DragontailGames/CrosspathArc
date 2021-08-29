using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fake_Life : SpecialSpell
{
    public override void EndOfDuration(CreatureController creatureController)
    {
        base.EndOfDuration(creatureController);
        target.attributeStatus.fakeLife = 0;
    }

    public Fake_Life(int duration, int value, CreatureController caster, CreatureController target, EnumCustom.SpecialEffect effect) : base(duration, value, caster, target, effect)
    {
        target.attributeStatus.fakeLife = value;
        AddToSpecialSpellList(this);
    }

    public Fake_Life(SpecialSpell specialSpell) : base(specialSpell.duration, specialSpell.value, specialSpell.caster, specialSpell.target, specialSpell.effect)
    {
        target.attributeStatus.fakeLife = value;
        AddToSpecialSpellList(this);
    }
}
