using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;

public class CharacterCombat : MonoBehaviour
{
    public CharacterController controller;

    public List<Skill> skills;

    public List<Skill> supportSkills;

    public List<GameObject> spellUi = new List<GameObject>();//Barra de spells

    public List<Spell> selectedSpell = new List<Spell>();//Skill que o jogador escolheu

    public Transform selectedUi;

    public List<CharacterMinions> minionCounts = new List<CharacterMinions>();

    public List<SpellAfterDelay> spellAfterDelays = new List<SpellAfterDelay>();

    /// <summary>
    /// Configs
    /// </summary>
    public void Start()
    {
        skills = skills.OrderBy(n => n.skill.skillName).ToList();
        supportSkills = supportSkills.OrderBy(n => n.skill.skillName).ToList();

        foreach(var aux in skills)
        {
            foreach(var spellSO in aux.skill.spells)
            {
                aux.spells.Add(new Spell() { configSpell = spellSO, cooldown = 0, locked = spellSO.unlockWhenKillThis != "" });
            }
        }
        foreach (Transform aux in Manager.Instance.canvasManager.skillPanel.GetChild(0))
        {
            spellUi.Add(aux.gameObject);
        }
        foreach (Transform aux in Manager.Instance.canvasManager.skillPanel.GetChild(1))
        {
            spellUi.Add(aux.gameObject);
        }

        SetupSpells();
    }

