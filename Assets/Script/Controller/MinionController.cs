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
            Defeat();
        }
    }

    public override void Defeat()
    {
        base.Defeat();

        Manager.Instance.characterController.CharacterCombat.minionCounts.Find(n => n.creatures.Contains(this.gameObject)).creatures.Remove(this.gameObject);
        Destroy(this.gameObject, 1.0f);
    }

    public override IEnumerator StartMyTurn(bool enemy = false)
    {
        target = GetEnemy();
        if(target == null)
        {
            target = Manager.Instance.characterController.transform;
        }
        yield return new WaitForSeconds(0.2f);

        StartCoroutine(base.StartMyTurn(false));
    }

    public Transform GetEnemy()
    {
        for (int x = -10; x <= 10; x++)
        {
            for (int y = -10; y <= 10; y++)
            {
                Vector3Int tile = new Vector3Int(x,y,0);
                var eTile = Manager.Instance.enemyManager.CheckEnemyInTile(tile);
                if(eTile)
                {
                    return eTile.transform;
                }
            }
        }
        return null;
    }
}
