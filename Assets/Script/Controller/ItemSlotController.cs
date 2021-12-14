using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ItemSlotController : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerClickHandler
{
    public InventoryManager inventoryManager;

    private CanvasGroup canvasGroup;

    public SlotController slotController;

    public ItemInterface item;

    public void Awake()
    {
        canvasGroup = this.GetComponent<CanvasGroup>();
        slotController = this.GetComponentInParent<SlotController>();
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        inventoryManager.dragItem = this;

        this.transform.SetParent(this.transform.parent.parent.parent);
        canvasGroup.blocksRaycasts = false;

        inventoryManager.dropItemController.SetActive(true);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        inventoryManager.dragItem = null;

        canvasGroup.blocksRaycasts = true;
        Invoke("BackToController", 0.1f);

        inventoryManager.dropItemController.SetActive(false);

    }

    public void BackToController()
    {
        this.transform.SetParent(slotController.transform);
        this.transform.localPosition = Vector2.zero;
        item.slot = slotController.index;
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = Input.mousePosition;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            if(item.item.GetType() == typeof(ConsumableSO))
            {
                item.qtd--;
                (item.item as ConsumableSO).Consume();
                SetupText();
                if (item.qtd <= 0)
                {
                inventoryManager.inventory.Remove(item);
                DestroyImmediate(this.gameObject);
                }
            }
        }
    }

    public void SetupText()
    {
        TextMeshProUGUI txt = this.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        txt.text = item.qtd.ToString();
        txt.enabled = true;
    }
}
