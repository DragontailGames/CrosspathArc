using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Arc/Item/Equipment")]
[System.Serializable]
public class EquipmentSO : ItemSO
{
    public EnumCustom.EquipmentType equipmentType;

    public List<Status> statuses;

    public List<Attributes> attributes;

    public int hp;

    public int mp;
}
