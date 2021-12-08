using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SlotController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryManager.slotSelectedController = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryManager.slotSelectedController = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag != null && !forAlchemy)
        {
            Transform objItem = eventData.pointerDrag.transform;
            objItem.GetComponent<ItemSlotController>().slotController = this;
            objItem.SetParent(this.transform);
            objItem.localPosition = Vector2.zero;
        }
    }
}
