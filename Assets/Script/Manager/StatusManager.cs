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
    public GameObject aboutPlayer, status, attributes, playerBase, availablePoints;

    public List<TextMeshProUGUI> attributesText = new List<TextMeshProUGUI>();

    public List<TextMeshProUGUI> statusText = new List<TextMeshProUGUI>();

    public List<Button> attributePlusButton = new List<Button>();

    public CharacterStatus characterStatus;

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
                characterStatus.attributeStatus.attributes[tempI].value++; 
                characterStatus.AvailablePoint--;
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

        nameTMPro.text = characterStatus.nickname;

        levelPlayerBaseTMPro = playerBase.transform.Find("Level").GetChild(0).GetComponent<TextMeshProUGUI>();
        hpPlayerBaseTMPro = playerBase.transform.Find("Hp").GetChild(0).GetComponent<TextMeshProUGUI>();
        mpPlayerBaseTMPro = playerBase.transform.Find("Mp").GetChild(0).GetComponent<TextMeshProUGUI>();

        characterStatus.levelUpAction += UpdateStatus;
    }

    private void OnEnable()
    {
        UpdateStatus();
    }

    /// <summary>
    /// Atualiza os status na ui, para evitar chamada a todo segundo
    /// </summary>
    private void UpdateStatus()
    {
        for (int i = 0; i < attributesText.Count; i++)
        {
            attributesText[i].text = characterStatus.attributeStatus.attributes[i].value.ToString();

            Button currentButton = attributePlusButton[i];
            currentButton.targetGraphic.enabled = characterStatus.AvailablePoint > 0 && characterStatus.attributeStatus.attributes[i].value < 10;
        }
        for (int i = 0; i < statusText.Count; i++)
        {
            statusText[i].text = characterStatus.attributeStatus.GetValue(characterStatus.attributeStatus.status[i].status).ToString();
        }

        levelPlayerBaseTMPro.text = characterStatus.Level.ToString();
        hpPlayerBaseTMPro.text = characterStatus.attributeStatus.GetMaxHP(characterStatus.Level).ToString();
        mpPlayerBaseTMPro.text = characterStatus.attributeStatus.GetMaxMP(characterStatus.Level).ToString();

        availablePoints.gameObject.SetActive(characterStatus.AvailablePoint > 0);
        availablePoints.GetComponentInChildren<TextMeshProUGUI>().text = characterStatus.AvailablePoint.ToString();
    }
}
