using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visibility : SpecialSpell
{
    int originalSize = 0;

    public override void EndOfDuration(CreatureController creatureController)
    {
        target.GetComponentInChildren<Light>().size = originalSize;
        target.GetComponentInChildren<Light>().SetupLightSize();

        base.EndOfDuration(creatureController);
    }

    public Visibility(SpecialSpell specialSpell) : base(specialSpell.duration, specialSpell.value, specialSpell.caster, specialSpell.target, specialSpell.effect, specialSpell.logName)
    {
        AddToSpecialSpellList(this);
        originalSize = target.GetComponentInChildren<Light>().size;
        target.GetComponentInChildren<Light>().size = value;
        target.GetComponentInChildren<Light>().SetupLightSize();
    }
}
