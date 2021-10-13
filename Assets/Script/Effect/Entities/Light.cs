using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light : CenarioEntity
{
    public int size = 10;

    public override void Start()
    {
        base.Start();
        SetupLightSize();
    }

    public void SetupLightSize()
    {
        this.transform.GetChild(0).localScale = Vector3.one * size / 10;
    }
}
