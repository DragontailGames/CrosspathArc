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
    public CharacterController controller;

    public GameObject hpInterface, mpInterface;

    private int maxHp, maxMp;

    private HPFlowController hpFlowController, mpFlowController;

    private TextMeshProUGUI txtHp, txtMp;

    private void Start()
    {
        //Configura as varaiveis
        hpFlowController = hpInterface.GetComponentInChildren<HPFlowController>();
        mpFlowController = mpInterface.GetComponentInChildren<HPFlowController>();
        txtHp = hpInterface.GetComponentInChildren<TextMeshProUGUI>();
        txtMp = mpInterface.GetComponentInChildren<TextMeshProUGUI>();

        controller.startTurnActions += () => { this.StartTurn(); };
    }

    private void Update()
    {
        //Define as variaveis dinamicamente
        maxHp = controller.attributeStatus.GetMaxHP(controller.level);
        maxMp = controller.attributeStatus.GetMaxMP(controller.level);

        hpFlowController.SetValue((float)controller.Hp/ (float)maxHp);
        mpFlowController.SetValue((float)controller.Mp / (float)maxMp);

        txtHp.text = controller.Hp + "/" + maxHp;
        txtMp.text = controller.Mp + "/" + maxMp;
    }

    public void StartTurn()
    {
        for (int i = 0; i < 10; i++)
        {
            if (controller.spells.Count > i && controller.spells[i].configSpell != null && controller.spells[i].cooldown>0)
            {
                controller.CharacterCombat.spellUi[i].transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                controller.CharacterCombat.spellUi[i].transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }
}
