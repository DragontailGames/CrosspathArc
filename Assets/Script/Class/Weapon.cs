using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapon : Equipment
{
    public bool twoHanded = false;

    public int damageMin = 0;

    public int damageMax = 10;

    public bool mainHand = false;

    public bool offHand = false;

    public int range = 1;

    public EnumCustom.WeaponType weaponType;
}
