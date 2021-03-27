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

    private int availablePoint = 0;//pontos de status para serem distribuído

    [Tooltip("Regular a quantidade de tile base para regenerar")]
    public int totalTilesToRegen = 20;//quantidade de tiles base para regenerar

    private int tilesToRegenHp = 20;//contador de tiles para regenerar

    private int tilesToRegenMp = 20;//contador de tiles para regenerar

    [Tooltip("Valor para adicionar no total da exp")]
    public int addLevelTemp = 25;//Temporario para desenvolvimento

    private int hp, mp;//quantidade de hp e mp

    //Gets e Sets para melhor visualização do codigo
    public int Hp { get => this.hp; set => this.hp = value; }
    public int Mp { get => this.mp; set => this.mp = value; }
    public int Level { get => this.level; set => this.level = value; }
    public int AvailablePoint { get => this.availablePoint; set => this.availablePoint = value; }
    public int NextLevelExp { get => this.nextLevelExp; set => this.nextLevelExp = value; }
    public int Exp { get => this.exp; set => this.exp = value; }

    public string nickname;//Apelido do jogador para ser exibido

    public UnityAction levelUpAction;//Ação para ser executada quando o jogador subir de nivel;

    public void Start()
    {
        ResetHp_Mp();//Reseta o hp e o mp com base no max
        NextLevelExp = MathfCustom.TotalLevelExp(Level);//Calcula o total de exp necessario
        Manager.Instance.canvasManager.SetupExpBar(Exp, NextLevelExp);//Define a exp na barra visual

        tilesToRegenHp = totalTilesToRegen - attributeStatus.GetValue(EnumCustom.Status.HpRegen);//Reseta os tiles para regenerar o Hp
        tilesToRegenMp = totalTilesToRegen - attributeStatus.GetValue(EnumCustom.Status.MpRegen);//Reseta os tiles para regenerar o Mp
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))//Comando temporario para adionar a exp 
        {
            AddExp(addLevelTemp);
        }

        Hp = Mathf.Clamp(Hp, 0, attributeStatus.GetMaxHP(Level));//Define os limetes de hp
        Mp = Mathf.Clamp(Mp, 0, attributeStatus.GetMaxMP(Level));//Define os limetes de mp
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
        AvailablePoint++;
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
}