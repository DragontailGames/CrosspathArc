using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDrop
{
    public ItemSO item;

    [Range(0,100)]
    public float probability;
}
