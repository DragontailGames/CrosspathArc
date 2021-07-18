using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sleep : SpecialSpell
{
    public Sleep(int duration, int value, CreatureController controller, EnumCustom.SpecialEffect effect) : base(duration, value, controller, effect)
    {
        AddToSpecialSpellList(this);
    }

    public Sleep(SpecialSpell specialSpell) : base(specialSpell.duration, specialSpell.value, specialSpell.controller, specialSpell.effect)
    {
        AddToSpecialSpellList(this);
    }

    public override void Cast(CreatureController creatureController, int value)
    {
        base.Cast(creatureController, value);
        creatureController.canMove = false;
    }

    public override void ReceiveHit(CreatureController attacker, CreatureController creatureTarget)
    {
        base.ReceiveHit(attacker, creatureTarget);

        creatureTarget.specialSpell.Remove(creatureTarget.specialSpell.Find(n => n.effect == EnumCustom.SpecialEffect.Sleep));
        creatureTarget.canMove = true;
    }
}
