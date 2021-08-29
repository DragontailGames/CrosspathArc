using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : SpecialSpell
{
    public override void ReceiveHit(CreatureController creatureDealer, CreatureController creatureTarget)
    {
        base.ReceiveHit(creatureDealer, creatureTarget);
        creatureDealer.ReceiveHit(creatureTarget, value, value + " (Spike)", true);
    }

    public Spike(int duration, int value, CreatureController caster, CreatureController target, EnumCustom.SpecialEffect effect) : base(duration, value, caster, target, effect)
    {
        AddToSpecialSpellList(this);
    }

    public Spike(SpecialSpell specialSpell) : base(specialSpell.duration, specialSpell.value, specialSpell.caster, specialSpell.target, specialSpell.effect)
    {
        AddToSpecialSpellList(this);
    }
}
