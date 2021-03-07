using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class : MonoBehaviour
{

}

[System.Serializable]
public class EquipmentUi
{
    public string name;
    public Transform slot;
    public EnumCustom.EquipmentType equipmentType;
}
