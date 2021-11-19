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

    public GameObject inventory;

    public MouseTipsManager mouseTipsManager;

    public InventoryManager inventoryManager;

    public SpellbookManager spellbookManager;

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

    public void ReStart()
    {
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

        foreach(var aux in Manager.Instance.characterController.attributeStatus.attributeModifiers)
        {
            var tempStatusLog = statusLogs.Find(n => n.text.Contains(aux.spellName) && n.text.Contains(aux.attribute.ToString()));
            if (tempStatusLog==null)
            {
                var newTempStatusLog = Instantiate(statusLogPrefab, statusLog.content);
                tempStatusLog = newTempStatusLog.GetComponent<TextMeshProUGUI>();
                statusLogs.Add(tempStatusLog);
            }
            SetupText(tempStatusLog, aux.spellName, aux.value, aux.attribute.ToString(), aux.count);
        }

        foreach (var aux in Manager.Instance.characterController.attributeStatus.statusModifiers)
        {
            if (aux.count < 140)
            {
                var tempStatusLog = statusLogs.Find(n => n.text.Contains(aux.spellName) && n.text.Contains(aux.status.ToString()));
                if (tempStatusLog == null)
                {
                    var newTempStatusLog = Instantiate(statusLogPrefab, statusLog.content);
                    tempStatusLog = newTempStatusLog.GetComponent<TextMeshProUGUI>();
                    statusLogs.Add(tempStatusLog);
                }
                SetupText(tempStatusLog, aux.spellName, aux.value, aux.status.ToString(), aux.count);
            }
        }

        foreach(var specialStatus in Manager.Instance.characterController.specialSpell)
        {
            SetupText(null, specialStatus.logName == "" ? specialStatus.effect.ToString(): specialStatus.logName, specialStatus.value, "", specialStatus.duration);
        }
    }

    public void StatusSpecial(string startText, int value, int duration)
    {
        var tempStatusLog = Manager.Instance.canvasManager.statusLogs.Find(n => n.text.Split('>')[1].StartsWith(startText));
        if (tempStatusLog == null)
        {
            var newTempStatusLog = Instantiate(statusLogPrefab, statusLog.content);
            tempStatusLog = newTempStatusLog.GetComponent<TextMeshProUGUI>();
            statusLogs.Add(tempStatusLog);
        }
        Manager.Instance.canvasManager.SetupText(tempStatusLog, startText, value, "", duration);
    }

    public void SetupText(TextMeshProUGUI tmpStatusText, string spellName, int value, string propName, int count)
    {
        TextMeshProUGUI statusTmp = tmpStatusText;

        if (statusTmp == null)
        {
            spellName = spellName.Replace("_", " ");
            statusTmp = statusLogs.Find(n => n.text.Split('>')[1].StartsWith(spellName));
            if (statusTmp == null)
            {
                var newTempStatusLog = Instantiate(statusLogPrefab, statusLog.content);
                statusTmp = newTempStatusLog.GetComponent<TextMeshProUGUI>();
                statusLogs.Add(statusTmp);
            }
        }
        if (value > 0)
        {
            statusTmp.text = $"<color=#{positiveColorValue}>{spellName} (+{value} {propName}) ({count})";
        }
        else if (value < 0)
        {
            statusTmp.text = $"<color=#{negativeColorValue}>{spellName} (-{value} {propName}) ({count})";
        }
        if(value == 0)
        {
            statusTmp.text = $"<color=#{positiveColorValue}>{spellName} ({count})";
        }
        else if(count == 0)
        {
            statusTmp.text = $"<color=#{positiveColorValue}>{spellName}";
        }
        else if (count < 0)
        {
            statusTmp.text = $"<color=#{positiveColorValue}>{spellName} {value}";
        }
    }

    public void RemoveLogText(string spellName)
    {
        if (string.IsNullOrEmpty(spellName) || statusLog == null)
            return;

        spellName = spellName.Replace("_", " ");
        var temp = statusLogs.Find(n => n.text.Split('>')[1].StartsWith(spellName));
        if (temp != null)
        {
            statusLogs.Remove(temp);
            DestroyImmediate(temp.gameObject);
        }
    }

    public void Rest_Btn()
    {
        Manager.Instance.gameManager.Rest();
    }

    public void InventoryOpen_Btn()
    {
        inventoryManager.OpenInventory();
    }
}
