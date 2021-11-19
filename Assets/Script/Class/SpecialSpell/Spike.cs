using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : SpecialSpell
{
    public override void ReceiveHit(CreatureController creatureDealer, CreatureController creatureTarget, bool isSpell)
    {
        base.ReceiveHit(creatureDealer, creatureTarget, isSpell);
        if (!isSpell)
        {
            creatureDealer.ReceiveHit(creatureTarget, value, value + " (Spike)", true);
        }
    }

    public Spike(SpecialSpell specialSpell) : base(specialSpell.duration, specialSpell.value, specialSpell.caster, specialSpell.target, specialSpell.tile, specialSpell.effect, specialSpell.logName)
    {
        AddToSpecialSpellList(this);
    }
}
