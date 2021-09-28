using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NigthController : MonoBehaviour
{
    void Update()
    {
        this.transform.position = Manager.Instance.characterController.transform.position;
    }
}
