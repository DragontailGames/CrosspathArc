using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;

public class CharacterCombat : MonoBehaviour
{
    private CharacterController characterController;
    private CharacterStatus characterStatus;

    public List<Skill> skills;

    private List<Spell> spells = new List<Spell>(10);

    public List<GameObject> spellUi = new List<GameObject>();//Barra de spells

    public Spell selectedSpell = null;//Skill que o jogador escolheu

    public Transform selectedUi;

    public List<MinionCount> minionCounts = new List<MinionCount>();

    public int invisibilityDuration = 0;

    private Color32 color;

    public CharacterController CharacterController { get => this.characterController; set => this.characterController = value; }

    /// <summary>
    /// Configs
    /// </summary>
    public void Start()
    {
        CharacterController = this.GetComponent<CharacterController>();
        characterStatus = CharacterController.CharacterStatus;
        color = this.transform.GetChild(0).GetComponent<SpriteRenderer>().color;

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

            color.a = invisibilityDuration > 0 ? (byte)100 : (byte)255;
            this.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;
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
        if (spells[index].manaCost > CharacterController.CharacterStatus.Mp)
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
            spells[index].CastBuff(characterController);
            invisibilityDuration = 0;
            Manager.Instance.canvasManager.UpdateStatus();
            Manager.Instance.gameManager.EndMyTurn(characterController);
        }
        else if(spells[index].spellType == EnumCustom.SpellType.Special)
        {
            if (spells[index].specialEffect == EnumCustom.SpecialEffect.Invisibility)
            {
                Manager.Instance.canvasManager.UpdateStatus();
                characterStatus.Mp -= spells[index].manaCost;
                CastInvisibility(spells[index].specialEffectDuration);
            }
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
            StartCoroutine(characterController.StartDelay());
            Manager.Instance.canvasManager.LogMessage("Inimigo fora do campo de visão");
            return;
        }

        invisibilityDuration = 0;

        CharacterController.direction = Manager.Instance.gameManager.GetDirection(CharacterController.CharacterMoveTileIsometric.CurrentTileIndex, enemy.currentTileIndex);

        if (selectedSpell != null && selectedSpell.name != "")//Se tem uma spell selecionada ele tenta atacar com ela
        {
            if(selectedSpell.castTarget == EnumCustom.CastTarget.Target && enemy == null)
            {
                return;
            }
            CastSpell(enemy);
            return;
        }

        int offsetRange = 0;

        if(clickPos.x != playerPos.x && clickPos.y != playerPos.y)
            offsetRange = 1;

