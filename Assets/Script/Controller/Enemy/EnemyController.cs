using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private EnemyManager enemyManager;

    public Enemy enemy;

    public Vector3 offsetPosition;

    public Transform hpBar;

    private int maxHp;

    void Start()
    {
        maxHp = enemy.hp;

        enemyManager = Manager.Instance.enemyManager;
        enemyManager.enemies.Add(this);

        this.transform.position = Manager.Instance.gameManager.tilemap.GetCellCenterWorld(enemy.tilePos) + offsetPosition;
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

    /// <summary>
    /// Executa se o inimigo morrer
    /// </summary>
    void Defeat()
    {
        Manager.Instance.characterController.CharacterStatus.AddExp(enemy.exp);
        Manager.Instance.canvasManager.LogMessage(enemy.name + " foi derrotado, <color=yellow>" + enemy.exp + "</color> exp ganha");
        Destroy(this.gameObject);
    }
}
