using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Arc/Item/Consumable")]
[System.Serializable]
public class ConsumableSO : ItemSO
{
    public int value;

    public EnumCustom.ConsumableType specialEffect;

    public void Consume()
    {
        switch(specialEffect)
        {
            case EnumCustom.ConsumableType.Hp:
                {
                    Manager.Instance.characterController.Hp += value;
                    break;
                }
        }
    }
}
