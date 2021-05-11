using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : MonoBehaviour
{
    protected GameManager gameManager;

    public AttributeStatus attributeStatus;

    public int hp;

    public bool hasTarget = false;
    public Animator animator;

    public Vector3Int currentTileIndex;

    public Vector3 offsetPosition = new Vector3(0,0.5f,0);

    private Vector3 movePosition;
    public float movementSpeed = 10;

    protected bool isDead;

    public string mainAnimation;

    public virtual void Start()
    {
        gameManager = Manager.Instance.gameManager;

        currentTileIndex = gameManager.tilemap.WorldToCell(this.transform.position);
        currentTileIndex.z = 0;

        this.transform.position = Manager.Instance.gameManager.tilemap.GetCellCenterWorld(currentTileIndex) + offsetPosition;
        movePosition = this.transform.position;

        animator = this.GetComponentInChildren<Animator>();
        animator.speed = 0.7f;
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
        if (!isDead)
        {
            PlayAnimation("Dead", gameManager.GetDirection(currentTileIndex, currentTileIndex));
            isDead = true;
            gameManager.creatures.Remove(this.gameObject);
            gameManager.currentCreature--;
            gameManager.EndMyTurn();
        }
    }

    public void Attack(CharacterCombat characterCombat = null, BotController botController = null)
    {
        if (isDead)
        {
            return;
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
            botController.ReceiveHit(str);
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
