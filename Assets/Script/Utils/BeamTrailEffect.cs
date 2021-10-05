using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamTrailEffect : MonoBehaviour
{
    public Transform originPoision;

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<LineRenderer>().SetPosition(0, this.transform.position);
        this.GetComponent<LineRenderer>().SetPosition(1, originPoision.position);
    }
}
