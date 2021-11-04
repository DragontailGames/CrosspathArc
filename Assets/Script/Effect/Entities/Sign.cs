using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class Sign : CenarioEntity
{
    public Color mouseOverColor;

    private Color baseColor;

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
        baseColor = this.transform.GetChild(0).GetComponent<SpriteRenderer>().color;
    }

    public override void OnMouseOver()
    {
        base.OnMouseOver();
        this.transform.GetChild(0).GetComponent<SpriteRenderer>().color = mouseOverColor;
    }

    public override void OnMouseExit()
    {
        base.OnMouseExit();
        this.transform.GetChild(0).GetComponent<SpriteRenderer>().color = baseColor;

    }

    public override void OnMouseDown()
    {
        base.OnMouseDown();
        sign.OpenSign();
        panel.SetActive(true);
        panel.GetComponentInChildren<TextMeshProUGUI>().text = text;

    }
}
