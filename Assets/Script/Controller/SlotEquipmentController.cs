using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotEquipmentController : SlotController, IPointerEnterHandler, IPointerExitHandler
{
    public EnumCustom.EquipmentType equipmentType;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            Transform objItem = eventData.pointerDrag.transform;

            ItemSO itemAux = objItem.GetComponent<ItemSlotController>().itemInventory.item;
            if (itemAux.GetType() == typeof(EquipmentSO) || itemAux.GetType() == typeof(ChestEquipmentSO))
            {
                EquipmentSO equipment = itemAux as EquipmentSO;
                if (equipment.equipmentType == equipmentType)
                {
                    objItem.localScale = (Vector2.one * 2f);
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            Transform objItem = eventData.pointerDrag.transform;
            objItem.localScale = Vector2.one;
        }
    }

    public override void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            Transform objItem = eventData.pointerDrag.transform;
            ItemInventory itemAux = objItem.GetComponent<ItemSlotController>().itemInventory;

            if(equipmentType != (itemAux.item as EquipmentSO).equipmentType)
            {
                return;
            }

            if (this.transform.GetComponentInChildren<ItemSlotController>())
            {
                ItemSlotController auxObject = this.transform.GetComponentInChildren<ItemSlotController>();
                auxObject.slotController = objItem.GetComponent<ItemSlotController>().slotController;
                auxObject.itemInventory.slot = objItem.GetComponent<ItemSlotController>().slotController.index;
                auxObject.transform.SetParent(objItem.GetComponent<ItemSlotController>().slotController.transform);
                auxObject.transform.localPosition = Vector2.zero;
                auxObject.transform.localScale = Vector2.one;

                objItem.GetComponent<ItemSlotController>().slotController = this;
                objItem.GetComponent<ItemSlotController>().itemInventory.slot = this.index;
                objItem.SetParent(this.transform);
                objItem.localPosition = Vector2.zero;
            }
            else
            {
                objItem.GetComponent<ItemSlotController>().slotController = this;
                objItem.SetParent(this.transform);
                objItem.localPosition = Vector2.zero;
            }

            objItem.localScale = (Vector2.one * 3f);
            objItem.GetComponent<ItemSlotController>().itemInventory.equiped = true;

            CharacterInventory characterInventory = Manager.Instance.characterController.CharacterInventory;

            ItemInventory itemFinded = characterInventory.equipements.Find(n => (n.item as EquipmentSO).equipmentType == (itemAux.item as EquipmentSO).equipmentType && 
            (n.item as EquipmentSO).equipmentType != EnumCustom.EquipmentType.Ring);

            if (itemFinded != null)
            {
                characterInventory.equipements.Remove(itemFinded);
            }

            /*
            if(equipmentType == EnumCustom.EquipmentType.Weapon)
            {
                characterInventory.weapon = (itemAux.item as WeaponEquipmentSO);
            }

            if (equipmentType == EnumCustom.EquipmentType.Bow || equipmentType == EnumCustom.EquipmentType.Shield)
            {
                characterInventory.extraWeapon = (itemAux.item as WeaponEquipmentSO);
            }*/

            characterInventory.equipements.Add(itemAux);

            characterInventory.UpdateEquipmentList();
        }
    }
}
