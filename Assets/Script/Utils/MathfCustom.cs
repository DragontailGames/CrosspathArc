﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MathfCustom
{
    public static int Sign(float value)
    {
        return value < 0 ? -1 : (value > 0 ? 1 : 0);//retorna o sinal do numero informado (-1,1 ou 0)
    }

    public static Vector3Int Sign(Vector3Int value)
    {
        return new Vector3Int(Sign(value.x), Sign(value.y), Sign(value.z));//Retorna o sinal de um vector3
    }

    public static int CalculateStatusByPoints(int status, int byPoints)
    {
        return Mathf.FloorToInt(status / byPoints);
    }

    public static int TotalLevelExp(int level)
    {
        int expBase = 100;
        return Mathf.RoundToInt(expBase + ((expBase * 0.5f) * level));
    }

    public static int GetDice(int sides)
    {
        return (UnityEngine.Random.Range(1, sides+1));
    }

    public static int MathWithFormula(float vValue, string operation)
    {
        if(operation.Contains("/"))
        {
            var opFormula = operation.Split('/');
            if(opFormula[0] == "v")
                return Mathf.RoundToInt(vValue / int.Parse(opFormula[1]));
            else
                return Mathf.RoundToInt(int.Parse(opFormula[1]) / vValue);
        }
        if (operation.Contains("*"))
        {
            var opFormula = operation.Split('*');
            if (opFormula[0] == "v")
                return Mathf.RoundToInt(vValue * int.Parse(opFormula[1]));
            else
                return Mathf.RoundToInt(int.Parse(opFormula[1]) * vValue);
        }
        if (operation.Contains("+"))
        {
            var opFormula = operation.Split('+');
            if (opFormula[0] == "v")
                return Mathf.RoundToInt(vValue + int.Parse(opFormula[1]));
            else
                return Mathf.RoundToInt(int.Parse(opFormula[1]) + vValue);
        }
        if (operation.Contains("-"))
        {
            var opFormula = operation.Split('-');
            if (opFormula[0] == "v")
                return Mathf.RoundToInt(vValue - int.Parse(opFormula[1]));
            else
                return Mathf.RoundToInt(int.Parse(opFormula[1]) - vValue);
        }

        return 0;
    }
}
