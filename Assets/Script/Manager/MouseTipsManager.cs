using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MouseTipsManager : MonoBehaviour
{
    void Awake()
    {
        this.gameObject.SetActive(false);
    }

    public void ShowMessage(string msg)
    {
        this.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = msg;
        this.transform.position = Input.mousePosition + Vector3.right * this.GetComponent<RectTransform>().rect.width / 3;
        this.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());

    }

    public void Update()
    {
        this.transform.position = Input.mousePosition + Vector3.right * this.GetComponent<RectTransform>().rect.width/3;
    }

    public void HideMessage()
    {
        this.gameObject.SetActive(false);
    }
}
