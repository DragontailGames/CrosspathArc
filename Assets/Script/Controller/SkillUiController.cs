using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SkillUiController : MonoBehaviour
{
    public TextMeshProUGUI textName;

    public GameObject gameObjectLevel;

    public TextMeshProUGUI textLevel;

    public Skill skill;

    public Button button;

    public CharacterController controller;

    public StatusManager statusManager;

    public void Start()
    {
        StartRound();
        controller.startTurnActions += () => { StartRound(); };

        button.onClick.AddListener(() =>
        {
            skill.level++;
            textLevel.text = skill.level.ToString();

            if (skill.skill.skillType == EnumCustom.SkillType.Spellbook)
            {
                controller.CharacterStatus.AvailableSkillMainPoint--;
                statusManager.availablePointsSkillMain.gameObject.SetActive(controller.CharacterStatus.AvailableSkillMainPoint > 0);
                statusManager.availablePointsSkillMain.GetComponentInChildren<TextMeshProUGUI>().text = controller.CharacterStatus.AvailableSkillMainPoint.ToString();
            }
            else if (skill.skill.skillType == EnumCustom.SkillType.Support)
            {
                controller.CharacterStatus.AvailableSkillSupportPoint--;
                statusManager.availablePointsSkillSupport.gameObject.SetActive(controller.CharacterStatus.AvailableSkillSupportPoint > 0);
                statusManager.availablePointsSkillSupport.GetComponentInChildren<TextMeshProUGUI>().text = controller.CharacterStatus.AvailableSkillSupportPoint.ToString();
                foreach (var aux in skill.skill.support)
                {
                    aux.IncreaseLevel(controller, skill);
                }

                statusManager.UpdateStatus();
            }

            statusManager.UpdateSkills();
            controller.CharacterCombat.SetupSpells();
        });
    }

    public void StartRound()
    {
        foreach (var aux in skill.skill.support)
        {
            aux.StartRound(controller, skill);
        }
    }
    
}
