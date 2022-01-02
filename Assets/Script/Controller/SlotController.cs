using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SlotController : MonoBehaviour, IDropHandler, IPointerDownHandler
{
    public Vector2 index;

    public bool forAlchemy = false;

    public Sprite icon;

    public InventoryManager inventoryManager;

    public void Setup()
    {
        if (forAlchemy)
        {
            this.GetComponent<Image>().color = new Color32(197, 253, 185, 255);

            Image img = this.transform.GetChild(0).GetComponent<Image>();
            img.sprite = icon;
            img.enabled = true;
            Color32 c = img.color;
            c.a = 100;
            img.color = c;
        }
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag != null && !forAlchemy)
        {
            Transform objItem = eventData.pointerDrag.transform;
            if (this.transform.childCount > 0)
            {
                if (this.transform.GetChild(0).GetComponent<ItemSlotController>().itemInventory.item.itemName == objItem.GetComponent<ItemSlotController>().itemInventory.item.itemName)
                {
                    if(this.transform.GetChild(0).GetComponent<ItemSlotController>().itemInventory.item.stackable > this.transform.GetChild(0).GetComponent<ItemSlotController>().itemInventory.qtd)
                    {
                        this.transform.GetChild(0).GetComponent<ItemSlotController>().itemInventory.qtd += objItem.GetComponent<ItemSlotController>().itemInventory.qtd;
                        this.transform.GetChild(0).GetComponent<ItemSlotController>().SetupText();
                        Destroy(objItem.gameObject);
                    }
                    else
                    {
                        SwitchItems(objItem);
                    }
                }
                else
                {
                    SwitchItems(objItem);
                }
            }
            else
            {
                objItem.GetComponent<ItemSlotController>().slotController = this;
                objItem.SetParent(this.transform);
                objItem.localPosition = Vector2.zero;
            }

            /*
            if (inventoryManager.inventory.Find(n => n.item.itemName == itemSlotController.item.item.itemName) == null)
            {
                inventoryManager.inventory.Add(new ItemInterface)
            }
            else
            {
                if (inventoryManager.inventory.Find(n => n.qtd == itemSlotController.item.qtd) == null)
                {

                }
            }*/
        }
    }

    public void SwitchItems(Transform objItem)
    {

        Transform auxObject = this.transform.GetChild(0);
        auxObject.GetComponent<ItemSlotController>().slotController = objItem.GetComponent<ItemSlotController>().slotController;
        auxObject.GetComponent<ItemSlotController>().itemInventory.slot = objItem.GetComponent<ItemSlotController>().slotController.index;
        auxObject.SetParent(objItem.GetComponent<ItemSlotController>().slotController.transform);
        auxObject.localPosition = Vector2.zero;

        objItem.GetComponent<ItemSlotController>().slotController = this;
        objItem.GetComponent<ItemSlotController>().itemInventory.slot = this.index;
        objItem.SetParent(this.transform);
        objItem.localPosition = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right && inventoryManager?.dragItem != null)
        {
            if (inventoryManager.dragItem.itemInventory.qtd > 1)
            {
                if (this.transform.GetChild(0).GetComponent<ItemSlotController>().itemInventory.item.stackable > this.transform.GetChild(0).GetComponent<ItemSlotController>().itemInventory.qtd)
                {
                    if (this.transform.childCount <= 0)
                    {
                        GameObject clone = Instantiate(inventoryManager.dragItem.gameObject, this.transform);

                        clone.GetComponent<ItemSlotController>().slotController = this;
                        clone.GetComponent<ItemSlotController>().itemInventory.qtd = 1;
                        clone.GetComponent<ItemSlotController>().itemInventory.slot = this.index;
                        clone.GetComponent<ItemSlotController>().SetupText();
                        clone.transform.localPosition = Vector2.zero;

                        clone.GetComponent<CanvasGroup>().blocksRaycasts = true;

                        inventoryManager.dragItem.itemInventory.qtd--;
                        inventoryManager.dragItem.SetupText();
                    }
                    else if (this.transform.GetChild(0).GetComponent<ItemSlotController>().itemInventory.item.itemName == inventoryManager.dragItem.itemInventory.item.itemName)
                    {
                        this.transform.GetChild(0).GetComponent<ItemSlotController>().itemInventory.qtd++;
                        this.transform.GetChild(0).GetComponent<ItemSlotController>().SetupText();
                        inventoryManager.dragItem.itemInventory.qtd--;
                        inventoryManager.dragItem.SetupText();
                    }
                }
            }
        }
    }
}
