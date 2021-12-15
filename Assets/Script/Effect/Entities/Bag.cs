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

        spriteRenderer = this.GetComponent<SpriteRenderer>();
        baseColor = this.GetComponent<SpriteRenderer>().color;
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
        this.GetComponent<ChestController>().OpenChest();
    }
}
