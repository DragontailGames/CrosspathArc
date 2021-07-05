using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterStatus : MonoBehaviour
{
    public AttributeStatus attributeStatus = new AttributeStatus();

    private int exp = 0;//pontos de experiencia atual

    private int nextLevelExp = 100;//pontos para o proximo nivel

    private int level = 1;//level atual

    private int availableStatusPoint = 0;//pontos de status para serem distribuído

    private int availableSkillPoint = 0;//pontos de status para serem distribuído

    [Tooltip("Regular a quantidade de tile base para regenerar")]
    public int totalTilesToRegen = 20;//quantidade de tiles base para regenerar

    private int tilesToRegenHp = 20;//contador de tiles para regenerar

    private int tilesToRegenMp = 20;//contador de tiles para regenerar

    [Tooltip("Valor para adicionar no total da exp")]
    public int addLevelTemp = 25;//Temporario para desenvolvimento

    private int hp, mp;//quantidade de hp e mp

    //Gets e Sets para melhor visualização do codigo
    public int Hp { get => this.hp; set => this.hp = Mathf.Clamp(value, 0, attributeStatus.GetMaxHP(Level)); }
    public int Mp { get => this.mp; set => this.mp = Mathf.Clamp(value, 0, attributeStatus.GetMaxMP(Level)); }
    public int Level { get => this.level; set => this.level = value; }
    public int AvailableStatusPoint { get => this.availableStatusPoint; set => this.availableStatusPoint = value; }
    public int AvailableSkillPoint { get => this.availableSkillPoint; set => this.availableSkillPoint = value; }
    public int NextLevelExp { get => this.nextLevelExp; set => this.nextLevelExp = value; }
    public int Exp { get => this.exp; set => this.exp = value; }

    public string nickname;//Apelido do jogador para ser exibido

    public UnityAction levelUpAction;//Ação para ser executada quando o jogador subir de nivel;

    public int hpRegen = 0;

    public int hpRegenDuration = 0;

    public void Start()
    {
        ResetHp_Mp();//Reseta o hp e o mp com base no max
        NextLevelExp = MathfCustom.TotalLevelExp(Level);//Calcula o total de exp necessario
        Manager.Instance.canvasManager.SetupExpBar(Exp, NextLevelExp);//Define a exp na barra visual

        tilesToRegenHp = totalTilesToRegen - attributeStatus.GetValue(EnumCustom.Status.HpRegen);//Reseta os tiles para regenerar o Hp
        tilesToRegenMp = totalTilesToRegen - attributeStatus.GetValue(EnumCustom.Status.MpRegen);//Reseta os tiles para regenerar o Mp

        Manager.Instance.timeManager.startNewTurnAction += () => StartNewTurn();
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))//Comando temporario para adionar a exp 
        {
            AddExp(addLevelTemp);
        }
    }

    /// <summary>
    /// Função que adiciona a exp ao total
    /// </summary>
    /// <param name="value">valor da exp</param>
    public void AddExp(int value)
    {
        Exp += value;//Adiociona o valor ao total de exp
        Manager.Instance.canvasManager.SetupExpBar(Exp, NextLevelExp);//Mostra exp na barra visual da UI
        if(Exp>=NextLevelExp)//Exp necessaria sobe de nivel
        {
            LevelUp();
        }
    }

    /// <summary>
    /// Sobe de nivel
    /// </summary>
    public void LevelUp()
    {
        Level++;
        AvailableStatusPoint++;
        AvailableSkillPoint++;
        ResetHp_Mp();
        levelUpAction?.Invoke();
        Exp -= NextLevelExp;
        NextLevelExp = MathfCustom.TotalLevelExp(Level);//Calcula o total de exp necessario para subir de nivel
        Manager.Instance.canvasManager.SetupExpBar(Exp, NextLevelExp);//Mostra exp na barra visual da UI
        Manager.Instance.canvasManager.LogMessage("<color=#f2af11><b>Subiu de Nivel !!!</b></color>");//Manda mensagem para o log
    }

    public void ResetHp_Mp()
    {
        Hp = attributeStatus.GetMaxHP(Level);//Define o hp para o max atual
        Mp = attributeStatus.GetMaxMP(Level);//Define o mp para o max atual
    }

    /// <summary>
    /// Função chamada quando o usuario se move um tile
    /// </summary>
    public void MoveOneTile()
    {
        //diminui um tile para regenerar se for 0 ele regenera e reseta o total de tiles
        tilesToRegenHp--;
        if(tilesToRegenHp<=0)
        {
            Hp += 1;
            tilesToRegenHp = totalTilesToRegen - attributeStatus.GetValue(EnumCustom.Status.HpRegen);
        }

        tilesToRegenMp--;
        if (tilesToRegenMp <= 0)
        {
            Mp += 1;
            tilesToRegenMp = totalTilesToRegen - attributeStatus.GetValue(EnumCustom.Status.MpRegen);
        }
    }

    public bool DropHP(int value)
    {
        Hp = Mathf.Clamp(Hp - value, 0, Hp);
        if(Hp<=0)
        {
            return false;
        }
        return true;
    }

    public void StartNewTurn()
    {
        hpRegenDuration = Mathf.Clamp(hpRegenDuration--, 0, hpRegenDuration);
        if(hpRegenDuration>0)
        {
            hp = Mathf.Clamp(hp + hpRegen, 0,attributeStatus.GetMaxHP(level));

            Manager.Instance.canvasManager.StatusSpecial("Hp Regen",hpRegen, hpRegenDuration);
        }
        else if(hpRegen>0)
        {
            hpRegen = 0;
            Manager.Instance.canvasManager.RemoveLogText("Hp Regen");
        }
    }
}