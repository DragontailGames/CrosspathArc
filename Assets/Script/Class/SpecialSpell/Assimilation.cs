using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assimilation : SpecialSpell
{
    public Assimilation(SpecialSpell specialSpell) : base(specialSpell.duration, specialSpell.value, specialSpell.caster, specialSpell.target, specialSpell.effect, specialSpell.logName)
    {
        duration = specialSpell.value;
        value = 0;
        AddToSpecialSpellList(this);
    }
}
