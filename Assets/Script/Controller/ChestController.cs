using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestController : MonoBehaviour
{
    public List<ItemInventory> itemInterfaces = new List<ItemInventory>();

    public List<SlotController> slotControllers = new List<SlotController>();

    private GameObject chestUi;

    public void Start()
    {
        chestUi = Manager.Instance.inventoryManager.chestUi;
        Transform slots = chestUi.transform.Find("Inventory");
        int y = 0;
        for (int i = 0; i < slots.childCount; i++)
        {
            Transform slot = slots.GetChild(i);

            SlotController slotController = slot.gameObject.AddComponent<SlotController>();
            slotControllers.Add(slotController);
            int x = i - (5 * y);
            slotController.index = new Vector2(x, y);
            slotController.inventoryManager = Manager.Instance.inventoryManager;
            slotController.Setup();

            if ((i + 1.0f) % 5 == 0)
            {
                y++;
            }
        }
    }

    public void OpenChest()
    {
        Manager.Instance.inventoryManager.OpenInventory();

        chestUi.gameObject.SetActive(true);

        foreach (var aux in itemInterfaces)
        {
            SetupItemInventory(aux);
        }
    }

    public void AddItem(ItemSO item)
    {
        try
        {
            itemInterfaces.Find(n => n.item.itemName == item.itemName).qtd++;
        }
        catch
        {
            itemInterfaces.Add(new ItemInventory(GetNextSlot(), 1, item, false));
        }
    }

    public void SetupItemInventory(ItemInventory item)
    {
        SlotController slotController = slotControllers.Find(n => n.index == item.slot);
        GameObject itemAux = Instantiate(Manager.Instance.inventoryManager.itemInventory, slotController.transform);

        Image img = itemAux.GetComponent<Image>();
        img.sprite = item.item.icon;
        img.enabled = true;

        ItemSlotController itemSlotAux = itemAux.AddComponent<ItemSlotController>();
        itemSlotAux.inventoryManager = Manager.Instance.inventoryManager;
        itemSlotAux.itemInventory = item;
        itemSlotAux.SetupText();
        Manager.Instance.inventoryManager.listToDestroy.Add(itemAux);
    }

    public Vector2 GetNextSlot()
    {
        foreach (var aux in slotControllers)
        {
            if (itemInterfaces.Find(n => n.slot == aux.index) == null)
            {
                return aux.index;
            }
        }

        return Vector2.zero;
    }

    public void Close()
    {
        if(!chestUi.gameObject.activeSelf)return;

        chestUi.gameObject.SetActive(false);

        itemInterfaces = new List<ItemInventory>();

        foreach(var aux in slotControllers)
        {
            if(aux.transform.childCount>0)
            {
                itemInterfaces.Add(aux.GetComponentInChildren<ItemSlotController>().itemInventory);
            }
        }

        if(this.GetComponent<Bag>().chest==false && itemInterfaces.Count<=0 )
        {
            DestroyImmediate(this.gameObject);
        }
    }

}
