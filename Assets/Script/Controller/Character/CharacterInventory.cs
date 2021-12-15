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

    public List<ItemInventory> equipements;

    public StatusManager statusManager;

    public void UpdateEquipmentList()
    {
        int extraHp = 0;
        int extraMp = 0;
        foreach (var aux in equipements)
        {
            if (aux.equiped)
            {
                extraHp += (aux.item as EquipmentSO).hp;
                extraMp += (aux.item as EquipmentSO).mp;
            }
        }
        controller.attributeStatus.hpExtra = extraHp;
        controller.attributeStatus.mpExtra = extraMp;

        statusManager.UpdateStatus();
    }
}
