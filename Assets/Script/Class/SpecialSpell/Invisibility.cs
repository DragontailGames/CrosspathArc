using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invisibility : SpecialSpell
{
    public override void EndOfDuration(CreatureController creatureController)
    {
        base.EndOfDuration(creatureController);

        Color32 color = target.transform.GetChild(0).GetComponent<SpriteRenderer>().color;
        color.a = (byte)255;
        target.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;
    }

    public Invisibility(SpecialSpell specialSpell) : base(specialSpell.duration, specialSpell.value, specialSpell.caster, specialSpell.target, specialSpell.effect, specialSpell.logName)
    {
        clearAfterDoAttack = true;
        foreach (var aux in Manager.Instance.enemyManager.enemies)
        {
            if (aux.target == target)
            {
                aux.hasTarget = false;
                aux.target = null;
            }
        }
        Color32 color = target.transform.GetChild(0).GetComponent<SpriteRenderer>().color;
        color.a = (byte)100;
        target.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;

        AddToSpecialSpellList(this);
    }
}
