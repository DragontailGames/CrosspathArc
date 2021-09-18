using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterStatus : MonoBehaviour
{
    public CharacterController controller;

    private int exp = 0;//pontos de experiencia atual

    private int nextLevelExp = 100;//pontos para o proximo nivel

    private int availableStatusPoint = 0;//pontos de status para serem distribuído

    private int availableSkillPoint = 0;//pontos de status para serem distribuído

    [Tooltip("Regular a quantidade de tile base para regenerar")]
    public int totalTilesToRegen = 20;//quantidade de tiles base para regenerar

    private int tilesToRegenHp = 20;//contador de tiles para regenerar

    private int tilesToRegenMp = 20;//contador de tiles para regenerar

    [Tooltip("Valor para adicionar no total da exp")]
    public int addLevelTemp = 25;//Temporario para desenvolvimento

    public int AvailableStatusPoint { get => this.availableStatusPoint; set => this.availableStatusPoint = value; }
    public int AvailableSkillPoint { get => this.availableSkillPoint; set => this.availableSkillPoint = value; }
    public int NextLevelExp { get => this.nextLevelExp; set => this.nextLevelExp = value; }
    public int Exp { get => this.exp; set => this.exp = value; }

    public UnityAction levelUpAction;//Ação para ser executada quando o jogador subir de nivel;

    public int hpRegen = 0;

    public int hpRegenDuration = 0;

    public void Start()
    {
        ResetHp_Mp();//Reseta o hp e o mp com base no max
        NextLevelExp = MathfCustom.TotalLevelExp(controller.level);//Calcula o total de exp necessario
        Manager.Instance.canvasManager.SetupExpBar(Exp, NextLevelExp);//Define a exp na barra visual

        tilesToRegenHp = totalTilesToRegen - controller.attributeStatus.GetValue(EnumCustom.Status.HpRegen);//Reseta os tiles para regenerar o Hp
        tilesToRegenMp = totalTilesToRegen - controller.attributeStatus.GetValue(EnumCustom.Status.MpRegen);//Reseta os tiles para regenerar o Mp
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
            AddExp(0);
        }
    }

    /// <summary>
    /// Sobe de nivel
    /// </summary>
    public void LevelUp()
    {
        controller.level++;
        AvailableStatusPoint++;
        AvailableSkillPoint++;
        ResetHp_Mp();
        levelUpAction?.Invoke();
        Exp -= NextLevelExp;
        NextLevelExp = MathfCustom.TotalLevelExp(controller.level);//Calcula o total de exp necessario para subir de nivel
        Manager.Instance.canvasManager.SetupExpBar(Exp, NextLevelExp);//Mostra exp na barra visual da UI
        Manager.Instance.canvasManager.LogMessage("<color=#f2af11><b>Subiu de Nivel !!!</b></color>");//Manda mensagem para o log
    }

    public void ResetHp_Mp()
    {
        controller.Hp = controller.attributeStatus.GetMaxHP(controller.level);//Define o hp para o max atual
        controller.Mp = controller.attributeStatus.GetMaxMP(controller.level);//Define o mp para o max atual
    }

    /// <summary>
    /// Função chamada quando o usuario se move um tile
    /// </summary>
    public void StartTurn()
    {
        //diminui um tile para regenerar se for 0 ele regenera e reseta o total de tiles
        tilesToRegenHp--;
        if(tilesToRegenHp<=0)
        {
            controller.Hp += 1;
            tilesToRegenHp = totalTilesToRegen - controller.attributeStatus.GetValue(EnumCustom.Status.HpRegen);
        }

        tilesToRegenMp--;
        if (tilesToRegenMp <= 0)
        {
            controller.Mp += 1;
            tilesToRegenMp = totalTilesToRegen - controller.attributeStatus.GetValue(EnumCustom.Status.MpRegen);
        }
    }

    public bool DropHP(int value)
    {
        controller.Hp = Mathf.Clamp(controller.Hp - value, 0, controller.Hp);
        if(controller.Hp <=0)
        {
            return false;
        }
        return true;
    }
}