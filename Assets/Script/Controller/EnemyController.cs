using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BotController
{
    private EnemyManager enemyManager;
    public Enemy enemy;

    private CharacterController player;

    public int range = 1;

    public override void Start()
    {
        player = Manager.Instance.characterController;

        enemyManager = Manager.Instance.enemyManager;
        enemyManager.enemies.Add(this);

        level = Mathf.Clamp(Manager.Instance.characterController.level - 1, 1, Manager.Instance.characterController.level);
        attributeStatus = new AttributeStatus(level);

        if (!gameManager.DetectLOS(gameManager.GetPath(currentTileIndex, player.CharacterMoveTileIsometric.controller.currentTileIndex)))
        {
            hasTarget = true;
        }

        base.Start();
    }


    public override void Update()
    {
        base.Update();

        //target = GetTarget(typeof(EnemyController), null, 10);
        /*if (!gameManager.creatures.Contains(this))
        {
            gameManager.creatures.Add(this);
        }
        else if(forceTarget == null)
        {
            if (gameManager.creatures.Contains(this))
            {
                gameManager.creatures.Remove(this);
            }
        }*/
    }

    public override IEnumerator StartMyTurn()
    {
        target = GetTarget(typeof(EnemyController), null, 10);
        yield return new WaitForSeconds(0.2f);

        yield return base.StartMyTurn();
    }

    public override void Defeat()
    {
        base.Defeat();

        Manager.Instance.characterController.CharacterStatus.AddExp(enemy.exp);
        Manager.Instance.canvasManager.LogMessage(nickname + " foi derrotado, <color=yellow>" + enemy.exp + "</color> exp ganha");
        this.transform.Find("HealthBar").gameObject.SetActive(false);
    }

}
