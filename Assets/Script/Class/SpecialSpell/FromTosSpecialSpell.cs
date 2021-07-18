using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FromTosSpecialSpell : MonoBehaviour
{
    public void ConvertSpecialSpell(SpecialSpell specialSpell)
    {
        switch (specialSpell.effect)
        {
            case EnumCustom.SpecialEffect.Poison:
            {
                specialSpell = new Poison(specialSpell.duration, specialSpell.value, specialSpell.controller, specialSpell.effect);
                break;
            }
        }
    }
}
