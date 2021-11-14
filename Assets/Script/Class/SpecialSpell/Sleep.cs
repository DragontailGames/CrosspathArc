using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sleep : SpecialSpell
{
    public Sleep(SpecialSpell specialSpell) : base(specialSpell.duration, specialSpell.value, specialSpell.caster, specialSpell.target, specialSpell.tile, specialSpell.effect, specialSpell.logName)
    {
        clearAfterReceiveHit = true;
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
        //creatureTarget.specialSpell.Remove(creatureTarget.specialSpell.Find(n => n.effect == EnumCustom.SpecialEffect.Sleep));
        creatureTarget.canMove = true;
    }

    public override void EndOfDuration(CreatureController creatureController)
    {
        base.EndOfDuration(creatureController);
        creatureController.canMove = true;
    }
}
