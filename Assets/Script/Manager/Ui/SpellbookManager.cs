using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SpellbookManager : MonoBehaviour
{
    public List<Skill> skills;
    public Spell[] selectedSpells = new Spell[10];

    public List<SpellbookSpellController> spellbookSpellControllers = new List<SpellbookSpellController>();

    public int indexSkill = 0;
    public int selectedIndex = -1;

    public GameObject assignMessage;

    public void OpenSpellbook()
    {
        this.gameObject.SetActive(true);
    }

    public void OnEnable()
    {
        Manager.Instance.gameManager.InPause = true;
        skills = new List<Skill>();
        foreach (var aux in Manager.Instance.characterController.CharacterCombat.skills)
        {
            if(aux.skill.spells.Count>0)
            {
                skills.Add(aux);
            }
        }
        SetupSpellbook(skills[indexSkill]);
    }

    public void Update()
    {
        if (selectedIndex != -1)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) SaveSpellInKeyboard(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SaveSpellInKeyboard(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SaveSpellInKeyboard(2);
            if (Input.GetKeyDown(KeyCode.Alpha4)) SaveSpellInKeyboard(3);
            if (Input.GetKeyDown(KeyCode.Alpha5)) SaveSpellInKeyboard(4);
            if (Input.GetKeyDown(KeyCode.Alpha6)) SaveSpellInKeyboard(5);
            if (Input.GetKeyDown(KeyCode.Alpha7)) SaveSpellInKeyboard(6);
            if (Input.GetKeyDown(KeyCode.Alpha8)) SaveSpellInKeyboard(7);
            if (Input.GetKeyDown(KeyCode.Alpha9)) SaveSpellInKeyboard(8);
            if (Input.GetKeyDown(KeyCode.Alpha0)) SaveSpellInKeyboard(9);
        }

        assignMessage.SetActive(selectedIndex != -1);
    }

    public void ChangeSpellbook(bool next)
    {
        if(next)
        {
            indexSkill++;
        }
        else
        {
            indexSkill--;
        }
        if(indexSkill>skills.Count-1)
        {
            indexSkill = 0;
        }
        else if(indexSkill<0)
        {
            indexSkill = skills.Count - 1;
        }
        SetupSpellbook(skills[indexSkill]);
    }

    public void SetupSpellbook(Skill skill)
    {
        Transform child = this.transform.GetChild(0);
        child.Find("Title").GetChild(0).GetComponent<TextMeshProUGUI>().text = skill.name;
        child.Find("Description").GetComponent<TextMeshProUGUI>().text = skill.description;
        selectedIndex = -1;
        spellbookSpellControllers.Clear();
        foreach (Transform aux in child.Find("Content"))
        {
            if(skill.skill.spells.Count>aux.GetSiblingIndex())
            {
                var tempSpellController = aux.GetComponent<SpellbookSpellController>();
                Spell spell = skill.skill.spells[aux.GetSiblingIndex()];
                spell.cooldown = 0;
                tempSpellController.SetupSpell(this, spell);
                spellbookSpellControllers.Add(tempSpellController);
            }
        }
    }

    public void SaveSpellInKeyboard(int index)
    {
        GameObject spellObject = this.transform.GetChild(0).Find("Content").GetChild(selectedIndex).gameObject;
        int hasInArray = Array.IndexOf(selectedSpells, spellbookSpellControllers[selectedIndex].spell);
        if (hasInArray != -1)
        {
            selectedSpells[hasInArray] = null;
        }
        selectedSpells[index] = spellbookSpellControllers[selectedIndex].spell;
        spellObject.transform.Find("selected").gameObject.SetActive(false);
        selectedIndex = -1;
    }

    public void Close()
    {
        Manager.Instance.characterController.CharacterCombat.SetSpells(new List<Spell>(selectedSpells));
        Manager.Instance.characterController.CharacterCombat.SetupSpells();
        this.gameObject.SetActive(false);
        Manager.Instance.gameManager.InPause = false;
        Manager.Instance.mouseTipsManager.HideMessage();
    }
}
