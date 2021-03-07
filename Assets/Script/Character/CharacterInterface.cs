using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CrusaderUI.Scripts;
using TMPro;

public class CharacterInterface : MonoBehaviour
{
    public int maxHp, maxMp, currentHp, currentMp;

    public GameObject hpInterface, mpInterface;

    private HPFlowController hpFlowController, mpFlowController;

    private TextMeshProUGUI txtHp, txtMp;

    private void Start()
    {
        hpFlowController = hpInterface.GetComponentInChildren<HPFlowController>();
        mpFlowController = mpInterface.GetComponentInChildren<HPFlowController>();
        txtHp = hpInterface.GetComponentInChildren<TextMeshProUGUI>();
        txtMp = mpInterface.GetComponentInChildren<TextMeshProUGUI>();

        currentHp = maxHp;
        currentMp = maxMp;
    }

    private void Update()
    {
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
        currentMp = Mathf.Clamp(currentMp, 0, maxMp);

        hpFlowController.SetValue((float)currentHp/ (float)maxHp);
        mpFlowController.SetValue((float)currentMp / (float)maxMp);

        txtHp.text = currentHp + "/" + maxHp;
        txtMp.text = currentMp + "/" + maxMp;

    }
}
