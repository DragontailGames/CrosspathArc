using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemSO : ScriptableObject
{
    public string itemName;

    [Range(1, 99)]
    public int stackable = 1;

    public Sprite icon;

    [TextArea]
    public string description;
}
