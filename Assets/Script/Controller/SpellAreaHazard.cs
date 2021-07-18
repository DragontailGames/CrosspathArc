using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpellAreaHazard : MonoBehaviour
{
    public int duration = 0;

    public int damage = 0;

    public Vector3Int position;

    public UnityAction action;

    public void Start()
    {
        action = () => StartRound();
        Manager.Instance.timeManager.startNewTurnAction += action;
    }

    public void StartRound()
    {
        duration--;
        if(duration<=0)
        {
            Manager.Instance.timeManager.startNewTurnAction -= action;
            Destroy(this.gameObject);
            return;
        }
        EnemyController enemy = Manager.Instance.enemyManager.CheckEnemyInTile(position);
        enemy?.ReceiveHit(null, damage, damage.ToString());
    }
}
