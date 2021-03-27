using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private GameManager gameManager;
    private EnemyManager enemyManager;
    public Enemy enemy;

    public CharacterController player;
    public Animator animator;

    public Vector3 offsetPosition;

    public Transform hpBar;
    private int maxHp;

    bool[,] tilesmap;
    PathFind.Grid grid;
    public Vector3Int currentTileIndex;
    int width; 
    int height;
    private Vector3 movePosition;
    public float movementSpeed = 1;
    public int range = 1;

    public readonly string mainAnimation = "Wolf";

    private IEnumerator action;

    void Start()
    {
        maxHp = enemy.hp;

        gameManager = Manager.Instance.gameManager;
        enemyManager = Manager.Instance.enemyManager;
        enemyManager.enemies.Add(this);

        this.transform.position = Manager.Instance.gameManager.tilemap.GetCellCenterWorld(enemy.tilePos) + offsetPosition;
        movePosition = this.transform.position;

        currentTileIndex = enemy.tilePos;

        width = gameManager.tilemap.size.x;
        height = gameManager.tilemap.size.y;

        tilesmap = new bool[width , height];

        for(int x = 0; x<width;x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                bool pathEnable = gameManager.tilemap.HasTile(pos) && !gameManager.collisionTM.HasTile(pos);
                tilesmap[x, y] = pathEnable;
            }
        }

        grid = new PathFind.Grid(width, height, tilesmap);

        animator = this.GetComponentInChildren<Animator>();
        animator.speed = 0.7f;

        action = EnemyAction(player);
        StartCoroutine(action);
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
        var scale = hpBar.localScale;
        scale.x =  Mathf.Clamp((float)enemy.hp/ (float)maxHp, 0, 1);//Animação da barra de hp
        hpBar.localScale = scale;
    }

    private void FixedUpdate()
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, movePosition, movementSpeed * Time.deltaTime);
        if (Vector3.Distance(this.transform.position, movePosition) <= 0.05f && animator.GetBool("Walk"))
        {
            animator.SetBool("Walk", false);
        }
    }

    /// <summary>
    /// Executa se o inimigo morrer
    /// </summary>
    void Defeat()
    {
        Manager.Instance.characterController.CharacterStatus.AddExp(enemy.exp);
        Manager.Instance.canvasManager.LogMessage(enemy.name + " foi derrotado, <color=yellow>" + enemy.exp + "</color> exp ganha");
        //Destroy(this.gameObject);
        StopCoroutine(action);
        this.transform.Find("HealthBar").gameObject.SetActive(false);

        PlayAnimation("Dead", gameManager.GetDirection(currentTileIndex, currentTileIndex));
    }

    public IEnumerator EnemyAction(CharacterController characterController)
    {
        yield return new WaitForSeconds(1f);
        if (!gameManager.InPause)
        {
            if (Vector3Int.Distance(characterController.CharacterMoveTileIsometric.CurrentTileIndex, currentTileIndex) < 10)
            {
                Vector3Int playerTileIndex = characterController.CharacterMoveTileIsometric.CurrentTileIndex;
                int offsetDiagonal = (playerTileIndex.x != currentTileIndex.x && playerTileIndex.y != currentTileIndex.y) ? 2 : 1;
                if (Vector3.Distance(playerTileIndex, currentTileIndex) <= offsetDiagonal)
                {
                    Attack(characterController.CharacterCombat);
                }
                else 
                {
                    Walk(playerTileIndex);
                }
            }
        }

        action = EnemyAction(player);
        StartCoroutine(action);
    }

    public List<PathFind.Point> path = new List<PathFind.Point>();

    public bool Walk(Vector3Int playerPos)
    {
        PathFind.Point _from = new PathFind.Point(currentTileIndex.x, currentTileIndex.y);
        PathFind.Point _to = new PathFind.Point(playerPos.x, playerPos.y);

        path = PathFind.Pathfinding.FindPath(grid, _from, _to);

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

        if (!Combat.TryHit(hitChance, str, dodge))
        {
            return;
        }

        PlayAnimation("Attack", gameManager.GetDirection(currentTileIndex ,characterCombat.CharacterController.CharacterMoveTileIsometric.CurrentTileIndex));
        characterCombat.GetHit(str, this);
    }

    public void PlayAnimation(string animationName, string dir)
    {
        string ani = mainAnimation + "_" + animationName + "_" + dir;
        animator.Play(ani);
    }
}
