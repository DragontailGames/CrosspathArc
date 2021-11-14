using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowLog : SpecialSpell
{
    public ShowLog(SpecialSpell specialSpell) : base(specialSpell.duration, specialSpell.value, specialSpell.caster, specialSpell.target, specialSpell.tile, specialSpell.effect, specialSpell.logName)
    {
        string logs = $"{target.nickname}: Hp({target.Hp}/{target.attributeStatus.GetMaxHP(target.level)})";
        foreach (var aux in target.attributeStatus.attributes)
        {
            logs += $"{aux.attribute}({target.attributeStatus.GetValue(aux.attribute)}) ";
        }
        Manager.Instance.canvasManager.LogMessage(logs);
        AddToSpecialSpellList(this);
    }
}
