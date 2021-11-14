using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fast_Cast : SpecialSpell
{
    public Fast_Cast(SpecialSpell specialSpell) : base(specialSpell.duration, specialSpell.value, specialSpell.caster, specialSpell.target, specialSpell.tile, specialSpell.effect, specialSpell.logName, specialSpell.spellObject)
    {

    }
}
