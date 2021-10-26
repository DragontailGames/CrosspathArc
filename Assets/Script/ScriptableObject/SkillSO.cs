using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Arc/Skill")]
public class SkillSO : ScriptableObject
{
    public string skillName;

    public EnumCustom.SkillType skillType;

    public List<SupportStatus> support = new List<SupportStatus>();

    public List<WeaponBuffSkill> weaponBuffSkills = new List<WeaponBuffSkill>();

    public List<SpellSO> spells = new List<SpellSO>();
}
