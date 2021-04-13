using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private GameManager gameManager;
    private EnemyManager enemyManager;
    public Enemy enemy;

    public CharacterController player;
    public bool hasTarget = false;
    public Animator animator;

    public Vector3 offsetPosition;

    public Transform hpBar;
    private int maxHp;

    public Vector3Int currentTileIndex;
    private Vector3 movePosition;
    public float movementSpeed = 10;
    public int range = 1;

    public readonly string mainAnimation = "Wolf";

    void Start()
    {
        maxHp = enemy.hp;

        gameManager = Manager.Instance.gameManager;
        enemyManager = Manager.Instance.enemyManager;
        enemyManager.enemies.Add(this);

        enemy.tilePos = gameManager.tilemap.WorldToCell(this.transform.position);
        enemy.tilePos.z = 0;

        this.transform.position = Manager.Instance.gameManager.tilemap.GetCellCenterWorld(enemy.tilePos) + offsetPosition;
        movePosition = this.transform.position;

        currentTileIndex = enemy.tilePos;

        animator = this.GetComponentInChildren<Animator>();
        animator.speed = 0.7f;

        if (!gameManager.DetectLOS(gameManager.GetPath(currentTileIndex, player.CharacterMoveTileIsometric.CurrentTileIndex)))
        {
            hasTarget = true;
        }
    }

    /// <summary>
    /// Inimigo sofrendo dano de hit
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="damageText"></param>
    public void HitEnemy(int damage, string damageText)
    {
        int armor = enemy.attributeStatus.GetValue(EnumCustom.Status.Armor);
        int trueDamage = Mathf.Clamp(damage - armor, 0, damage);
        enemy.hp -= trueDamage;
        Manager.Instance.canvasManager.LogMessage(enemy.name + " sofreu " + damageText + " - " + armor + " = <color=red>" + trueDamage + "</color> de dano");//Manda mensagem do dano que o inimigo recebeu

        if(enemy.hp<=0)
        {
            Defeat();//mata o inimigo
        }
    }

    /// <summary>
    /// Inimigo sofrendo dano de speel
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="damageText"></param>
    public void SpellEnemy(int damage, string damageText)
    {
        enemy.hp -= damage;
        Manager.Instance.canvasManager.LogMessage(enemy.name + " sofreu " + damageText + " = <color=red>" + damage + "</color> de dano");

        if (enemy.hp <= 0)
        {
            Defeat();
        }
    }

    void Update()
    {
        if (enemy.hp <= 0)//Correção temporaria
            return;

        var scale = hpBar.localScale;
        scale.x = Mathf.Clamp((float)enemy.hp / (float)maxHp, 0, 1);//Animação da barra de hp
        hpBar.localScale = scale;

        if (Vector3Int.Distance(player.CharacterMoveTileIsometric.CurrentTileIndex, currentTileIndex) < 10)
        {
            if (!gameManager.creatures.Contains(this.gameObject))
            {
                gameManager.creatures.Add(this.gameObject);
            }
        }
        else
        {
            if (gameManager.creatures.Contains(this.gameObject))
            {
                gameManager.creatures.Remove(this.gameObject);
            }
        }
    }

    private void FixedUpdate()
    {
        if (enemy.hp <= 0)//Correção temporaria
            return;

        this.transform.position = Vector3.MoveTowards(this.transform.position, movePosition, movementSpeed * Time.deltaTime);
        animator.SetBool("Walk", Vector3.Distance(this.transform.position, movePosition) > 0.05f && enemy.hp>0);
    }

    /// <summary>
    /// Executa se o inimigo morrer
    /// </summary>
    void Defeat()
    {
        PlayAnimation("Dead", gameManager.GetDirection(currentTileIndex, currentTileIndex));
        Manager.Instance.characterController.CharacterStatus.AddExp(enemy.exp);
        Manager.Instance.canvasManager.LogMessage(enemy.name + " foi derrotado, <color=yellow>" + enemy.exp + "</color> exp ganha");
        //Destroy(this.gameObject);
        this.transform.Find("HealthBar").gameObject.SetActive(false);
        gameManager.creatures.Remove(this.gameObject);
        gameManager.EndMyTurn();
    }

    public IEnumerator StartMyTurn()
    {
        if(enemy.hp <= 0)
        {
            yield break;
        }

        Vector3Int playerTileIndex = player.CharacterMoveTileIsometric.CurrentTileIndex;
        List<PathFind.Point> path = gameManager.GetPath(currentTileIndex, playerTileIndex);

        if (gameManager.DetectLOS(path))
        {
            hasTarget = false;
            gameManager.EndMyTurn();
            yield break;
        }

        hasTarget = true;
        yield return new WaitForSeconds(0.4f);

        int offsetDiagonal = (playerTileIndex.x != currentTileIndex.x && playerTileIndex.y != currentTileIndex.y) ? 2 : 1;
        if (Vector3.Distance(playerTileIndex, currentTileIndex) <= offsetDiagonal)
        {
            Attack(player.CharacterCombat);
        }
        else
        {
            Walk(playerTileIndex, path);
        }
        gameManager.EndMyTurn();
    }

    public bool Walk(Vector3Int playerPos, List<PathFind.Point> path)
    {
        Vector3Int dest = new Vector3Int(path[0].x, path[0].y, 0);

        animator.SetBool("Walk", true);
        PlayAnimation("Walk", gameManager.GetDirection(currentTileIndex, dest));

        currentTileIndex = dest;

        movePosition = gameManager.tilemap.GetCellCenterWorld(currentTileIndex) + offsetPosition;

        return true;
    }

    public void Attack(CharacterCombat characterCombat)
    {
        int hitChance = enemy.attributeStatus.GetValue(EnumCustom.Status.HitChance);
        int str = enemy.attributeStatus.GetValue(EnumCustom.Attribute.Str);
        int dodge = characterCombat.CharacterController.CharacterStatus.attributeStatus.GetValue(EnumCustom.Status.Dodge);

        PlayAnimation("Attack", gameManager.GetDirection(currentTileIndex, characterCombat.CharacterController.CharacterMoveTileIsometric.CurrentTileIndex));

        if (!Combat.TryHit(hitChance, str, dodge, player.CharacterStatus.nickname))
        {
            return;
        }

        characterCombat.GetHit(str, this);
    }

    public void PlayAnimation(string animationName, string dir)
    {
        string ani = mainAnimation + "_" + animationName + "_" + dir;
        animator.Play(ani);
    }
}
