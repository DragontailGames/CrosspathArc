using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Sign : CenarioEntity
{
    public Color mouseOverColor;

    private Color baseColor;

    [TextArea(0,10)]
    public string text;

    public GameObject panel;

    public override void Start()
    {
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

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Manager.Instance.gameManager.SetupPause(false);
            panel.transform.parent.gameObject.SetActive(false);
            panel.SetActive(false);
        }
    }

    public override void OnMouseDown()
    {
        base.OnMouseDown();
        gameManager.SetupPause(true);
        panel.transform.parent.gameObject.SetActive(true);
        panel.SetActive(true);
        panel.GetComponentInChildren<TextMeshProUGUI>().text = text;

    }
}
