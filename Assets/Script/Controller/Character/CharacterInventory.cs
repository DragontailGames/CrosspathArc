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
        List<Status> statusAux = new List<Status>();
        List<Attributes> attributesAux = new List<Attributes>();

        SupportStatus supportStatus;

        foreach (var aux in equipements)
        {
            if (aux.equiped)
            {
                EquipmentSO equip = aux.item as EquipmentSO;
                extraHp += equip.hp;
                extraMp += equip.mp;
                foreach(var statusTemp in equip.statuses)
                {
                    statusAux.Add(statusTemp);
                }
                foreach (var attributeTemp in equip.attributes)
                {
                    attributesAux.Add(attributeTemp);
                }

                if(equip.GetType() == typeof(ChestEquipmentSO))
                {
                    ChestEquipmentSO chestEquipment = (equip as ChestEquipmentSO); 
                    foreach(var tempSupportSkill in chestEquipment.skill.skill.support)
                    {
                        supportStatus = tempSupportSkill;
                    }
                    //supportStatuses.Add(chestEquipment.supportStatus);
                }
            }
        }
        controller.attributeStatus.hpExtra = extraHp;
        controller.attributeStatus.mpExtra = extraMp;
        controller.attributeStatus.statusEquipmentModifier = statusAux;
        controller.attributeStatus.attributeEquipmentModifier = attributesAux;
        foreach (var tempSpell in controller.CharacterCombat.supportSkills)
        {
            foreach (var suportAux in tempSpell.skill.support)
            {
                suportAux.StartRound(controller, tempSpell);
            }
        }

        statusManager.UpdateStatus();
    }
}