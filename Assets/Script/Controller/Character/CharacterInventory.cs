using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inventario simples para o combate do jogador
/// </summary>
public class CharacterInventory : MonoBehaviour
{
    public CharacterController controller;

    public WeaponEquipmentSO handStandartEquipment;

    private WeaponEquipmentSO weapon;

    public EquipmentSO extraWeapon;

    public InventoryManager inventoryManager;

    public List<ItemInventory> equipements;

    public StatusManager statusManager;

    public WeaponEquipmentSO Weapon { get {
            if (this.weapon != null)
                return this.weapon;
            else
                return this.handStandartEquipment;
            } set => this.weapon = value; }

    public void UpdateEquipmentList()
    {
        int extraHp = 0;
        int extraMp = 0;
        List<Status> statusAux = new List<Status>();
        List<Attributes> attributesAux = new List<Attributes>();

        SupportStatus supportStatus;

        ChestEquipmentSO chest = null;

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
                    chest = chestEquipment;
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

        List<EquipmentSO> weaponList = new List<EquipmentSO>();
        foreach(var aux in equipements)
        {
            var equipAux = (aux.item as EquipmentSO);
            if(equipAux.equipmentType == EnumCustom.EquipmentType.Shield ||
                equipAux.equipmentType == EnumCustom.EquipmentType.Weapon ||
                equipAux.equipmentType == EnumCustom.EquipmentType.Bow)
            {
                weaponList.Add(equipAux);
            }
        }
        if (weaponList.Count>0)
        {
            var weapon = weaponList.Find(n => n.equipmentType == EnumCustom.EquipmentType.Weapon);
            controller.CharacterInventory.Weapon = weapon as WeaponEquipmentSO;

            var extraWeapon = weaponList.Find(n => n.equipmentType == EnumCustom.EquipmentType.Bow ||
            n.equipmentType == EnumCustom.EquipmentType.Shield) as EquipmentSO;
            controller.CharacterInventory.extraWeapon = extraWeapon;
        }
        else
        {
            controller.CharacterInventory.Weapon = null;
            controller.CharacterInventory.extraWeapon = null;
        }

        statusManager.UpdateStatus();

        this.controller.animator.GetComponent<AnimatorEquipmentController>().SetController(chest == null? EnumCustom.ArmorType.None : chest.animatorName, 
            controller.CharacterInventory.extraWeapon == null ? false : controller.CharacterInventory.extraWeapon.equipmentType == EnumCustom.EquipmentType.Shield,
            controller.CharacterInventory.Weapon == null ? false : controller.CharacterInventory.Weapon.weaponType == EnumCustom.WeaponType.Sword) ;
    }
}