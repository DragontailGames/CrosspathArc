using System.Collections;
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

    public GameObject logPrefab;//Prefab da linha de log

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
        GameObject log = Instantiate(logPrefab, combatLog.content);
        log.GetComponent<TextMeshProUGUI>().text = text;
        Invoke("FixPositionScroll", 0.2f);
    }

    public void FixPositionScroll()
    {
        combatLog.normalizedPosition = new Vector2(0, 0);
    }
}
