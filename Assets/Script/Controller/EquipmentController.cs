using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EquipmentController : MonoBehaviour
{
    private List<SlotEquipmentController> slotEquipmentControllers = new List<SlotEquipmentController>();

    public void Start()
    {
        Manager.Instance.inventoryManager.equipmentController = this;

        foreach(Transform aux in this.transform)
        {
            slotEquipmentControllers.Add(aux.GetComponent<SlotEquipmentController>());
        }
    }

    public void OpenEquipmentController()
    {
        foreach(var aux in Manager.Instance.characterController.CharacterInventory.equipements)
        {
            SlotEquipmentController slotEquipmentController = slotEquipmentControllers.Find(n => n.equipmentType == (aux.item as EquipmentSO).equipmentType);
            Manager.Instance.inventoryManager.SetupItemInventory(aux.item, slotEquipmentController.transform);
        }
    }

    public void CloseInventory()
    {
        foreach (var aux in Manager.Instance.characterController.CharacterInventory.equipements.ToList())
        {
            SlotEquipmentController slotEquipmentController = slotEquipmentControllers.Find(n => n.equipmentType == (aux.item as EquipmentSO).equipmentType);
            if(slotEquipmentController.GetComponentInChildren<ItemSlotController>() == null)
            {
                Manager.Instance.characterController.CharacterInventory.equipements.Remove(aux);
            }
        }

        Manager.Instance.characterController.CharacterInventory.UpdateEquipmentList();
    }
}
