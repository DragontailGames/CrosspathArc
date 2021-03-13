using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gerenciamento do inventario
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public List<EquipmentUi> equipmentUi;

    public List<Item> tempSetupItem;

    public GameObject inventory;

    public GameManager manager;

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            OpenInventory();
        }
        manager.InPause = inventory.activeSelf;
    }

    public void OpenInventory()
    {
        if(inventory.activeSelf)
        {
            inventory.SetActive(false);
            return;
        }

        inventory.SetActive(true);

        foreach (var aux in tempSetupItem)
        {
            SetupItem(aux);
        }
    }

    /// <summary>
    /// Define o item na estrutura do inventario mais comentarios no futuro
    /// </summary>
    /// <param name="item"></param>
    public void SetupItem(Item item)
    {
        List<EquipmentUi> equipment = equipmentUi.FindAll(n => n.equipmentType == item.equipmentType);
        if(equipment.Count == 1)
        {
            equipment[0].slot.Find("BackItem").gameObject.SetActive(false);
            Image icon = equipment[0].slot.Find("Icon").GetComponent<Image>();
            icon.enabled = true;
            icon.sprite = item.icon;
        }
        else if(equipment.Count >= 1)
        {
            if(equipment[1].slot.Find("BackItem").gameObject.activeSelf)
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
        }
    }
}
