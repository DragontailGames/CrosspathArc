using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bag : CenarioEntity
{
    public List<ItemSO> items;

    public bool chest = true;

    public override void Start()
    {
        base.Start();

        if (this.GetComponent<SpriteRenderer>())
        {
            spriteRenderer = this.GetComponent<SpriteRenderer>();
        }
        else
        {
            spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
        }
        baseColor = spriteRenderer.color;
        this.gameObject.AddComponent<ChestController>();
        Invoke("SetupInventory", 0.2f);
    }

    public void SetupInventory()
    {
        foreach (var aux in items)
        {
            this.GetComponent<ChestController>().AddItem(aux);
        }
    }

    public override void OnMouseDown()
    {
        if (Vector3Int.Distance(Manager.Instance.characterController.currentTileIndex, currentTileIndex) < 3)
        {
            this.GetComponent<ChestController>().OpenChest();
        }
    }
}
