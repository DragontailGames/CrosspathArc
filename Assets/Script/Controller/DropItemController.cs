using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropItemController : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            Transform objItem = eventData.pointerDrag.transform;
            ItemSlotController itemSlotController = objItem.GetComponent<ItemSlotController>();
            GameObject bagAux = Instantiate(
                Manager.Instance.inventoryManager.bag, 
                Manager.Instance.gameManager.tilemap.CellToWorld(Manager.Instance.characterController.currentTileIndex) + 
                (Vector3.up * 0.25f), Quaternion.identity);
            for(int i = 0; i<itemSlotController.item.qtd;i++)
            {
                bagAux.GetComponent<Bag>().items.Add(itemSlotController.item.item);
            }
            objItem.GetComponent<ItemSlotController>().inventoryManager.inventory.Remove(itemSlotController.item);
            DestroyImmediate(objItem.gameObject);
            this.gameObject.SetActive(false);
        }
    }
}
