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

        if (!gameManager.DetectLOS(gameManager.GetPath(currentTileIndex, player.CharacterMoveTileIsometric.CurrentTileIndex)))
        {
            hasTarget = true;
        }

        base.Start();
    }


    public override void Update()
    {
        base.Update();
        if (Vector3Int.Distance(player.CharacterMoveTileIsometric.CurrentTileIndex, currentTileIndex) < 10)
        {
            target = GetTarget();
            if (!gameManager.creatures.Contains(this))
            {
                gameManager.creatures.Add(this);
            }
        }
        else if(forceTarget == null)
        {
            if (gameManager.creatures.Contains(this))
            {
                gameManager.creatures.Remove(this);
            }
        }
    }

    public override IEnumerator StartMyTurn()
    {
        target = GetTarget();
        yield return new WaitForSeconds(0.2f);

        yield return base.StartMyTurn();
    }

    public Transform GetTarget()
    {
        var creaturesWithoutEnemy = gameManager.creatures.FindAll(n => n.GetComponent<EnemyController>() == null);
        var shortestDistance = Mathf.Infinity;
        Transform smallDistance = null; 
        foreach (var aux in creaturesWithoutEnemy)
        {
            if (aux.specialSpell.Find(n => n.CheckType<Invisibility>())?.duration > 0)
            {
                return null;
            }
            var distance = Vector3.Distance(aux.transform.position, this.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                smallDistance = aux.transform;
            }
        }
        return smallDistance;
    }

    public override void Defeat()
    {
        base.Defeat();

        Manager.Instance.characterController.CharacterStatus.AddExp(enemy.exp);
        Manager.Instance.canvasManager.LogMessage(enemy.name + " foi derrotado, <color=yellow>" + enemy.exp + "</color> exp ganha");
        this.transform.Find("HealthBar").gameObject.SetActive(false);
    }

}
