using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CrusaderUI.Scripts;
using TMPro;

/// <summary>
/// Controle da interface do usuario
/// </summary>
public class CharacterInterface : MonoBehaviour
{
    public GameObject hpInterface, mpInterface;

    private int maxHp, maxMp;

    private HPFlowController hpFlowController, mpFlowController;

    private TextMeshProUGUI txtHp, txtMp;

    private CharacterStatus characterStatus;

    private void Start()
    {
        //Configura as varaiveis
        hpFlowController = hpInterface.GetComponentInChildren<HPFlowController>();
        mpFlowController = mpInterface.GetComponentInChildren<HPFlowController>();
        txtHp = hpInterface.GetComponentInChildren<TextMeshProUGUI>();
        txtMp = mpInterface.GetComponentInChildren<TextMeshProUGUI>();

        characterStatus = this.GetComponent<CharacterStatus>();
    }

    private void Update()
    {
        //Define as variaveis dinamicamente
        maxHp = characterStatus.attributeStatus.GetMaxHP(characterStatus.Level);
        maxMp = characterStatus.attributeStatus.GetMaxMP(characterStatus.Level);

        hpFlowController.SetValue((float)characterStatus.Hp/ (float)maxHp);
        mpFlowController.SetValue((float)characterStatus.Mp / (float)maxMp);

        txtHp.text = characterStatus.Hp + "/" + maxHp;
        txtMp.text = characterStatus.Mp + "/" + maxMp;
    }
}
