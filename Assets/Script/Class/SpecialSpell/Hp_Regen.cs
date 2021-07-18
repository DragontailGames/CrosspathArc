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

    public Hp_Regen(int duration, int value, CreatureController controller, EnumCustom.SpecialEffect effect) : base(duration, value, controller, effect)
    {
        AddToSpecialSpellList(this);
    }

    public Hp_Regen(SpecialSpell specialSpell) : base(specialSpell.duration, specialSpell.value, specialSpell.controller, specialSpell.effect)
    {
        AddToSpecialSpellList(this);
    }
}
