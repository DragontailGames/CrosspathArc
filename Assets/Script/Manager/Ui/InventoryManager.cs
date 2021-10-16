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

    public List<Equipment> tempSetupItem;

    public GameObject inventory;

    public void Start()
    {
        if (inventory == null)
        {
            inventory = Manager.Instance.canvasManager.inventory;
        }
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            OpenInventory();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && inventory.activeSelf == true)
        {
            inventory.SetActive(false);
            Manager.Instance.gameManager.SetupPause(false);
        }
    }

    public void OpenInventory()
    {
        Manager.Instance.gameManager.SetupPause(true);

        if (inventory.activeSelf)//Inventario aberto
        {
            inventory.SetActive(false);
            Manager.Instance.gameManager.SetupPause(false);
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
    public void SetupItem(Equipment item)
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
