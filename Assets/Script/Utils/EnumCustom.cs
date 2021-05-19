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
        Unlock,
        Hit_Special,
        Hit_Buff,
        Hit_Debuff,
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
        Invoke,
        Hp_Regen,
        Mp_Regen,
        Fake_Life,
        Blink,
    }

    public enum BuffDebuffType
    {
        Status,
        Attribute,
        Special,
    }

    public enum CastTarget
    {
        Target,
        Area,
        Tile,
        None,
    }
}
