using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SpecialSpell
{
    public string logName = "";

    public int duration;

    public int value;

    public Vector3Int tile;

    public CreatureController caster;

    public CreatureController target;

    public EnumCustom.SpecialEffect effect;

    public bool clearAfterReceiveHit = false;

    public bool clearAfterDoAttack = false;

    public GameObject spellObject = null;

    public ItemSO itemSO;

    public SpecialSpell(int duration, int value, CreatureController caster, CreatureController target, Vector3Int tile, EnumCustom.SpecialEffect effect, string logName, GameObject spellObject = null, ItemSO itemSO = null)
    {
        this.logName = logName;
        this.duration = duration;
        this.value = value;
        this.caster = caster;
        this.target = target;
        this.effect = effect;
        this.tile = tile;
        this.spellObject = spellObject;
        this.itemSO = itemSO;

        if (duration == 0)
        {
            Cast(target, value);
            EndOfDuration(target);
        }
    }

    public virtual void SetupSpell() {}

    public void AddToSpecialSpellList(SpecialSpell specialSpell)
    {
        if (duration > 0)
        {
            var existingSpell = target?.specialSpell.Find(n => n.effect == this.effect);
            if (existingSpell != null)
            {
                existingSpell.duration = duration;
                existingSpell.value = value;
            }
            else
            {
                target.specialSpell.Add(specialSpell);
            }
        }
        else if(duration == -1)
        {
            var existingSpell = target.specialSpell.Find(n => n.effect == this.effect);
            if (existingSpell != null)
            {
                existingSpell.value = value;
            }
            else
            {
                target.specialSpell.Add(specialSpell);
            }
        }
    }

    public bool ExistingInSpecialSpellList(SpecialSpell specialSpell)
    {
        return target.specialSpell.Find(n => n.effect == this.effect) == null;
    }

    public virtual void StartNewTurn(CreatureController creatureController)
    {
        if (duration > 0)
        {
            Cast(creatureController, value);
            duration--;
            if (duration <= 0)
            {
                EndOfDuration(creatureController);
                creatureController.specialSpell.Remove(this);
            }
        }
    }

    public virtual void Cast(CreatureController creatureController, int value) { }

    public virtual void EndOfDuration(CreatureController creatureController)
    {
        creatureController.specialSpell.Remove(this);
        Manager.Instance.canvasManager.RemoveLogText(logName != "" ?logName:effect.ToString());
    }

    public virtual void ReceiveHit(CreatureController creatureDealer, CreatureController creatureTarget, bool isSpell) 
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

    public virtual void ChangeValue(int value)
    {
        var sSpellAux = target.specialSpell.Find(n => n.effect == this.effect);
        if(sSpellAux != null)
        {
            sSpellAux.value += value;
        }
        if(sSpellAux.value<=0)
        { 
            target.specialSpell.Remove(sSpellAux);
            Manager.Instance.canvasManager.RemoveLogText(logName != "" ? logName : effect.ToString());
        }
    }
}
