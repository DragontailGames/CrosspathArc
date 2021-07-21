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

    private List<Spell> spells = new List<Spell>(10);

    public List<GameObject> spellUi = new List<GameObject>();//Barra de spells

    public Spell selectedSpell = null;//Skill que o jogador escolheu

    public Transform selectedUi;

    public List<MinionCount> minionCounts = new List<MinionCount>();

    /// <summary>
    /// Configs
    /// </summary>
    public void Start()
    {
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
            if (spells.Count> i && spells[i] != null)
            {
                spellUi[i].GetComponent<Image>().sprite = spells[i].icon;
                spellUi[i].GetComponent<Image>().enabled = true;
                Button btSpell;
                if (spellUi[i].GetComponent<Button>())
                {
                    btSpell = spellUi[i].GetComponent<Button>();
                }
                else
                {
                    btSpell = spellUi[i].AddComponent<Button>();
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
        if(spells.Count<=index || spells[index] == null)
        {
            return;
        }

        if (selectedSpell!=null)
        {
            var auxSpell = selectedSpell;
            selectedSpell = null;
            selectedUi.gameObject.SetActive(false);
            if (auxSpell == spells[index])
            {
                return;
            }
        }

        //Check mana
        if (spells[index].manaCost > controller.Mp)
        {
            Manager.Instance.canvasManager.LogMessage("<color=grey>Mana insuficiente</color>");
            return;
        }

        if (spells[index].castTarget != EnumCustom.CastTarget.None)
        {
            selectedSpell = spells[index];
            selectedUi = spellUi[index].transform.GetChild(0);
            selectedUi.gameObject.SetActive(true);
        }
        else if(spells[index].spellType == EnumCustom.SpellType.Buff)
        {
            spells[index].CastBuff(controller);
            foreach (var aux in controller.specialSpell)
            {
                aux.HandleAttack(controller);
            }
            Manager.Instance.canvasManager.UpdateStatus();
            Manager.Instance.gameManager.EndMyTurn(controller);
        }
        else if(spells[index].spellType == EnumCustom.SpellType.Special)
        {
            controller.Mp -= spells[index].manaCost;
            spells[index].CastSpecial(controller, controller);
            Manager.Instance.canvasManager.UpdateStatus();
        }

        Manager.Instance.canvasManager.UpdateStatus();
    }

    /// <summary>
    /// Click no inimigo para atacar
    /// </summary>
    /// <param name="enemy">inimigo</param>
    /// <param name="clickPos">tile do inimigo</param>
    /// <param name="playerPos">posição do jogador</param>
    public void TryHit(EnemyController enemy, Vector3Int clickPos, Vector3Int playerPos)
    {
        if (enemy.hasTarget==false)
        {
            StartCoroutine(controller.StartDelay());
            Manager.Instance.canvasManager.LogMessage("Inimigo fora do campo de visão");
            return;
        }

        //controller.specialSpell.Find(n => n.CheckType<Invisibility>())?.duration = 0;//pedro maybe

        controller.direction = Manager.Instance.gameManager.GetDirection(controller.CharacterMoveTileIsometric.controller.currentTileIndex, enemy.currentTileIndex);

        if (selectedSpell != null && selectedSpell.name != "")//Se tem uma spell selecionada ele tenta atacar com ela
        {
            if(selectedSpell.castTarget == EnumCustom.CastTarget.Enemy && enemy == null)
            {
                return;
            }
            CastSpell(enemy);
            return;
        }

        int offsetRange = 0;

        if(clickPos.x != playerPos.x && clickPos.y != playerPos.y)
            offsetRange = 1;

        if (Vector3Int.Distance(clickPos, playerPos) <= controller.CharacterInventory.weapon.range + offsetRange)//Detecta se o jogador esta a uma distancia suficiente
        {
            HitEnemy(enemy);
        }
        else
        {
            Manager.Instance.canvasManager.LogMessage("Inimigo fora do alcançe de ataque");
            StartCoroutine(controller.StartDelay());
        }
    }

    /// <summary>
    /// Casta a spell
    /// </summary>
    /// <param name="creature"></param>
    public void CastSpell(CreatureController creature = null, Vector3Int tile = new Vector3Int())
    {
        if (selectedSpell.castTarget == EnumCustom.CastTarget.Enemy && creature == null)
            return;

        //controller.specialSpell.Find(n => n.CheckType<Invisibility>()).duration = 0;//pedro maybe

        controller.Mp -= selectedSpell.manaCost;

        controller.animator.Play(controller.animationName + "_Cast_" + controller.direction);

        int hitChance = controller.attributeStatus.GetValue(EnumCustom.Status.SpellHit);
        int intAttribute = controller.attributeStatus.GetValue(EnumCustom.Attribute.Int);

        int spellDamage = selectedSpell.GetValue(controller);

        string extraDamage = "";
        int damage = spellDamage;

        if(selectedSpell.attributeInfluence.Count>0)
        {
            extraDamage = " + ";
        }

        foreach (var aux in selectedSpell.attributeInfluence)
        {
            var auxAttribute = aux.GetValue(controller);
            extraDamage += auxAttribute;
            damage += auxAttribute;
        }

        string textDamage = "(" + spellDamage + extraDamage + ")";

        if(selectedSpell.castTarget == EnumCustom.CastTarget.Area)
        {
            if(tile == new Vector3Int() && creature != null)
            {
                tile = creature.currentTileIndex;
            }
            CastAreaSpell(hitChance, intAttribute, tile, damage, textDamage, selectedSpell);
        }
        else if (selectedSpell.castTarget == EnumCustom.CastTarget.Tile)
        {
            CastSpellInTile(tile, selectedSpell);
        }
        else if(selectedSpell.castTarget == EnumCustom.CastTarget.Enemy)
        {
            if (creature != null && creature.GetType() == typeof(EnemyController))
            {
                CastProjectileSpell(hitChance, intAttribute, creature as EnemyController, damage, textDamage, selectedSpell);
            }
            else
            {
                selectedSpell = null;
                selectedUi.gameObject.SetActive(false);
            }
        }
        else if (selectedSpell.castTarget == EnumCustom.CastTarget.Target)
        {
            CastProjectileSpell(100, intAttribute, creature, damage, textDamage, selectedSpell);
        }
    }

    private void CastProjectileSpell(int hitChance, int intAttribute, CreatureController creature, int damage, string textDamage, Spell spell)
    {
        bool hit = Combat.TryHit(hitChance, intAttribute, creature.attributeStatus.GetValue(EnumCustom.Status.SpellDodge), creature.nickname); 
        if (!hit)
        {
            selectedSpell = null;
            selectedUi.gameObject.SetActive(false);
            
            Manager.Instance.gameManager.EndMyTurn(controller);
            return;
        }

        UnityAction extraEffect = null;

        //Cria a spell e configura para a animação
        selectedSpell.AnimateCastProjectileSpell(creature.transform.position, this.transform, () => {
            creature.ReceiveSpell(controller, damage, textDamage, spell);
            spell.Cast(controller, creature);
            extraEffect?.Invoke();
            selectedSpell = null;
            selectedUi.gameObject.SetActive(false);
            Manager.Instance.gameManager.EndMyTurn(controller);
        });
    }

    private void CastAreaSpell(int hitChance, int intAttribute, Vector3Int startIndex, int damage, string textDamage, Spell spell)
    {
        List<Vector3Int> tiles = new List<Vector3Int>();
        int area = selectedSpell.area;

        int endX = Mathf.FloorToInt(area / 2) - ((area % 2 == 0) ? 1 : 0);
        int endY = Mathf.FloorToInt(area / 2) - ((area % 2 == 0) ? 1 : 0);

        for (int x = startIndex.x - Mathf.FloorToInt(area/2); x<= startIndex.x + endX; x++)
        {
            for(int y = startIndex.y - Mathf.FloorToInt(area / 2); y <= startIndex.y + endY; y++)
            {
                Vector3Int t = new Vector3Int(x, y, 0);
                if (x >= 0 && y>=0 && Manager.Instance.gameManager.tilemap.HasTile(t))
                {
                    tiles.Add(t);
                }
            }
        }

        foreach(var aux in tiles)
        {
            spell.AnimateCastAreaSpell(Manager.Instance.gameManager.tilemap.CellToLocal(aux), aux);
            if (spell.spellType != EnumCustom.SpellType.Area_Hazard)
            {
                EnemyController enemy = Manager.Instance.enemyManager.CheckEnemyInTile(aux);

                if (enemy != null)
                {
                    bool hit = Combat.TryHit(hitChance, intAttribute, enemy.attributeStatus.GetValue(EnumCustom.Status.SpellDodge), enemy.nickname);
                    if (hit)
                    {
                        enemy.ReceiveSpell(controller, damage, textDamage, spell);
                    }
                }
            }
            
        }

        selectedSpell = null;
        selectedUi.gameObject.SetActive(false);

        Manager.Instance.gameManager.EndMyTurn(controller);
    }

    public void CastSpellInTile(Vector3Int index, Spell spell)
    {
        if (spell.spellType == EnumCustom.SpellType.Invoke)
        {
            GameObject creature = spell.InvokeCreature(Manager.Instance.gameManager.tilemap.CellToLocal(index));
            if (minionCounts.Find(n => n.spell == spell) != null)
            {
                if (minionCounts.Find(n => n.spell == spell).creatures.Count >= spell.invokeLimit)
                {
                    List<GameObject> orderedCreature = minionCounts.Find(n => n.spell == spell).creatures.OrderBy(n => n.GetComponent<MinionController>().duration).ToList();
                    orderedCreature[0].GetComponent<MinionController>().Defeat();
                }
                minionCounts.Find(n => n.spell == spell).creatures.Add(creature);
            }
            else
            {
                minionCounts.Add(new MinionCount()
                {
                    spell = spell,
                    creatures = new List<GameObject>()
                {
                    creature
                }
                });
            }
        }
        else if (spell.spellType == EnumCustom.SpellType.Blink)
        {
            var points = Manager.Instance.gameManager.GetPath(controller.CharacterMoveTileIsometric.controller.currentTileIndex, index);
            PathFind.Point point = null;
            for(int i = 0;i < (points.Count > 5 ? 5 : points.Count - 1);i++ )
            {
                if(!Manager.Instance.gameManager.elevationTM.HasTile(new Vector3Int(points[i].x + 1, points[i].y + 1, 0)))
                {
                    point = points[i];
                }
                else
                {
                    break;
                }
            }
            if (point != null)
            {
                Vector3Int pos = new Vector3Int(point.x, point.y, 0);
                controller.CharacterMoveTileIsometric.Blink(pos);
            }
        }

        selectedSpell = null;
        selectedUi.gameObject.SetActive(false);

        Manager.Instance.gameManager.EndMyTurn(controller);
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
        controller.animator.Play(controller.animationName + "_Punch_" + controller.direction);

        Manager.Instance.gameManager.EndMyTurn(controller);

        if (!Combat.TryHit(hitChance, dex, enemy.attributeStatus.GetValue(EnumCustom.Status.Dodge), enemy.nickname))//Calcula se o hit errou
        {
            return;
        }

        //Define o dano do ataque
        int weaponDamage = Random.Range(controller.CharacterInventory.weapon.damageMin, controller.CharacterInventory.weapon.damageMax+1);
        int str = controller.attributeStatus.GetValue(EnumCustom.Attribute.Str);
        int critical = Combat.Critical(controller.attributeStatus.GetValue(EnumCustom.Status.CriticalHit));

        List<Skill> skillBuffs = skills.FindAll(n => n.skill.skillType == EnumCustom.SkillType.WeaponModifier);
        int skillModifier = 0;
        //Detecta os buffs
        if(skillBuffs.Count>0)
        {
            foreach(var aux in skillBuffs)
            {
                var tempweaponBuffSkills = aux.skill.weaponBuffSkills.FindAll(n => n.weaponType == controller.CharacterInventory.weapon.weaponType);

                if(tempweaponBuffSkills.Count>0)
                {
                    foreach(var temp in tempweaponBuffSkills)
                    {
                        skillModifier += temp.value;
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
        this.spells = spells;
    }
}
