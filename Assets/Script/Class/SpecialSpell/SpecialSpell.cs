using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpecialSpell
{
    public int duration;

    public int value;

    public CreatureController controller;

    public EnumCustom.SpecialEffect effect;

    public bool clearAfterReceiveHit = false;

    public bool clearAfterDoAttack = false;

    public SpecialSpell(int duration, int value, CreatureController controller, EnumCustom.SpecialEffect effect)
    {
        this.duration = duration;
        this.value = value;
        this.controller = controller;
        this.effect = effect;
    }

    public virtual void SetupSpell() {}

    public void AddToSpecialSpellList(SpecialSpell specialSpell)
    {
        var existingSpell = controller.specialSpell.Find(n => n.effect == this.effect);
        if (existingSpell != null)
        {
            existingSpell.duration = duration;
        }
        else
        {
            controller.specialSpell.Add(specialSpell);
        }
    }

    public virtual void StartNewTurn(CreatureController creatureController)
    {
        Cast(creatureController, value);
        duration--;
        if (duration<=0)
        {
            EndOfDuration(creatureController);
            creatureController.specialSpell.Remove(this);
        }
    }

    public virtual void Cast(CreatureController creatureController, int value) { }

    public virtual void EndOfDuration(CreatureController creatureController)
    {
        Manager.Instance.canvasManager.RemoveLogText(effect.ToString());
    }

    public virtual void ReceiveHit(CreatureController creatureDealer, CreatureController creatureTarget) 
    { 
        if (clearAfterReceiveHit)
        {
            duration = 0;
            EndOfDuration(creatureTarget);
        }
    }

    public virtual bool CheckType<T>()
    {
        return typeof(T) == this.GetType();
    }

    public virtual void HandleAttack(CreatureController creatureController)
    {
        if (clearAfterDoAttack)
        {
            duration = 0;
            EndOfDuration(creatureController);
        }
    }
}
