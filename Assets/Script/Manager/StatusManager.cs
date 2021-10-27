using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Manager dos status na ui
/// </summary>
public class StatusManager : MonoBehaviour
{
    public GameObject aboutPlayer, status, attributes, playerBase, availablePointsStatus, availablePointsSkillMain, availablePointsSkillSupport;
    public ScrollRect skillScrollRect;
    public GameObject skillMainContentModel, skillSupportContentModel;
    public GameObject textModel;

    public List<TextMeshProUGUI> attributesText = new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> statusText = new List<TextMeshProUGUI>();
    private List<SkillUiController> skillsContent = new List<SkillUiController>();

    public List<Button> attributePlusButton = new List<Button>();

    private CharacterController controller;

    public TextMeshProUGUI nameTMPro;

    private TextMeshProUGUI levelPlayerBaseTMPro, hpPlayerBaseTMPro, mpPlayerBaseTMPro;

    void Awake()
    {
        controller = Manager.Instance.characterController;
        for (int i = 0; i < attributes.transform.childCount; i++)
        {
            //Pega as variaveis na ui dos atributos
            Transform attribute = attributes.transform.GetChild(i);
            attribute.GetComponent<TextMeshProUGUI>().text = ((EnumCustom.Attribute)i).ToString();

            attributesText.Add(attribute.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>());

            Button bt = attribute.GetChild(0).GetChild(1).GetComponent<Button>();
            int tempI = i;

            //Botao para adicionar pontos ao atributo
            bt.onClick.AddListener(() => 
            {
                controller.attributeStatus.attributes[tempI].value++;
                controller.CharacterStatus.AvailableStatusPoint--;
                UpdateStatus();
            });

            attributePlusButton.Add(bt);
        }
        for (int i = 0; i < status.transform.childCount; i++)
        {
            //Pega as variavies dos status
            Transform stats = status.transform.GetChild(i);
            stats.GetComponent<TextMeshProUGUI>().text = ((EnumCustom.Status)i).ToString();

            statusText.Add(stats.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>());
        }

        nameTMPro.text = controller.nickname;

        levelPlayerBaseTMPro = playerBase.transform.Find("Level").GetChild(0).GetComponent<TextMeshProUGUI>();
        hpPlayerBaseTMPro = playerBase.transform.Find("Hp").GetChild(0).GetComponent<TextMeshProUGUI>();
        mpPlayerBaseTMPro = playerBase.transform.Find("Mp").GetChild(0).GetComponent<TextMeshProUGUI>();

        CreateMainSkills(controller.CharacterCombat.skills);
        CreateSupportSkills(controller.CharacterCombat.supportSkills);

        controller.CharacterStatus.levelUpAction += UpdateStatus;
        controller.CharacterStatus.levelUpAction += UpdateSkills;
    }

    private void OnEnable()
    {
        UpdateStatus();
        UpdateSkills();
    }

    /// <summary>
    /// Atualiza os status na ui, para evitar chamada a todo segundo
    /// </summary>
    public void UpdateStatus()
    {
        for (int i = 0; i < attributesText.Count; i++)
        {
            attributesText[i].text = controller.attributeStatus.GetValue(controller.attributeStatus.attributes[i].attribute).ToString();

            Button currentButton = attributePlusButton[i];
            currentButton.targetGraphic.enabled = controller.CharacterStatus.AvailableStatusPoint > 0 && controller.attributeStatus.attributes[i].value < 10;
        }
        for (int i = 0; i < statusText.Count; i++)
        {
            statusText[i].text = controller.attributeStatus.GetValue(controller.attributeStatus.status[i].status).ToString();
        }

        levelPlayerBaseTMPro.text = controller.level.ToString();
        hpPlayerBaseTMPro.text = controller.attributeStatus.GetMaxHP(controller.level).ToString();
        mpPlayerBaseTMPro.text = controller.attributeStatus.GetMaxMP(controller.level).ToString();

        availablePointsStatus.gameObject.SetActive(controller.CharacterStatus.AvailableStatusPoint > 0);
        availablePointsStatus.GetComponentInChildren<TextMeshProUGUI>().text = controller.CharacterStatus.AvailableStatusPoint.ToString();

        availablePointsSkillMain.gameObject.SetActive(controller.CharacterStatus.AvailableSkillMainPoint > 0);
        availablePointsSkillMain.GetComponentInChildren<TextMeshProUGUI>().text = controller.CharacterStatus.AvailableSkillMainPoint.ToString();

        availablePointsSkillSupport.gameObject.SetActive(controller.CharacterStatus.AvailableSkillSupportPoint > 0);
        availablePointsSkillSupport.GetComponentInChildren<TextMeshProUGUI>().text = controller.CharacterStatus.AvailableSkillSupportPoint.ToString();
    }

    public void CreateMainSkills(List<Skill> skills)
    {
        GameObject tempTextModel = Instantiate(textModel, skillScrollRect.content);
        tempTextModel.GetComponent<TextMeshProUGUI>().text = "MAIN";

        for (int i = 0; i < skills.Count; i++)
        {
            Skill aux = skills[i];
            SkillUiController tempSkillUiController = Instantiate(skillMainContentModel, skillScrollRect.content).GetComponent<SkillUiController>();
            tempSkillUiController.textName.text = aux.name;
            tempSkillUiController.textLevel.text = aux.level.ToString();
            tempSkillUiController.skill = aux;
            tempSkillUiController.controller = controller;
            tempSkillUiController.statusManager = this;

            skillsContent.Add(tempSkillUiController);
        }
    }

    public void CreateSupportSkills(List<Skill> skills)
    {
        GameObject tempTextModel = Instantiate(textModel, skillScrollRect.content);
        tempTextModel.GetComponent<TextMeshProUGUI>().text = "Support";

        for (int i = 0; i < skills.Count; i++)
        {
            Skill aux = skills[i];
            SkillUiController tempSkillUiController = Instantiate(skillSupportContentModel, skillScrollRect.content).GetComponent<SkillUiController>();
            tempSkillUiController.textName.text = aux.name;
            tempSkillUiController.textLevel.text = aux.level.ToString();
            tempSkillUiController.skill = aux;
            tempSkillUiController.controller = controller;
            tempSkillUiController.statusManager = this;

            skillsContent.Add(tempSkillUiController);
        }
    }

    public void UpdateSkills()
    {
        for (int i = 0; i < skillsContent.Count; i++)
        {
            if (skillsContent[i].skill.skill.skillType == EnumCustom.SkillType.Spellbook)
            {
                skillsContent[i].gameObjectLevel.transform.GetChild(1).gameObject.SetActive(controller.CharacterStatus.AvailableSkillMainPoint > 0 && skillsContent[i].skill.level < 10);
            }
            else if (skillsContent[i].skill.skill.skillType == EnumCustom.SkillType.Support)
            {
                skillsContent[i].gameObjectLevel.transform.GetChild(1).gameObject.SetActive(controller.CharacterStatus.AvailableSkillSupportPoint > 0 && skillsContent[i].skill.level < 10);
            }
            else if (skillsContent[i].skill.skill.skillType == EnumCustom.SkillType.WeaponModifier)
            {
                skillsContent[i].gameObjectLevel.transform.GetChild(1).gameObject.SetActive(controller.CharacterStatus.AvailableSkillMainPoint > 0 && skillsContent[i].skill.level < 10);
            }
        }
    }
}
