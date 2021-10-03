using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumCustom : MonoBehaviour
{
    public enum EquipmentType 
    {
        Helm,
        Neck,
        Chest,
        Belt,
        Pants,
        Gloves,
        Ring,
        Boots,
        Weapon,
        Shield,
        Bow
    }

    public enum Attribute
    {
        Str,
        Con,
        Dex,
        Agi,
        Int,
        Wis,
        Foc,
        Cha
    }

    public enum Status
    {
        HitChance,
        Dodge,
        Armor,
        CriticalHit,
        SpellHit,
        SpellDodge,
        HpRegen,
        MpRegen
    }

    public enum SkillType
    {
        WeaponModifier,
        Spellbook,
    }

    public enum SpellType
    {
        Hit,
        Buff,
        Debuff,
        Special,
        Invoke,
        Blink,
        Unlock,
        Create_Food,
        Area_Hazard,
        Cure_Disease,
    }

    public enum WeaponType
    {
        Hands,
        Sword,
        Shield
    }

    public enum SpecialEffect
    {
        None,
        Poison,
        Sleep,
        Paralyze,
        Invisibility,
        Hp_Regen,
        Mp_Regen,
        Fake_Life,
        Spike,
        Cannot_Walk,
        All,
        Invoke_Wisp,
        Visibility,
    }

    public enum BuffDebuffType
    {
        Status,
        Attribute,
        Special,
    }

    public enum CastTarget
    {
        Enemy,
        Area,
        Tile,
        None,
        Caster,
        Target,
        Minions,
    }

    public enum FormulaType
    {
        None,
        SkillLevel,
    }

    public enum CostType
    {
        Mana,
        Wisp,
    }
}
