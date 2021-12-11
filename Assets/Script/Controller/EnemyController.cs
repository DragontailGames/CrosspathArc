﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BotController
{
    private EnemyManager enemyManager;

    private CharacterController player;

    public List<ItemDrop> itemsToDrop = new List<ItemDrop>();


    public override void Start()
    {
        player = Manager.Instance.characterController;

        enemyManager = Manager.Instance.enemyManager;
        enemyManager.enemies.Add(this);

        level = Mathf.Clamp(Manager.Instance.characterController.level - 1, 1, Manager.Instance.characterController.level);
        attributeStatus.SetupAttributeStatus(level);

        if (!gameManager.DetectLOS(gameManager.GetPathForLOS(currentTileIndex, player.CharacterMoveTileIsometric.controller.currentTileIndex)))
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

    public void StartTurn()
    {
        StartCoroutine(StartMyTurn());
    }

    public override IEnumerator StartMyTurn(bool canStartTurn = true)
    {
        var newTarget = GetTarget(typeof(EnemyController), null);
        if (forceTarget)
        {
            if (newTarget)
            {
                target = newTarget;
            }
        }
        else
        {
            target = newTarget;
        }
        if(target !=null)
        {
            target.inCombat = true;
            this.inCombat = true;
        }
        else
        {
            this.inCombat = false;
        }
        //yield return new WaitForSeconds(0.2f);
        yield return base.StartMyTurn();
    }

    public override void Defeat()
    {
        GameObject bagAux = Instantiate(Manager.Instance.inventoryManager.bag, Manager.Instance.gameManager.tilemap.CellToWorld(this.currentTileIndex) + (Vector3.up * 0.25f), Quaternion.identity);
        bagAux.GetComponent<Bag>().items = Manager.Instance.inventoryManager.DropItem(itemsToDrop);

        base.Defeat();

        Manager.Instance.characterController.CharacterStatus.AddExp(exp);
        Manager.Instance.canvasManager.LogMessage(nickname + " foi derrotado, <color=yellow>" + exp + "</color> exp ganha");
        this.transform.Find("HealthBar").gameObject.SetActive(false);

    }

}
