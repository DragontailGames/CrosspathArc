using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MouseTipsManager : MonoBehaviour
{
    private Vector3 fixPosition = new Vector3();

    void Awake()
    {
        Manager.Instance.mouseTipsManager = this;
        this.gameObject.SetActive(false);
    }

    public void ShowMessage(string msg)
    {
        this.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = msg;
        fixPosition = new Vector3(this.GetComponent<RectTransform>().rect.width / 1.9f, this.GetComponent<RectTransform>().rect.height / 2.2f, 0);
        this.transform.position = Input.mousePosition + fixPosition;
        this.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
    }

    public void Update()
    {
        fixPosition = new Vector3(this.GetComponent<RectTransform>().rect.width / 1.9f, this.GetComponent<RectTransform>().rect.height / 2.2f, 0);
        this.transform.position = Input.mousePosition + fixPosition;
    }

    public void HideMessage()
    {
        this.gameObject.SetActive(false);
    }
}
