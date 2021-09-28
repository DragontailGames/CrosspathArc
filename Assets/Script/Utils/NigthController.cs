using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NigthController : MonoBehaviour
{
    public GameObject lightEffect;

    private void Awake()
    {
        foreach(var aux in FindObjectsOfType<Light>())
        {
            var tempLight = Instantiate(lightEffect, aux.transform);
            tempLight.transform.localPosition = Vector3.zero;
        }
    }

    void Update()
    {
        this.transform.position = Manager.Instance.characterController.transform.position;
    }
}
