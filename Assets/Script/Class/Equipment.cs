using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Equipamentos em geral
/// </summary>
[System.Serializable]
public class Equipment
{
    public EnumCustom.EquipmentType equipmentType;

    public string itemName;

    public Sprite icon;

    public List<Status> statuses;
}
