using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Add_Item_Inventory : SpecialSpell
{
    public Add_Item_Inventory(SpecialSpell specialSpell) : base(specialSpell.duration, specialSpell.value, specialSpell.caster, specialSpell.target, specialSpell.tile, specialSpell.effect, specialSpell.logName, null, specialSpell.itemSO)
    {
        Manager.Instance.inventoryManager.AddToInventory(specialSpell.itemSO, specialSpell.value);
    }
}
