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
    public GameObject aboutPlayer, status, attributes, playerBase, availablePointsStatus, availablePointsSkill;
    public ScrollRect skillScrollRect;
    public GameObject skillContentModel;

    public List<TextMeshProUGUI> attributesText = new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> statusText = new List<TextMeshProUGUI>();
    private List<GameObject> skillsContent = new List<GameObject>();

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

        CreateSkills(controller.CharacterCombat.skills);

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
    private void UpdateStatus()
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
    }

    public void CreateSkills(List<Skill> skills)
    {
        for (int i = 0; i < skills.Count; i++)
        {
            Skill aux = skills[i];
            GameObject tempSkillContent = Instantiate(skillContentModel, skillScrollRect.content);
            tempSkillContent.transform.Find("SkillName").GetComponent<TextMeshProUGUI>().text = aux.name;
            Transform skillLevel = tempSkillContent.transform.Find("SkillLevel");
            skillLevel.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = aux.level.ToString();
            var currentSkill = i;
            skillLevel.GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
            {
                controller.CharacterCombat.skills[currentSkill].level++;
                controller.CharacterStatus.AvailableSkillPoint--;
                UpdateSkill(skills[currentSkill], currentSkill);
                UpdateSkills();
                controller.CharacterCombat.SetupSpells();
            });
            skillsContent.Add(tempSkillContent);
        }
    }

    public void UpdateSkill(Skill skills, int index)
    {
        Transform skillLevel = skillsContent[index].transform.Find("SkillLevel");
        skillLevel.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = skills.level.ToString();
    }

    public void UpdateSkills()
    {
        availablePointsSkill.gameObject.SetActive(controller.CharacterStatus.AvailableSkillPoint > 0);
        availablePointsSkill.GetComponentInChildren<TextMeshProUGUI>().text = controller.CharacterStatus.AvailableSkillPoint.ToString();

        for (int i = 0; i < skillsContent.Count; i++)
        {
            GameObject aux = (GameObject)skillsContent[i];
            Transform skillLevel = aux.transform.Find("SkillLevel");
            skillLevel.GetChild(1).gameObject.SetActive(controller.CharacterStatus.AvailableSkillPoint > 0 && controller.CharacterCombat.skills[i].level < 10);
        }
    }
}
