using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat
{
    /// <summary>
    /// Calcula o critico
    /// </summary>
    /// <returns></returns>
    public static int Critical(int criticalHit)
    {
        int value = MathfCustom.GetDice(100);
        return value < (95 - criticalHit) ? 1 : 2;
    }

    /// <summary>
    /// Calcula se o hit acerta
    /// </summary>
    /// <param name="hitChance">Status com a chance de acerto</param>
    /// <param name="attribute">Atribute para ser calculado</param>
    /// <param name="dodge">Chance de dodge do openten</param>
    /// <returns></returns>
    public static bool TryHit(int hitChance, int attribute, int dodge)
    {
        int dice = MathfCustom.GetDice(20);

        int value = dice + hitChance + attribute - dodge;

        string message = "Tentando atacar: dado(" + dice + ") + " + hitChance + " + " + attribute + " - " + dodge + " = " + value;

        message += value >= 20 ? " <color=green>(acertou)</color>" : " <color=red>(errou)</color>";

        Manager.Instance.canvasManager.LogMessage(message);

        return value >= 20;
    }

}
