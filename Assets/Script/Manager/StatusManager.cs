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

    public CharacterController character;

    public TextMeshProUGUI nameTMPro;

    private TextMeshProUGUI levelPlayerBaseTMPro, hpPlayerBaseTMPro, mpPlayerBaseTMPro;

    void Awake()
    {
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
                character.CharacterStatus.attributeStatus.attributes[tempI].value++;
                character.CharacterStatus.AvailableStatusPoint--;
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

        nameTMPro.text = character.CharacterStatus.nickname;

        levelPlayerBaseTMPro = playerBase.transform.Find("Level").GetChild(0).GetComponent<TextMeshProUGUI>();
        hpPlayerBaseTMPro = playerBase.transform.Find("Hp").GetChild(0).GetComponent<TextMeshProUGUI>();
        mpPlayerBaseTMPro = playerBase.transform.Find("Mp").GetChild(0).GetComponent<TextMeshProUGUI>();

        CreateSkills(character.CharacterCombat.skills);

        character.CharacterStatus.levelUpAction += UpdateStatus;
        character.CharacterStatus.levelUpAction += UpdateSkills;
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
            attributesText[i].text = character.CharacterStatus.attributeStatus.attributes[i].value.ToString();

            Button currentButton = attributePlusButton[i];
            currentButton.targetGraphic.enabled = character.CharacterStatus.AvailableStatusPoint > 0 && character.CharacterStatus.attributeStatus.attributes[i].value < 10;
        }
        for (int i = 0; i < statusText.Count; i++)
        {
            statusText[i].text = character.CharacterStatus.attributeStatus.GetValue(character.CharacterStatus.attributeStatus.status[i].status).ToString();
        }

        levelPlayerBaseTMPro.text = character.CharacterStatus.Level.ToString();
        hpPlayerBaseTMPro.text = character.CharacterStatus.attributeStatus.GetMaxHP(character.CharacterStatus.Level).ToString();
        mpPlayerBaseTMPro.text = character.CharacterStatus.attributeStatus.GetMaxMP(character.CharacterStatus.Level).ToString();

        availablePointsStatus.gameObject.SetActive(character.CharacterStatus.AvailableStatusPoint > 0);
        availablePointsStatus.GetComponentInChildren<TextMeshProUGUI>().text = character.CharacterStatus.AvailableStatusPoint.ToString();
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
                character.CharacterCombat.skills[currentSkill].level++;
                character.CharacterStatus.AvailableSkillPoint--;
                UpdateSkill(skills[currentSkill], currentSkill);
                UpdateSkills();
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
        availablePointsSkill.gameObject.SetActive(character.CharacterStatus.AvailableSkillPoint > 0);
        availablePointsSkill.GetComponentInChildren<TextMeshProUGUI>().text = character.CharacterStatus.AvailableSkillPoint.ToString();

        for (int i = 0; i < skillsContent.Count; i++)
        {
            GameObject aux = (GameObject)skillsContent[i];
            Transform skillLevel = aux.transform.Find("SkillLevel");
            skillLevel.GetChild(1).gameObject.SetActive(character.CharacterStatus.AvailableSkillPoint > 0 && character.CharacterCombat.skills[i].level < 10);
        }
    }
}
