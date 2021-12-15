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
            if(objItem.GetComponent<ItemSlotController>().itemInventory.item.GetType() == typeof(EquipmentSO))
            {
                EquipmentSO equipment = objItem.GetComponent<ItemSlotController>().itemInventory.item as EquipmentSO;
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
            if (this.transform.GetComponentInChildren<ItemSlotController>())
            {
                ItemSlotController auxObject = this.transform.GetComponentInChildren<ItemSlotController>();
                auxObject.slotController = objItem.GetComponent<ItemSlotController>().slotController;
                auxObject.itemInventory.slot = objItem.GetComponent<ItemSlotController>().slotController.index;
                auxObject.transform.SetParent(objItem.GetComponent<ItemSlotController>().slotController.transform);
                auxObject.transform.localPosition = Vector2.zero;

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
            ItemInventory itemAux = objItem.GetComponent<ItemSlotController>().itemInventory;

            ItemInventory itemFinded = characterInventory.equipements.Find(n => (n.item as EquipmentSO).equipmentType == (itemAux.item as EquipmentSO).equipmentType);
            if (itemFinded != null)
            {
                characterInventory.equipements.Remove(itemFinded);
            }
            characterInventory.equipements.Add(itemAux);

            characterInventory.UpdateEquipmentList();
        }
    }
}
