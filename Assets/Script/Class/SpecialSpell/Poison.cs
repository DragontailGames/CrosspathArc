using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : SpecialSpell
{
    public Poison(SpecialSpell specialSpell) : base(specialSpell.duration, specialSpell.value, specialSpell.caster, specialSpell.target, specialSpell.effect, specialSpell.logName)
    {
        AddToSpecialSpellList(this);
    }

    public override void Cast(CreatureController creatureController, int value)
    {
        base.Cast(creatureController, value);
        creatureController.ReceiveHit(creatureController, value, $"sofreu {value} de veneno", true);
        //creatureController.Hp -= value;
        //Manager.Instance.canvasManager.LogMessage($"{creatureController.name} sofreu {value} do veneno");//Manda mensagem do dano que o inimigo recebeu
    }
}
