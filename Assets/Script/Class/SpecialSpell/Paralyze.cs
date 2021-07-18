using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralyze : SpecialSpell
{
    public Paralyze(int duration, int value, CreatureController controller, EnumCustom.SpecialEffect effect) : base(duration, value, controller, effect)
    {
        AddToSpecialSpellList(this);
        SetupSpell();
    }
    public Paralyze(SpecialSpell specialSpell) : base(specialSpell.duration, specialSpell.value, specialSpell.controller, specialSpell.effect)
    {
        AddToSpecialSpellList(this);
        SetupSpell();
    }

    public override void SetupSpell()
    {
        base.SetupSpell();
        controller.canMove = false;
    }

    public override void EndOfDuration(CreatureController creatureController)
    {
        base.EndOfDuration(creatureController);
        controller.canMove = true;
    }
}
