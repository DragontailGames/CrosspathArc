using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class SpellbookSpellController : MonoBehaviour
{
    public Spell spell;

    public SpellbookManager spellbookManager;

    public void SetupSpell(SpellbookManager spellbookManager, Spell spell)
    {
        this.spell = spell;
        this.spellbookManager = spellbookManager;
        this.transform.name = spell.name;
        this.GetComponent<Button>().onClick.RemoveAllListeners();
        this.GetComponent<Button>().onClick.AddListener(() => { SelectSpell(this.transform.GetSiblingIndex()); });
        this.transform.Find("icon").GetComponent<Image>().enabled = true;
        this.transform.Find("icon").GetComponent<Image>().sprite = spell.icon;

        this.transform.Find("Locked").gameObject.SetActive(spell.availableAt > spellbookManager.skills[spellbookManager.indexSkill].level);
    }

    public void SelectSpell(int index)
    {
        if (index == spellbookManager.selectedIndex)
        {
            spellbookManager.selectedIndex = -1;
        }
        else
        {
            spellbookManager.selectedIndex = index;
        }
    }

    public void Update()
    {
        if(spellbookManager != null && spell != null)
        {
            if(Array.Find(spellbookManager.selectedSpells, n => n == spell))
            {
                int key = Array.IndexOf(spellbookManager.selectedSpells, spell);
                this.transform.Find("shortcut").gameObject.SetActive(true);
                this.transform.Find("shortcut").GetChild(0).GetComponent<TextMeshProUGUI>().text = key < 9 ? (key +1).ToString() : "0";
            }
            else
            {
                this.transform.Find("shortcut").gameObject.SetActive(false);
            }

            if (spellbookManager.selectedIndex == this.transform.GetSiblingIndex())
            {
                this.transform.Find("selected").gameObject.SetActive(true);
            }
            else
            {
                this.transform.Find("selected").gameObject.SetActive(false);
            }
        }
    }
}
