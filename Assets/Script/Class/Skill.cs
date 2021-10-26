using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill
{
    public string name;

    public string description;

    public SkillSO skill;

    public List<Spell> spells;

    public int level;
}
