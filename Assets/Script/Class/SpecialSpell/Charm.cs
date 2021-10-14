using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charm : SpecialSpell
{
    CharacterMinions minion;

    public override void EndOfDuration(CreatureController creatureController)
    {
        base.EndOfDuration(creatureController);
        Manager.Instance.gameManager.creatures.Add(target.GetComponent<EnemyController>());
        target.GetComponent<EnemyController>().Setup(target.GetComponent<MinionController>());
        Manager.Instance.gameManager.creatures.Remove(target.GetComponent<MinionController>());
        target.GetComponent<EnemyController>().StartTurn();
        target.GetComponent<EnemyController>().enabled = true;
        MonoBehaviour.Destroy(target.GetComponent<MinionController>());
        caster.GetComponent<CharacterCombat>().minionCounts.Remove(minion);
    }

    public Charm(SpecialSpell specialSpell) : base(specialSpell.duration, specialSpell.value, specialSpell.caster, specialSpell.target, specialSpell.effect, specialSpell.logName)
    {
        AddToSpecialSpellList(this);
        Manager.Instance.gameManager.creatures.Remove(target.GetComponent<EnemyController>());
        target.GetComponent<EnemyController>().enabled = false;
        target.gameObject.AddComponent<MinionController>();
        target.GetComponent<MinionController>().Setup(target.GetComponent<EnemyController>());

        minion = new CharacterMinions()
        {
            creatures = new List<MinionController>()
                    {
                        target.GetComponent<MinionController>()
                    }
        };

        caster.GetComponent<CharacterCombat>().minionCounts.Add(minion);
        duration = value;
    }
}

