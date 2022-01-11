using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotEquipmentController : SlotController, IPointerEnterHandler, IPointerExitHandler
{
    public EnumCustom.EquipmentType equipmentType;

    public ItemInventory currentItem;

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

            if(equipmentType != (itemAux.item as EquipmentSO)?.equipmentType)
            {
                objItem.transform.localScale = Vector2.one;
                return;
            }

            ItemSlotController oldObject = null;

            if (this.transform.GetComponentInChildren<ItemSlotController>())
            {
                oldObject = this.transform.GetComponentInChildren<ItemSlotController>();
                oldObject.slotController = objItem.GetComponent<ItemSlotController>().slotController;
                oldObject.itemInventory.slot = objItem.GetComponent<ItemSlotController>().slotController.index;
                oldObject.transform.SetParent(objItem.GetComponent<ItemSlotController>().slotController.transform);
                oldObject.transform.localPosition = Vector2.zero;
                oldObject.transform.localScale = Vector2.one;

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

            if (currentItem != null)
            {
                characterInventory.equipements.Remove(currentItem);
            }

            currentItem = itemAux;

            characterInventory.equipements.Add(itemAux);

            characterInventory.UpdateEquipmentList();
        }
    }
}
