using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : SpecialSpell
{
    public override void EndOfDuration(CreatureController creatureController)
    {
        base.EndOfDuration(creatureController);
        GameObject.DestroyImmediate(spellObject);
    }

    public Wall(SpecialSpell specialSpell) : base(specialSpell.duration, specialSpell.value, specialSpell.caster, specialSpell.target, specialSpell.tile, specialSpell.effect, specialSpell.logName, specialSpell.spellObject)
    {
        if (Manager.Instance.gameManager.GetCreatureInTile(tile) == null)
        {
            if (spellObject?.GetComponent<WallController>() == null)
            {
                spellObject?.AddComponent<WallController>();
            }
            spellObject?.GetComponent<WallController>().CreateWall(tile, duration);
            //AddToSpecialSpellList(this);
        }
        else
        {
            GameObject.DestroyImmediate(spellObject);
            Manager.Instance.canvasManager.LogMessage("Wall precisa ser criada em um tile vazio");
        }
    }
}
