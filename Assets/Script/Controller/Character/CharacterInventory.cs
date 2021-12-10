using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inventario simples para o combate do jogador
/// </summary>
public class CharacterInventory : MonoBehaviour
{
    public CharacterController controller;

    public Weapon weapon;

    public Weapon secondaryWeapon;

    public InventoryManager inventoryManager;

    public void AddToInventory(ItemSO item, int qtd)
    {
        try
        {
            inventoryManager.inventory.Find(n => n.item.itemName == item.itemName).qtd += qtd;
        }
        catch
        {
            inventoryManager.inventory.Add(new ItemInterface() { item = item, qtd = qtd, slot = inventoryManager.GetNextSlot() });
        }
    }
}
