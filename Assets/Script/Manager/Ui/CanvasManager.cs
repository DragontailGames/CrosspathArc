﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manager no canvas geral
/// </summary>
public class CanvasManager : MonoBehaviour
{
    private void Awake()
    {
        Manager.Instance.canvasManager = this;//Configura o canvas no manager geral
    }

    public Image expBar;//Barra de exp

    public ScrollRect combatLog;//Combat log que aparece na tela

    public GameObject combatLogPrefab;//Prefab da linha de log

    public Transform skillPanel;

    /// <summary>
    /// Carrega a barra de exp
    /// </summary>
    /// <param name="exp">exp atual</param>
    /// <param name="nextLevelExp">total de exp</param>
    public void SetupExpBar(float exp, float nextLevelExp)
    {
        float value = (float)(exp /nextLevelExp);
        expBar.fillAmount = value;
    }

    /// <summary>
    /// Carrega a mensagem no log
    /// </summary>
    /// <param name="text">mensagem a aparecer no log</param>
    public void LogMessage(string text)
    {
        GameObject log = Instantiate(combatLogPrefab, combatLog.content);
        log.GetComponent<TextMeshProUGUI>().text = text;
        Invoke("FixPositionScroll", 0.2f);
    }

    public void FixPositionScroll()
    {
        combatLog.normalizedPosition = new Vector2(0, 0);
    }

    public ScrollRect statusLog;//Status log que aparece na tela

    public GameObject statusLogPrefab;//Prefab da linha de status

    public List<TextMeshProUGUI> statusLogs;

    private string positiveColorValue = "7EE0F5";
    private string negativeColorValue = "F34242";

    public void UpdateStatus()
    {
        List<AttributeModifier> tempAttributeModifiers = new List<AttributeModifier>();
        List<StatusModifier> tempStatusModifier = new List<StatusModifier>();

        foreach(var aux in Manager.Instance.characterController.CharacterStatus.attributeStatus.attributeModifiers)
        {
            var tempStatusLog = statusLogs.Find(n => n.text.Split('>')[1].StartsWith(aux.spellName));
            if(tempStatusLog==null)
            {
                var newTempStatusLog = Instantiate(statusLogPrefab, statusLog.content);
                tempStatusLog = newTempStatusLog.GetComponent<TextMeshProUGUI>();
                statusLogs.Add(tempStatusLog);
            }
            SetupText(tempStatusLog, aux.spellName, aux.value, aux.attribute.ToString(), aux.count);
        }
        foreach (var aux in Manager.Instance.characterController.CharacterStatus.attributeStatus.statusModifiers)
        {
            var tempStatusLog = statusLogs.Find(n => n.text.Split('>')[1].StartsWith(aux.spellName));
            if (tempStatusLog == null)
            {
                var newTempStatusLog = Instantiate(statusLogPrefab, statusLog.content);
                tempStatusLog = newTempStatusLog.GetComponent<TextMeshProUGUI>();
                statusLogs.Add(tempStatusLog);
            }
            SetupText(tempStatusLog, aux.spellName, aux.value, aux.status.ToString(), aux.count);
        }
    }

    public void SetupText(TextMeshProUGUI tmpStatusText, string spellName, int value, string propName, int count)
    {
        if (value > 0)
        {
            tmpStatusText.text = $"<color=#{positiveColorValue}>{spellName} (+{value} {propName}) ({count})";
        }
        else if (value < 0)
        {
            tmpStatusText.text = $"<color=#{negativeColorValue}>{spellName} (-{value} {propName}) ({count})";
        }
    }

    public void RemoveLogText(string spellName)
    {
        var temp = statusLogs.Find(n => n.text.Split('>')[1].StartsWith(spellName));
        statusLogs.Remove(temp);
        DestroyImmediate(temp.gameObject);
    }
}