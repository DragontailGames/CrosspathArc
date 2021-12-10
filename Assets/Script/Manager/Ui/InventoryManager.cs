using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gerenciamento do inventario
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public List<ItemInterface> inventory = new List<ItemInterface>();

    public List<SlotController> slotControllers = new List<SlotController>();

    public GameObject inventoryObject;

    public Sprite alchemyIcon;

    public GameObject itemInventory;

    public SlotController slotSelectedController = null;

    public void Start()
    {
        if (inventoryObject == null)
        {
            inventoryObject = Manager.Instance.canvasManager.inventory;
        }

        Transform slots = inventoryObject.transform.Find("Inventory").Find("Slots");
        int y = 0;
        for (int i = 0; i < slots.childCount; i++)
        {
            Transform slot = slots.GetChild(i);

            SlotController slotController = slot.gameObject.AddComponent<SlotController>();
            slotControllers.Add(slotController);
            int x = i - (7 * y);
            slotController.index = new Vector2(x, y);

            if (y < 2)
            {
                slotController.forAlchemy = true;
                slotController.icon = alchemyIcon;

                Instantiate(itemInventory, slotController.transform);
            }
            slotController.inventoryManager = this;
            slotController.Setup();

            if ((i + 1.0f) % 7 == 0)
            {
                y++;
            }
        }
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryObject.activeSelf)//Inventario aberto
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && inventoryObject.activeSelf == true)
        {
            inventoryObject.SetActive(false);
            Manager.Instance.gameManager.SetupPause(false);
            CloseInventory();
        }
    }

    public void OpenInventory()
    {
        Manager.Instance.gameManager.SetupPause(true);

        inventoryObject.SetActive(true);

        foreach (var aux in inventory)
        {
            SetupItemInventory(aux);
        }
    }

    public List<GameObject> listToDestroy = new List<GameObject>();

    public void CloseInventory()
    {
        inventoryObject.SetActive(false);
        Manager.Instance.gameManager.SetupPause(false);
        foreach(var aux in listToDestroy)
        {
            Destroy(aux.gameObject, 0.2f);
        }
    }

    /// <summary>
    /// Define o item na estrutura do inventario mais comentarios no futuro
    /// </summary>
    /// <param name="item"></param>
    public void SetupItemInventory(ItemInterface item)
    {
        SlotController slotController = slotControllers.Find(n => n.index == item.slot);
        GameObject itemAux = Instantiate(itemInventory, slotController.transform);

        Image img = itemAux.GetComponent<Image>();
        img.sprite = item.item.icon;
        img.enabled = true;

        ItemSlotController itemSlotAux = itemAux.AddComponent<ItemSlotController>();
        itemSlotAux.inventoryManager = this;
        itemSlotAux.item = item;
        itemSlotAux.SetupText();
        listToDestroy.Add(itemAux);
    }

    public void SetupEquipment(ItemInterface item)
    {/*
        List<EquipmentUi> equipment = equipmentUi.FindAll(n => n.equipmentType == item.equipmentType);
        if (equipment.Count == 1)
        {
            equipment[0].slot.Find("BackItem").gameObject.SetActive(false);
            Image icon = equipment[0].slot.Find("Icon").GetComponent<Image>();
            icon.enabled = true;
            icon.sprite = item.icon;
        }
        else if (equipment.Count >= 1)
        {
            if (equipment[1].slot.Find("BackItem").gameObject.activeSelf)
            {
                equipment[1].slot.Find("BackItem").gameObject.SetActive(false);
                Image icon1 = equipment[1].slot.Find("Icon").GetComponent<Image>();
                icon1.enabled = true;
                icon1.sprite = item.icon;
                return;
            }
            equipment[0].slot.Find("BackItem").gameObject.SetActive(false);
            Image icon = equipment[0].slot.Find("Icon").GetComponent<Image>();
            icon.enabled = true;
            icon.sprite = item.icon;
        }
        else
        {
            Debug.LogError("Tipo de equipamento não encontrado");
        }*/
    }

    public Vector2 GetNextSlot()
    {
        foreach(var aux in slotControllers)
        {
            if(aux.transform.childCount<=0 && aux.forAlchemy == false)
            {
                return aux.index;
            }
        }

        return Vector2.zero;
    }
}
