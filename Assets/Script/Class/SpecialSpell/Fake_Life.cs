using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fake_Life : SpecialSpell
{
    public override void EndOfDuration(CreatureController creatureController)
    {
        base.EndOfDuration(creatureController);
        controller.attributeStatus.fakeLife = 0;
    }

    public Fake_Life(int duration, int value, CreatureController controller, EnumCustom.SpecialEffect effect) : base(duration, value, controller, effect)
    {
        controller.attributeStatus.fakeLife = value;
        AddToSpecialSpellList(this);
    }

    public Fake_Life(SpecialSpell specialSpell) : base(specialSpell.duration, specialSpell.value, specialSpell.controller, specialSpell.effect)
    {
        controller.attributeStatus.fakeLife = value;
        AddToSpecialSpellList(this);
    }
}
