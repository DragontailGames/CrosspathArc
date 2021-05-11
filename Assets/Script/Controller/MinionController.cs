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

        gameManager.creatures.Add(this.gameObject);
    }

    public void StartNewTurn()
    {
        duration--;
        if(duration<=0)
        {
            //Die();
        }
    }
}
