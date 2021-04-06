using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe das skills 
/// </summary>
[System.Serializable]
public class Skill
{
    public string name;

    public int level;

    public EnumCustom.SkillType skillType;

    public List<Status> modifier = new List<Status>();

    public List<WeaponBuffSkill> weaponBuffSkills = new List<WeaponBuffSkill>();

    public List<Spell> spells = new List<Spell>();
}

/// <summary>
/// Classe com o tipo de skill de buff
/// </summary>
[System.Serializable]
public class WeaponBuffSkill
{
    public EnumCustom.WeaponType weaponType;

    public int value;
}
