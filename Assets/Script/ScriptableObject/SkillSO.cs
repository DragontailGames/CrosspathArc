using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Arc/Skill")]
public class SkillSO : ScriptableObject
{
    public string skillName;

    public EnumCustom.SkillType skillType;

    public List<Status> modifier = new List<Status>();

    public List<WeaponBuffSkill> weaponBuffSkills = new List<WeaponBuffSkill>();

    public List<Spell> spells = new List<Spell>();
}
