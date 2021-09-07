using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionController : BotController
{

    public int duration;

    public override void Start()
    {
        base.Start();
        Manager.Instance.timeManager.startNewTurnAction += () => { StartNewTurn(); };
    }

    public void StartNewTurn()
    {
        if(Hp<=0)
        {
            //gameManager.EndMyTurn();
            return;
        }
    }

    public override void Defeat()
    {
        base.Defeat();

        Manager.Instance.characterController.CharacterCombat.minionCounts.Find(n => n.creatures.Contains(this))?.creatures.Remove(this);
        Destroy(this.gameObject, 3.0f);
    }

    public override IEnumerator StartMyTurn(bool canStartTurn)
    {
        duration--;
        if (duration <= 0)
        {
            Defeat();
            hp = 0;
        }
        if (Hp<0)
        {
            gameManager.EndMyTurn(this);
            yield break;
        }
        target = GetTarget(null, typeof(EnemyController),10);
        if(target == null)
        {
            target = Manager.Instance.characterController;
            StartCoroutine(base.StartMyTurn());
        }
        else
        {
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(base.StartMyTurn());
        }

    }
}
