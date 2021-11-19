using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectSpellsController : MonoBehaviour
{
    public GameObject spellUiPrefab;

    private SpellbookSpellController selectedSpell1, selectedSpell2;

    private List<Spell> spells = new List<Spell>();

    private CharacterController characterController;

    private List<SpellbookSpellController> spellbookSpellControllers = new List<SpellbookSpellController>();

    private Transform content;

    public GameObject lastGameobject;

    private GameObject confirm;

    public void Setup()
    {
        content = this.transform.GetChild(0);
        characterController = Manager.Instance.characterController;

        foreach (var aux in characterController.CharacterCombat.skills)
        {
            foreach (var temp in aux.spells)
            {
                spells.Add(temp);
            }
        }
    }

    public void OnEnable()
    {
        if(content == null)
        {
            Setup();
        }

        SpellbookManager spellbookManager = FindObjectOfType<SpellbookManager>();

        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i<spells.Count;i++)
        {
            Spell spell = spells[i];
            GameObject auxSpellUi = Instantiate(spellUiPrefab, content);
            SpellbookSpellController spellbookSpellController = auxSpellUi.GetComponent<SpellbookSpellController>();
            bool locked = false;
            if (spell.locked == true || spell.configSpell.availableAt > characterController.CharacterCombat.skills.Find(n => n.spells.Contains(spell)).level || spell.cooldown>0)
            {
                locked = true;
            }
            spellbookSpellController.SetupSimpleSpell(spell, locked, () => { SelectSpell(spellbookSpellController);});
        }

        confirm = Instantiate(lastGameobject, content);
        confirm.GetComponent<Button>().onClick.AddListener(() => { ConfirmSelection(); });
    }

    public void SelectSpell(SpellbookSpellController spell)
    {
        if (selectedSpell1 == null)
        {
            selectedSpell1 = spell;
            selectedSpell1.transform.Find("selected").gameObject.SetActive(true);
        }
        else
        {
            if (selectedSpell1 == spell)
            {
                selectedSpell1.gameObject.transform.Find("selected").gameObject.SetActive(false);
                selectedSpell1 = null;
            }
            else if (selectedSpell2 == spell)
            {
                selectedSpell2.gameObject.transform.Find("selected").gameObject.SetActive(false);
                selectedSpell2 = null;
            }
            else
            {
                selectedSpell2 = spell;
                selectedSpell2.transform.Find("selected").gameObject.SetActive(true);
            }
        }

        if (selectedSpell1 != null && selectedSpell2 != null)
        {
            confirm.GetComponent<Button>().interactable = true;
        }
        else
        {
            confirm.GetComponent<Button>().interactable = false;
        }

        SetupAfter();
    }

    public void ConfirmSelection()
    {
        this.gameObject.SetActive(false);
        if (selectedSpell1.spell.configSpell.castTarget == EnumCustom.CastTarget.Enemy || selectedSpell2.spell.configSpell.castTarget == EnumCustom.CastTarget.Enemy || 
            selectedSpell1.spell.configSpell.castTarget == EnumCustom.CastTarget.Target || selectedSpell2.spell.configSpell.castTarget == EnumCustom.CastTarget.Target)
        {
            Manager.Instance.canvasManager.LogMessage("Selecione um inimigo para atacar...");
        }
        else if(selectedSpell1.spell.configSpell.castTarget == EnumCustom.CastTarget.Area || selectedSpell2.spell.configSpell.castTarget == EnumCustom.CastTarget.Area ||
            selectedSpell1.spell.configSpell.castTarget == EnumCustom.CastTarget.Area_Hazard || selectedSpell2.spell.configSpell.castTarget == EnumCustom.CastTarget.Area_Hazard ||
            selectedSpell1.spell.configSpell.castTarget == EnumCustom.CastTarget.Tile || selectedSpell2.spell.configSpell.castTarget == EnumCustom.CastTarget.Tile)
        {
            Manager.Instance.canvasManager.LogMessage("Selecione um tile para atacar...");
        }
        else
        {
            selectedSpell1.spell.Cast(null,Manager.Instance.characterController, Manager.Instance.characterController,Vector3Int.zero,null);
            selectedSpell2.spell.Cast(null, Manager.Instance.characterController, Manager.Instance.characterController, Vector3Int.zero, null);
            Manager.Instance.canvasManager.LogMessage($"As spells {selectedSpell1.spell.configSpell.spellName} e {selectedSpell2.spell.configSpell.spellName} foram usadas");
        }

        Manager.Instance.characterController.CharacterCombat.selectedSpell.Add(selectedSpell1.spell);
        Manager.Instance.characterController.CharacterCombat.selectedSpell.Add(selectedSpell2.spell);
    }

    public void SetupAfter()
    {
        if (selectedSpell1 != null && selectedSpell2 != null)
        {
            for (int i = 0; i < spells.Count; i++)
            {
                Spell auxSpell = spells[i];
                bool locked = true;

                if (auxSpell == selectedSpell1.spell || auxSpell == selectedSpell2.spell)
                {
                    locked = false;
                }
            }

            this.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Spells Selecionadas (2/2)";
        }
        else
        {
            for (int i = 0; i < spells.Count; i++)
            {
                Spell auxSpell = spells[i];
                bool locked = false;

                if (auxSpell.locked == true || auxSpell.configSpell.availableAt > characterController.CharacterCombat.skills.Find(n => n.spells.Contains(auxSpell)).level || auxSpell.cooldown > 0)
                {
                    locked = true;
                }
            }

            if(selectedSpell1 != null || selectedSpell2 != null)
            {
                this.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Spells Selecionadas (1/2)";
            }
            else
            {
                this.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Spells Selecionadas (0/2)";
            }
        }
    }
}
