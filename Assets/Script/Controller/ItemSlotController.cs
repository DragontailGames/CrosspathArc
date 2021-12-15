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

    public ItemInventory itemInventory;

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

        itemInventory.equiped = false;
        Manager.Instance.characterController.CharacterInventory.UpdateEquipmentList();
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
        itemInventory.slot = slotController.index;

        if(slotController.GetType() == typeof(SlotEquipmentController))
        {
            this.transform.localScale = Vector2.one * 4;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = Input.mousePosition;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2 && eventData.button == PointerEventData.InputButton.Left)
        {
            if(itemInventory.item.GetType() == typeof(ConsumableSO))
            {
                Debug.Log("Teste");
                itemInventory.qtd--;
                (itemInventory.item as ConsumableSO).Consume();
                SetupText();
                if (itemInventory.qtd <= 0)
                {
                    inventoryManager.inventory.Remove(itemInventory);
                    DestroyImmediate(this.gameObject);
                }
            }
        }
    }

    public void SetupText()
    {
        TextMeshProUGUI txt = this.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        txt.text = itemInventory.qtd.ToString();
        if (itemInventory.qtd > 1)
        {
            txt.enabled = true;
        }
    }
}
