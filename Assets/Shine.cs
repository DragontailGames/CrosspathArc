using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shine : MonoBehaviour
{
    void Update()
    {
        this.GetComponent<Image>().material.SetFloat("_ShineLocation", Mathf.PingPong(Time.time, 1));//Função simples que faz a animação da skill selecionada
    }
}
