using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : MonoBehaviour
{
    protected GameManager gameManager;

    public AttributeStatus attributeStatus;

    public int hp;
    public Transform hpBar;
    private int maxHp;

    public bool hasTarget = false;
    public Animator animator;

    public Vector3Int currentTileIndex;

    public Vector3 offsetPosition = new Vector3(0,0.5f,0);

    private Vector3 movePosition;
    public float movementSpeed = 10;

    protected bool isDead;

    public string mainAnimation;

    public Transform target;

    public virtual void Start()
    {
        gameManager = Manager.Instance.gameManager;

        maxHp = hp;
        hpBar = this.transform.Find("HealthBar").GetChild(0);

        currentTileIndex = gameManager.tilemap.WorldToCell(this.transform.position);
        currentTileIndex.z = 0;

        this.transform.position = Manager.Instance.gameManager.tilemap.GetCellCenterWorld(currentTileIndex) + offsetPosition;
        movePosition = this.transform.position;

        animator = this.GetComponentInChildren<Animator>();
        animator.speed = 0.7f;
    }

    public virtual void Update()
    {
        var scale = hpBar.localScale;
        scale.x = Mathf.Clamp((float)hp / (float)maxHp, 0, 1);//Animação da barra de hp
        hpBar.localScale = scale;
    }

    public void FixedUpdate()
    {
        if (hp <= 0)//Correção temporaria
            return;

        this.transform.position = Vector3.MoveTowards(this.transform.position, movePosition, movementSpeed * Time.deltaTime);
        animator.SetBool("Walk", Vector3.Distance(this.transform.position, movePosition) > 0.05f && hp > 0);
    }

    public void Walk(Vector3Int playerPos, List<PathFind.Point> path)
    {
        if (isDead)
        {
            return;
        }

        Vector3Int dest = new Vector3Int(path[0].x, path[0].y, 0);

        if (Manager.Instance.gameManager.CheckHasBotInTile(dest))
        {
            return;
        }

        animator.SetBool("Walk", true);
        PlayAnimation("Walk", gameManager.GetDirection(currentTileIndex, dest));
        
        currentTileIndex = dest;

        movePosition = gameManager.tilemap.GetCellCenterWorld(currentTileIndex) + offsetPosition;
    }


    /// <summary>
    /// Executa quando morrer
    /// </summary>
    public virtual void Defeat()
    {
        hp = 0;
        if (!isDead)
        {
            PlayAnimation("Dead", gameManager.GetDirection(currentTileIndex, currentTileIndex));
            isDead = true;
            gameManager.creatures.Remove(this.gameObject);
            //gameManager.currentCreature--;
            //gameManager.EndMyTurn();
        }
    }

    public void Attack(CharacterController characterController = null, BotController botController = null)
    {
        if (isDead)
        {
            return;
        }

        CharacterCombat characterCombat = null;
        if(characterController != null)
        {
            characterCombat = characterController.CharacterCombat;
        }

        int hitChance = attributeStatus.GetValue(EnumCustom.Status.HitChance);
        int str = attributeStatus.GetValue(EnumCustom.Attribute.Str);
        int dodge = characterCombat != null ? characterCombat.CharacterController.CharacterStatus.attributeStatus.GetValue(EnumCustom.Status.Dodge):
            botController.attributeStatus.GetValue(EnumCustom.Status.Dodge);

        Vector3Int destTileIndex = characterCombat != null ? characterCombat.CharacterController.CharacterMoveTileIsometric.CurrentTileIndex :
            botController.currentTileIndex;

        PlayAnimation("Attack", gameManager.GetDirection(currentTileIndex, destTileIndex));

        if (!Combat.TryHit(hitChance, str, dodge, characterCombat?.CharacterController.CharacterStatus.nickname))
        {
            return;
        }

        if (characterCombat)
            characterCombat.GetHit(str, this);
        else
            botController.ReceiveHit(str, str.ToString());
    }

    public virtual IEnumerator StartMyTurn(bool enemy = true)
    {
        if (isDead)
        {
            yield break;
        }

        yield return new WaitForSeconds(0.2f);

        CharacterController characterController;
        BotController botController;

        if (!target)
        {
            gameManager.EndMyTurn();
            yield break;
        }

        target.TryGetComponent(out characterController);
        target.TryGetComponent(out botController);

        Vector3Int targetTileIndex = botController==null?characterController.CharacterMoveTileIsometric.CurrentTileIndex:botController.currentTileIndex;
        List<PathFind.Point> path = gameManager.GetPath(currentTileIndex, targetTileIndex);

        if (gameManager.DetectLOS(path))
        {
            hasTarget = false;
            gameManager.EndMyTurn();
            yield break;
        }

        hasTarget = true;
        yield return new WaitForSeconds(0.4f);

        int offsetDiagonal = (targetTileIndex.x != currentTileIndex.x && targetTileIndex.y != currentTileIndex.y) ? 2 : 1;
        if (Vector3.Distance(targetTileIndex, currentTileIndex) <= offsetDiagonal)
        {
            if(!(characterController != null && !enemy))
                Attack(characterController, botController);
        }
        else
        {
            Walk(targetTileIndex, path);
        }
        gameManager.EndMyTurn();
    }

    /// <summary>
    /// Inimigo sofrendo dano de hit
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="damageText"></param>
    public virtual void ReceiveHit(int damage, string damageText = "")
    {
        int armor = attributeStatus.GetValue(EnumCustom.Status.Armor);
        int trueDamage = Mathf.Clamp(damage - armor, 0, damage);
        hp -= trueDamage;

        if (hp <= 0)
        {
            Defeat();//mata o inimigo
        }
    }

    public void PlayAnimation(string animationName, string dir)
    {
        string ani = mainAnimation + "_" + animationName + "_" + dir;
        animator.Play(ani);
    }
}
