using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light : MonoBehaviour
{
    public int size = 10;

    public void Start()
    {
        SetupLightSize();
    }

    public void SetupLightSize()
    {
        this.transform.localScale = Vector3.one * size / 10;
    }
}
