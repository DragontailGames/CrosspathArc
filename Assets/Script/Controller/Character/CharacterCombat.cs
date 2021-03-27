using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CharacterCombat : MonoBehaviour
{
    private CharacterController characterController;
    private CharacterStatus characterStatus;

    public List<Skill> skills;

    public List<Spell> spells;

    public List<GameObject> spellUi = new List<GameObject>();//Barra de spells

    public Spell selectedSpell = null;//Skill que o jogador escolheu

    public Transform selectedUi;

    public CharacterController CharacterController { get => this.characterController; set => this.characterController = value; }

    /// <summary>
    /// Configs
    /// </summary>
    public void Start()
    {
        CharacterController = this.GetComponent<CharacterController>();
        characterStatus = CharacterController.CharacterStatus;

        foreach (Transform aux in Manager.Instance.canvasManager.skillPanel.GetChild(0))
        {
            spellUi.Add(aux.gameObject);
        }
        foreach (Transform aux in Manager.Instance.canvasManager.skillPanel.GetChild(1))
        {
            spellUi.Add(aux.gameObject);
        }

        for(int i = 0;i< spells.Count;i++)
        {
            spellUi[i].GetComponent<Image>().sprite = spells[i].icon;
            spellUi[i].GetComponent<Image>().enabled = true;
        }
    }

    public void Update()
    {
        if (selectedSpell == null || string.IsNullOrEmpty(selectedSpell.name))//Identifica se não existe outra skill selecionada
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSkill(0);//Ativa a skill no primeiro slot
        }
    }

    /// <summary>
    /// Seleciona a skill pelo index
    /// </summary>
    /// <param name="index"></param>
    public void SelectSkill(int index)
    {
        //Checa mana
        if (spells[index].manaCost > CharacterController.CharacterStatus.Mp)
        {
            Manager.Instance.canvasManager.LogMessage("<color=grey>Mana insuficiente</color>");
            return;
        }

        selectedSpell = spells[index];
        selectedUi = spellUi[index].transform.GetChild(0);
        selectedUi.gameObject.SetActive(true);
    }

    /// <summary>
    /// Click no inimigo para atacar
    /// </summary>
    /// <param name="enemy">inimigo</param>
    /// <param name="clickPos">tile do inimigo</param>
    /// <param name="playerPos">posição do jogador</param>
    public void TryHit(EnemyController enemy, Vector3Int clickPos, Vector3Int playerPos)
    {
        if(selectedSpell != null && selectedSpell.name != "")//Se tem uma spell selecionada ele tenta atacar com ela
        {
            CastSpell(enemy);
            return;
        }

        int offsetRange = 0;

        if(clickPos.x != playerPos.x && clickPos.y != playerPos.y)
            offsetRange = 1;

        if (Vector3Int.Distance(clickPos, playerPos)<= CharacterController.CharacterInventory.weapon.range + offsetRange)//Detecta se o jogador esta a uma distancia suficiente
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
    /// <param name="enemy"></param>
    public void CastSpell(EnemyController enemy)
    {
        CharacterController.CharacterStatus.Mp -= selectedSpell.manaCost;

        int hitChance = characterStatus.attributeStatus.GetValue(EnumCustom.Status.SpellHit);
        int intAttribute = characterStatus.attributeStatus.GetValue(EnumCustom.Attribute.Int);

        if (!Combat.TryHit(hitChance, intAttribute, enemy.enemy.attributeStatus.GetValue(EnumCustom.Status.SpellDodge)))//Calcula se o hit errou
        {
            selectedSpell = null;
            selectedUi.gameObject.SetActive(false);
            return;
        }

        int weaponDamage = Random.Range(selectedSpell.minDamage, selectedSpell.maxDamage+1);

        int damage = weaponDamage + intAttribute;
        string textDamage = "(" + weaponDamage + " + " + intAttribute + ")";

        CharacterController.direction = Manager.Instance.gameManager.GetDirection(CharacterController.CharacterMoveTileIsometric.CurrentTileIndex, enemy.enemy.tilePos);
        CharacterController.Animator.Play(CharacterController.animationName + "_Cast_" + CharacterController.direction);

        //Cria a spell e configura para a animação
        StartCoroutine(AnimateCastSpell(enemy.transform.position, selectedSpell, () => { 
            enemy.HitEnemy(damage, textDamage); 
            selectedSpell = null; 
            selectedUi.gameObject.SetActive(false); 
        }));
    }

    /// <summary>
    /// Animação da spell
    /// </summary>
    /// <param name="targetPos">posição do inimigo</param>
    /// <param name="spell"></param>
    /// <param name="hitAction">Ação apos o hit</param>
    /// <returns></returns>
    public IEnumerator AnimateCastSpell(Vector3 targetPos, Spell spell, UnityAction hitAction)
    {
        //Corrige a rotação da spell
        Vector3 diff = targetPos - transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

        Transform spellCreated = Instantiate(spell.spellObject, this.transform.position + Vector3.up * 0.5f, Quaternion.Euler(0f, 0f, rot_z - 180)).transform;
        yield return new WaitForSeconds(0.05f);

        //Detecta a distancia
        while (Vector3.Distance(spellCreated.position, targetPos) > 0.1f)
        {
            if (Vector3.Distance(spellCreated.position, targetPos) <= 0.3f)
            {
                //Destroi depois de acertar
                Destroy(spellCreated.gameObject);
                hitAction?.Invoke();
                break;
            }
            //Move a spell
            float step = 4f * Time.deltaTime;
            spellCreated.position = Vector3.MoveTowards(spellCreated.position, targetPos, step);
            yield return new WaitForSeconds(0.001f);
        }
    }

    /// <summary>
    /// Ataca o inimigo fisicamente
    /// </summary>
    /// <param name="enemy"></param>
    public void HitEnemy(EnemyController enemy)
    {
        int hitChance = characterStatus.attributeStatus.GetValue(EnumCustom.Status.HitChance);
        int dex = characterStatus.attributeStatus.GetValue(EnumCustom.Attribute.Dex);

        if (!Combat.TryHit(hitChance, dex, enemy.enemy.attributeStatus.GetValue(EnumCustom.Status.Dodge)))//Calcula se o hit errou
        {
            return;
        }

        //Define o dano do ataque
        int weaponDamage = Random.Range(CharacterController.CharacterInventory.weapon.damageMin, CharacterController.CharacterInventory.weapon.damageMax+1);
        int str = characterStatus.attributeStatus.GetValue(EnumCustom.Attribute.Str);
        int critical = Combat.Critical(characterStatus.attributeStatus.GetValue(EnumCustom.Status.CriticalHit));

        List<Skill> skillBuffs = skills.FindAll(n => n.skillType == EnumCustom.SkillType.WeaponModifier);
        int skillModifier = 0;
        //Detecta os buffs
        if(skillBuffs.Count>0)
        {
            foreach(var aux in skillBuffs)
            {
                var tempweaponBuffSkills = aux.weaponBuffSkills.FindAll(n => n.weaponType == CharacterController.CharacterInventory.weapon.weaponType);

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

        CharacterController.direction = Manager.Instance.gameManager.GetDirection(CharacterController.CharacterMoveTileIsometric.CurrentTileIndex, enemy.enemy.tilePos);
        CharacterController.Animator.Play(CharacterController.animationName + "_Punch_" + CharacterController.direction);

        enemy.HitEnemy(damage, textDamage);
    }

    public void GetHit(int damage, EnemyController enemy)
    {
        CharacterController.direction = Manager.Instance.gameManager.GetDirection(CharacterController.CharacterMoveTileIsometric.CurrentTileIndex, enemy.currentTileIndex);
        CharacterController.Animator.Play(CharacterController.animationName + "_GetHit_" + CharacterController.direction);

        int armor = characterStatus.attributeStatus.GetValue(EnumCustom.Status.Armor);
        int trueDamage = Mathf.Clamp(damage - armor, 0, damage);

        Manager.Instance.canvasManager.LogMessage(characterController.CharacterStatus.nickname + " sofreu " + damage + " - " + armor + " = <color=red>" + trueDamage + "</color> de dano");

        if (!CharacterController.CharacterStatus.DropHP(trueDamage))
        {
            CharacterController.direction = Manager.Instance.gameManager.GetDirection(CharacterController.CharacterMoveTileIsometric.CurrentTileIndex, enemy.enemy.tilePos);
            CharacterController.Animator.Play(CharacterController.animationName + "_Die_" + CharacterController.direction);
            Manager.Instance.gameManager.InPause = true;
            Debug.Log("Morreu");
        }
    }
}
