using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invisibility : SpecialSpell
{
    public override void EndOfDuration(CreatureController creatureController)
    {
        base.EndOfDuration(creatureController);

        Color32 color = controller.transform.GetChild(0).GetComponent<SpriteRenderer>().color;
        color.a = (byte)255;
        controller.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;
    }

    public Invisibility(int duration, int value, CreatureController controller, EnumCustom.SpecialEffect effect) : base(duration, value, controller, effect)
    {
        clearAfterDoAttack = true;
        foreach (var aux in Manager.Instance.enemyManager.enemies)
        {
            if (aux.target == controller)
            {
                aux.hasTarget = false;
                aux.target = null;
            }
        }
        Color32 color = controller.transform.GetChild(0).GetComponent<SpriteRenderer>().color;
        color.a = (byte)100;
        controller.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;
    }
}
