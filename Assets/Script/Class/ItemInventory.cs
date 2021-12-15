using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemInventory
{
    public Vector2 slot;

    public int qtd;

    public ItemSO item;

    public bool equiped;

    public ItemInventory(Vector2 slot, int qtd, ItemSO item,bool equiped)
    {
        this.slot = slot;
        this.qtd = qtd;
        this.item = item;
        this.equiped = equiped;
    }
}
