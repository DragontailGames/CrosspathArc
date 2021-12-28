using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inventario simples para o combate do jogador
/// </summary>
public class CharacterInventory : MonoBehaviour
{
    public CharacterController controller;

    public WeaponEquipmentSO weapon;

    public WeaponEquipmentSO extraWeapon;

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
        foreach (var tempSpell in controller.CharacterCombat.skills)
        {
            foreach (var suportAux in tempSpell.skill.support)
            {
                suportAux.StartRound(controller, tempSpell);
            }
        }

        var weaponList = equipements.FindAll(n => n.item.GetType() == typeof(WeaponEquipmentSO));
        if (weaponList.Count>0)
        {
            var weapon = weaponList.Find(n => (n.item as WeaponEquipmentSO).equipmentType == EnumCustom.EquipmentType.Weapon).item as WeaponEquipmentSO;
            controller.CharacterInventory.weapon = weapon;

            var extraWeapon = weaponList.Find(n => (n.item as WeaponEquipmentSO).equipmentType == EnumCustom.EquipmentType.Bow ||
            (n.item as WeaponEquipmentSO).equipmentType == EnumCustom.EquipmentType.Shield)?.item as WeaponEquipmentSO;
            controller.CharacterInventory.extraWeapon = extraWeapon;
        }
        else
        {
            controller.CharacterInventory.weapon = null;
            controller.CharacterInventory.extraWeapon = null;
        }

        statusManager.UpdateStatus();
    }
}