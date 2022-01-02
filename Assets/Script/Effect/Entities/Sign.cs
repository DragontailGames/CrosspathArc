using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class Sign : CenarioEntity
{
    [TextArea(0,10)]
    public string text;

    public GameObject panel;

    public SignsController sign;

    public override void Start()
    {
        if(sign == null)
        {
            sign = FindObjectOfType<SignsController>();
        }

        base.Start();

        spriteRenderer = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
        baseColor = this.transform.GetChild(0).GetComponent<SpriteRenderer>().color;
    }

    public override void OnMouseDown()
    {
        if (Vector3Int.Distance(Manager.Instance.characterController.currentTileIndex, currentTileIndex) < 3)
        {
            base.OnMouseDown();
            sign.OpenSign();
            panel.SetActive(true);
            panel.GetComponentInChildren<TextMeshProUGUI>().text = text;
        }
    }
}
