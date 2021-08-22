using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : SpecialSpell
{
    public Poison(int duration, int value, CreatureController controller, EnumCustom.SpecialEffect effect) : base(duration, value, controller, effect)
    {
        AddToSpecialSpellList(this);
    }
    public Poison(SpecialSpell specialSpell) : base(specialSpell.duration, specialSpell.value, specialSpell.controller, specialSpell.effect)
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
