using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CreatureController : MonoBehaviour
{
    public GameManager gameManager;

    public AttributeStatus attributeStatus;

    public bool myTurn = true;

    public string animationName = "";

    public string direction;

    public int level = 1;//level atual

    private int hp, mp;//quantidade de hp e mp

    public bool canMove = true;

    public int Hp { get => this.hp; set => this.hp = Mathf.Clamp(value, 0, attributeStatus.GetMaxHP(level)); }
    public int Mp { get => this.mp; set => this.mp = Mathf.Clamp(value, 0, attributeStatus.GetMaxMP(level)); }

    public List<SpecialSpell> specialSpell = new List<SpecialSpell>();

    public virtual void Awake()
    {
        gameManager = Manager.Instance.gameManager;
    }

    public virtual IEnumerator StartMyTurn()
    {
        gameManager.StartNewTurn();

        foreach(var aux in specialSpell.ToList())
        {
            aux.StartNewTurn(this);
        }

        yield return new WaitForSeconds(0.1f);
        myTurn = true;

        //CharacterStatus.attributeStatus.StartNewTurn();
        Manager.Instance.canvasManager.UpdateStatus();
    }

    public string GetDirection(Vector3Int index)
    {
        if (index == new Vector3Int(1, 1, 0)) return "N";
        if (index == new Vector3Int(1, 0, 0)) return "NE";
        if (index == new Vector3Int(1, -1, 0)) return "E";
        if (index == new Vector3Int(0, -1, 0)) return "SE";
        if (index == new Vector3Int(-1, -1, 0)) return "S";
        if (index == new Vector3Int(-1, 0, 0)) return "SW";
        if (index == new Vector3Int(-1, 1, 0)) return "W";
        if (index == new Vector3Int(0, 1, 0)) return "NW";

        return "DirectionWrong";
    }

    public virtual void ReceiveDamage(CreatureController attacker)
    {
        foreach (var aux in specialSpell.ToList())
        {
            aux.ReceiveHit(attacker, this);
        }
    }

    [ContextMenu("Deal fake damage")]
    public void DealFakeDamage()
    {
        hp -= 5;
    }

}