        if (Vector3Int.Distance(clickPos, playerPos) <= CharacterController.CharacterInventory.weapon.range + offsetRange)//Detecta se o jogador esta a uma distancia suficiente
        {
            HitEnemy(enemy);
        }
        else
        {
            Manager.Instance.canvasManager.LogMessage("Inimigo fora do alcançe de ataque");
            StartCoroutine(characterController.StartDelay());
        }
    }

    /// <summary>
    /// Casta a spell
    /// </summary>
    /// <param name="enemy"></param>
    public void CastSpell(EnemyController enemy = null, Vector3Int tile = new Vector3Int())
    {
        if (selectedSpell.castTarget == EnumCustom.CastTarget.Target && enemy == null)
            return;

        invisibilityDuration = 0;

        CharacterController.CharacterStatus.Mp -= selectedSpell.manaCost;

        CharacterController.Animator.Play(CharacterController.animationName + "_Cast_" + CharacterController.direction);

        int hitChance = characterStatus.attributeStatus.GetValue(EnumCustom.Status.SpellHit);
        int intAttribute = characterStatus.attributeStatus.GetValue(EnumCustom.Attribute.Int);

        int spellDamage = selectedSpell.GetValue();

        string extraDamage = "";
        int damage = spellDamage;

        if(selectedSpell.attributeInfluence.Count>0)
        {
            extraDamage = " + ";
        }

        foreach (var aux in selectedSpell.attributeInfluence)
        {
            var auxAttribute = characterStatus.attributeStatus.GetValue(aux);
            extraDamage += auxAttribute;
            damage += auxAttribute;
        }

        string textDamage = "(" + spellDamage + extraDamage + ")";

        if(selectedSpell.castTarget == EnumCustom.CastTarget.Area)
        {
            CastAreaSpell(hitChance, intAttribute, tile, damage, textDamage, selectedSpell);
        }
        else if (selectedSpell.castTarget == EnumCustom.CastTarget.Tile)
        {
            CastSpellInTile(tile, selectedSpell);
        }
        else if(selectedSpell.castTarget == EnumCustom.CastTarget.Target)
        {
            if (enemy != null)
            {
                CastProjectileSpell(hitChance, intAttribute, enemy, damage, textDamage, selectedSpell);
            }
            else
            {
                selectedSpell = null;
                selectedUi.gameObject.SetActive(false);
            }
        }
    }

    private void CastProjectileSpell(int hitChance, int intAttribute, EnemyController enemy, int damage, string textDamage, Spell spell)
    {
        bool hit = Combat.TryHit(hitChance, intAttribute, enemy.attributeStatus.GetValue(EnumCustom.Status.SpellDodge), enemy.enemy.name); 
        if (!hit)
        {
            selectedSpell = null;
            selectedUi.gameObject.SetActive(false);
            
            Manager.Instance.gameManager.EndMyTurn(characterController);
            return;
        }

        int poisonDamage = 0;

        UnityAction extraEffect = null;

        if(spell.specialEffect == EnumCustom.SpecialEffect.Poison)
            poisonDamage = Mathf.RoundToInt(skills.Find(n => n.name == "Necromancy").level/2);
        if(spell.specialEffect == EnumCustom.SpecialEffect.Hp_Regen)
        {
            extraEffect += () => { characterStatus.Hp += spell.minSpecialValue != 0 ? Random.Range(spell.minSpecialValue, spell.maxSpecialValue) : spell.fixedSpecialValue; };
        }
        if (spell.specialEffect == EnumCustom.SpecialEffect.Mp_Regen)
        {
            extraEffect += () => { characterStatus.Mp += spell.minSpecialValue != 0 ? Random.Range(spell.minSpecialValue, spell.maxSpecialValue) : spell.fixedSpecialValue; };
        }

        //Cria a spell e configura para a animação
        selectedSpell.AnimateCastProjectileSpell(enemy.transform.position, this.transform, () => {
            enemy.ReceiveSpell(damage, textDamage, spell, poisonDamage);
            extraEffect?.Invoke();
            selectedSpell = null;
            selectedUi.gameObject.SetActive(false);
            Manager.Instance.gameManager.EndMyTurn(characterController);
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
            EnemyController enemy = Manager.Instance.enemyManager.CheckEnemyInTile(aux);

            selectedSpell.AnimateCastAreaSpell(Manager.Instance.gameManager.tilemap.CellToLocal(aux));

            if(enemy!=null)
            {
                bool hit = Combat.TryHit(hitChance, intAttribute, enemy.attributeStatus.GetValue(EnumCustom.Status.SpellDodge), enemy.enemy.name);
                if (hit)
                {
                    enemy.ReceiveSpell(damage, textDamage, spell);
                }
            }
        }

        selectedSpell = null;
        selectedUi.gameObject.SetActive(false);

        Manager.Instance.gameManager.EndMyTurn(characterController);
    }

    public void CastSpellInTile(Vector3Int index, Spell spell)
    {
        if (spell.specialEffect == EnumCustom.SpecialEffect.Invoke)
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
        else if (spell.specialEffect == EnumCustom.SpecialEffect.Blink)
        {
            var points = Manager.Instance.gameManager.GetPath(characterController.CharacterMoveTileIsometric.CurrentTileIndex, index);
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
                characterController.CharacterMoveTileIsometric.Blink(pos);
            }
        }

        selectedSpell = null;
        selectedUi.gameObject.SetActive(false);

        Manager.Instance.gameManager.EndMyTurn(characterController);
    }

    /// <summary>
    /// Ataca o inimigo fisicamente
    /// </summary>
    /// <param name="enemy"></param>
    public void HitEnemy(EnemyController enemy)
    {
        int hitChance = characterStatus.attributeStatus.GetValue(EnumCustom.Status.HitChance);
        int dex = characterStatus.attributeStatus.GetValue(EnumCustom.Attribute.Dex);

        CharacterController.direction = Manager.Instance.gameManager.GetDirection(CharacterController.CharacterMoveTileIsometric.CurrentTileIndex, enemy.currentTileIndex);
        CharacterController.Animator.Play(CharacterController.animationName + "_Punch_" + CharacterController.direction);

        Manager.Instance.gameManager.EndMyTurn(characterController);

        if (!Combat.TryHit(hitChance, dex, enemy.attributeStatus.GetValue(EnumCustom.Status.Dodge), enemy.enemy.name))//Calcula se o hit errou
        {
            return;
        }

        //Define o dano do ataque
        int weaponDamage = Random.Range(CharacterController.CharacterInventory.weapon.damageMin, CharacterController.CharacterInventory.weapon.damageMax+1);
        int str = characterStatus.attributeStatus.GetValue(EnumCustom.Attribute.Str);
        int critical = Combat.Critical(characterStatus.attributeStatus.GetValue(EnumCustom.Status.CriticalHit));

        List<Skill> skillBuffs = skills.FindAll(n => n.skill.skillType == EnumCustom.SkillType.WeaponModifier);
        int skillModifier = 0;
        //Detecta os buffs
        if(skillBuffs.Count>0)
        {
            foreach(var aux in skillBuffs)
            {
                var tempweaponBuffSkills = aux.skill.weaponBuffSkills.FindAll(n => n.weaponType == CharacterController.CharacterInventory.weapon.weaponType);

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

        enemy.ReceiveHit(damage, textDamage);
    }

    public void GetHit(int damage, BotController enemy)
    {
        CharacterController.direction = Manager.Instance.gameManager.GetDirection(CharacterController.CharacterMoveTileIsometric.CurrentTileIndex, enemy.currentTileIndex);
        CharacterController.Animator.Play(CharacterController.animationName + "_GetHit_" + CharacterController.direction);

        int armor = characterStatus.attributeStatus.GetValue(EnumCustom.Status.Armor);
        int trueDamage = Mathf.Clamp(damage - armor, 0, damage);

        Manager.Instance.canvasManager.LogMessage(characterController.CharacterStatus.nickname + " sofreu " + damage + " - " + armor + " = <color=red>" + trueDamage + "</color> de dano");

        if (!CharacterController.CharacterStatus.DropHP(trueDamage))
        {
            CharacterController.direction = Manager.Instance.gameManager.GetDirection(CharacterController.CharacterMoveTileIsometric.CurrentTileIndex, enemy.currentTileIndex);
            CharacterController.Animator.Play(CharacterController.animationName + "_Die_" + CharacterController.direction);
            Manager.Instance.gameManager.InPause = true;
            Manager.Instance.gameManager.creatures.Remove(this.gameObject);
            Debug.Log("Morreu");
        }
    }

    public void CastInvisibility(int duration)
    {
        invisibilityDuration = duration;
        foreach(var aux in Manager.Instance.enemyManager.enemies)
        {
            if(aux.target == this.gameObject)
            {
                aux.hasTarget = false;
                aux.target = null;
            }
        }
    }

    public void SetSpells(List<Spell> spells)
    {
        this.spells = spells;
    }
}