    public void Update()
    {
        if (!Manager.Instance.gameManager.InPause)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSkill(0);//Ativa a skill no slot
            if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSkill(1);//Ativa a skill no slot
            if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSkill(2);//Ativa a skill no slot
            if (Input.GetKeyDown(KeyCode.Alpha4)) SelectSkill(3);//Ativa a skill no slot
            if (Input.GetKeyDown(KeyCode.Alpha5)) SelectSkill(4);//Ativa a skill no slot
            if (Input.GetKeyDown(KeyCode.Alpha6)) SelectSkill(5);//Ativa a skill no slot
            if (Input.GetKeyDown(KeyCode.Alpha7)) SelectSkill(6);//Ativa a skill no slot
            if (Input.GetKeyDown(KeyCode.Alpha8)) SelectSkill(7);//Ativa a skill no slot
            if (Input.GetKeyDown(KeyCode.Alpha9)) SelectSkill(8);//Ativa a skill no slot
            if (Input.GetKeyDown(KeyCode.Alpha0)) SelectSkill(9);//Ativa a skill no slot
        }
    }

    public void SetupSpells()
    {
        for (int i = 0; i < 10; i++)
        {
            if (controller.spells.Count> i && controller.spells[i] != null && controller.spells[i].configSpell != null)
            {
                SpellSO spellSelected = controller.spells[i].configSpell;
                spellUi[i].GetComponent<Image>().sprite = spellSelected.icon;
                spellUi[i].GetComponent<Image>().enabled = true;
                Button btSpell;
                if (spellUi[i].GetComponent<Button>())
                {
                    btSpell = spellUi[i].GetComponent<Button>();
                }
                else
                {
                    btSpell = spellUi[i].AddComponent<Button>();
                    var navigation = btSpell.navigation;
                    navigation.mode = Navigation.Mode.None;
                    btSpell.navigation = navigation;
                }

                var skillIndex = i;
                btSpell.onClick.RemoveAllListeners();
                btSpell.onClick.AddListener(() =>
                {
                    SelectSkill(skillIndex);
                });
            }
            else
            {
                spellUi[i].GetComponent<Image>().sprite = null;
                spellUi[i].GetComponent<Image>().enabled = false;
            }
        }
    }

    /// <summary>
    /// Seleciona a skill pelo index
    /// </summary>
    /// <param name="index"></param>
    public void SelectSkill(int index)
    {
        if(controller.spells.Count<=index || controller.spells[index] == null || controller.spells[index].configSpell == null)
        {
            return;
        }

        if(controller.spells[index].cooldown > 0)
        {
            Manager.Instance.canvasManager.LogMessage($"<color=grey>Habilidade {controller.spells[index].configSpell.spellName} esta em cooldown</color>");
            return;
        }

        if (selectedSpell.Count > 0 && selectedSpell[0] != null && selectedSpell[0].configSpell != null)
        {
            Spell auxSpell = selectedSpell[0];
            selectedSpell = new List<Spell>();
            selectedUi.gameObject.SetActive(false);
            if (auxSpell == controller.spells[index])
            {
                return;
            }
        }

        if (controller.spells[index].configSpell != null && controller.spells[index].configSpell.castTarget != EnumCustom.CastTarget.None)
        {
            selectedSpell.Add(controller.spells[index]);
            selectedUi = spellUi[index].transform.GetChild(0);
            selectedUi.gameObject.SetActive(true);
        }
        else
        {
            if(!CheckMana(controller.spells[index].configSpell))
            {
                return;
            }
            controller.spells[index].Cast(()=> {  },controller,null, new Vector3Int(), minionCounts);
            controller.gameManager.EndMyTurn(controller);
        }
    }

    public bool CheckMana(SpellSO spell)
    {
        if (spell.costType == EnumCustom.CostType.Mana)
        {
            //Check mana
            if (spell.manaCost > controller.Mp)
            {
                Manager.Instance.canvasManager.LogMessage("<color=grey>Mana insuficiente</color>");
                return false;
            }
            else
            {
                controller.Mp -= spell.manaCost;
            }
        }
        else
        {
            var specialSpellWisp = controller.specialSpell.Find(n => n.effect == EnumCustom.SpecialEffect.Invoke_Wisp) as Invoke_Wisp;
            //Check mana
            if (specialSpellWisp == null || spell.manaCost > specialSpellWisp.value)
            {
                Manager.Instance.canvasManager.LogMessage("<color=grey>Wisps insuficiente</color>");
                return false;
            }
            else
            {
                specialSpellWisp.Cast(controller, spell.manaCost);
            }
        }
        return true;
    }

    /// <summary>
    /// Click no inimigo para atacar
    /// </summary>
    /// <param name="enemy">inimigo</param>
    /// <param name="clickPos">tile do inimigo</param>
    /// <param name="playerPos">posição do jogador</param>
    public void TryHit(EnemyController enemy, Vector3Int clickPos, Vector3Int playerPos)
    {
        //controller.specialSpell.Find(n => n.CheckType<Invisibility>())?.duration = 0;//pedro maybe

        controller.direction = Manager.Instance.gameManager.GetDirection(controller.CharacterMoveTileIsometric.controller.currentTileIndex, enemy.currentTileIndex);

        int offsetRange = 0;

        if (clickPos.x != playerPos.x && clickPos.y != playerPos.y)
            offsetRange = 1;

        if (selectedSpell.Count > 0)//Se tem uma spell selecionada ele tenta atacar com ela
        {
            foreach (var aux in selectedSpell)
            {
                if (aux.configSpell != null && aux.configSpell.name != "")
                {
                    if (aux.configSpell.castTarget == EnumCustom.CastTarget.Enemy && enemy == null)
                    {
                        return;
                    }

                    if (!aux.configSpell.melee)
                    {
                        CastSpell(enemy);
                        return;
                    }

                    if (aux.configSpell.melee && Vector3Int.Distance(clickPos, playerPos) <= 1 + offsetRange)//Detecta se o jogador esta a uma distancia suficiente
                    {
                        CastSpell(enemy);
                        return;
                    }
                    else
                    {
                        Manager.Instance.canvasManager.LogMessage("Inimigo fora do alcançe de ataque");
                    }
                    return;
                }
            }
        }

        if (Vector3Int.Distance(clickPos, playerPos) <= controller.CharacterInventory.Weapon.range + offsetRange)//Detecta se o jogador esta a uma distancia suficiente
        {
            HitEnemy(enemy);
        }
        else
        {
            Manager.Instance.canvasManager.LogMessage("Inimigo fora do alcançe de ataque");
        }
    }

    /// <summary>
    /// Casta a spell
    /// </summary>
    /// <param name="creature"></param>
    public void CastSpell(CreatureController creature = null, Vector3Int tile = new Vector3Int())
    {
        if (Manager.Instance.gameManager.DetectLOS(Manager.Instance.gameManager.GetPathForLOS(controller.currentTileIndex, tile != Vector3Int.zero ? tile : creature.currentTileIndex)))
        {
            Manager.Instance.canvasManager.LogMessage("Inimigo ou Tile fora do campo de visão");
            return;
        }
        foreach(var aux in selectedSpell)
        {
            if (aux.configSpell.castTarget == EnumCustom.CastTarget.Enemy && creature == null)
            {
                return;
            }

            if (selectedSpell.Count == 1)
            {
                if (!CheckMana(aux.configSpell))
                {
                    return;
                }
            }

        }

        //controller.specialSpell.Find(n => n.CheckType<Invisibility>()).duration = 0;//pedro maybe

        controller.animator.Play(controller.animationName + "Cast_" + controller.direction);

        Manager.Instance.gameManager.EndMyTurn(controller);
        foreach (var aux in selectedSpell)
        {
            aux.Cast(() =>
            {
                selectedSpell = new List<Spell>();
                if (selectedUi != null)
                {
                    selectedUi?.gameObject.SetActive(false);
                }
            }, controller, creature, tile, minionCounts,selectedSpell.Count>1?true:false);
        }
    }

    /// <summary>
    /// Ataca o inimigo fisicamente
    /// </summary>
    /// <param name="enemy"></param>
    public void HitEnemy(EnemyController enemy)
    {
        int hitChance = controller.attributeStatus.GetValue(EnumCustom.Status.HitChance);
        int dex = controller.attributeStatus.GetValue(EnumCustom.Attribute.Dex);

        controller.direction = Manager.Instance.gameManager.GetDirection(controller.CharacterMoveTileIsometric.controller.currentTileIndex, enemy.currentTileIndex);
        controller.animator.Play(controller.animationName + "Punch_" + controller.direction);

        Manager.Instance.gameManager.EndMyTurn(controller);

        if (!Combat.TryHit(hitChance, dex, enemy.attributeStatus.GetValue(EnumCustom.Status.Dodge), enemy.nickname))//Calcula se o hit errou
        {
            return;
        }

        //Define o dano do ataque
        int weaponDamage = controller.CharacterInventory.Weapon.getValue(); 
        int str = controller.attributeStatus.GetValue(EnumCustom.Attribute.Str);
        int critical = Combat.Critical(controller.attributeStatus.GetValue(EnumCustom.Status.CriticalHit));

        List<Skill> skillBuffs = skills.FindAll(n => n.skill.skillType == EnumCustom.SkillType.WeaponModifier);
        int skillModifier = 0;
        //Detecta os buffs
        if(skillBuffs.Count>0)
        {
            foreach(var aux in skillBuffs)
            {
                var tempweaponBuffSkills = aux.skill.weaponBuffSkills.FindAll(n => n.weaponType == controller.CharacterInventory.Weapon.weaponType);

                if(tempweaponBuffSkills.Count>0)
                {
                    foreach(var temp in tempweaponBuffSkills)
                    {
                        skillModifier += temp.value * aux.level;
                    }
                }
            }
        }

        int damage = (weaponDamage + str + skillModifier) * critical;

        string textDamage = "(" + weaponDamage + " + " + str + " + " + skillModifier + ") * " + critical;//texto do dano

        enemy.ReceiveHit(controller, damage, textDamage);
    }

    public void SetSpells(List<Spell> spells)
    {
        this.controller.spells = spells;
    }

    public void TryAssimilation(string name)
    {
        if(controller.specialSpell.Find(n => n.effect == EnumCustom.SpecialEffect.Assimilation) != null)
        {
            foreach(var aux in skills)
            {
                foreach(var temp in aux.spells)
                {
                    if(temp.configSpell.unlockWhenKillThis == name)
                    {
                        temp.locked = false;
                    }
                }
            }
        }
    }
}
