using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatusManager : MonoBehaviour
{

    public GameObject status, attributes;

    void Start()
    {
        for(int i =0;i<status.transform.childCount;i++)
        {
            status.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = ((EnumCustom.Status)i).ToString();
        }
        for (int i = 0; i < attributes.transform.childCount; i++)
        {
            attributes.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = ((EnumCustom.Attribute)i).ToString();
        }
    }
}
