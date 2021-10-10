using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ParserCustom
{
    public static void SpellSpecialParser(SpecialSpell specialSpell)
    {
        switch(specialSpell.effect)
        {
            case EnumCustom.SpecialEffect.Cannot_Walk:
                {
                    Debug.LogError("Cannot Walk not implemented");
                    break;
                }
            case EnumCustom.SpecialEffect.Fake_Life:
                {
                    new Fake_Life(specialSpell);
                    break;
                }
            case EnumCustom.SpecialEffect.Hp_Regen:
                {
                    new Hp_Regen(specialSpell);
                    break;
                }
            case EnumCustom.SpecialEffect.Invisibility:
                {
                    new Invisibility(specialSpell);
                    break;
                }
            case EnumCustom.SpecialEffect.Mp_Regen:
                {
                    new Mp_Regen(specialSpell);
                    break;
                }
            case EnumCustom.SpecialEffect.Paralyze:
                {
                    new Paralyze(specialSpell);
                    break;
                }
            case EnumCustom.SpecialEffect.Poison:
                {
                    new Poison(specialSpell);
                    break;
                }
            case EnumCustom.SpecialEffect.Sleep:
                {
                    new Sleep(specialSpell);
                    break;
                }
            case EnumCustom.SpecialEffect.Spike:
                {
                    new Spike(specialSpell);
                    break;
                }
            case EnumCustom.SpecialEffect.Invoke_Wisp:
                {
                    new Invoke_Wisp(specialSpell);
                    break;
                }
            case EnumCustom.SpecialEffect.Visibility:
                {
                    new Visibility(specialSpell);
                    break;
                }
            case EnumCustom.SpecialEffect.Aggro:
                {
                    new Aggro(specialSpell);
                    break;
                }
            case EnumCustom.SpecialEffect.Charm:
                {
                    new Charm(specialSpell);
                    break;
                }
        }
    }
}
