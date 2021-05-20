using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlinkEffect : MonoBehaviour
{
    private TextMeshProUGUI text;


    void Start()
    {
        this.TryGetComponent(out text);
    }

    // Update is called once per frame
    void Update()
    {
        
       if(text != null)
        {
            Color32 c = text.color;
            c.a = (byte)GetAlpha();
            text.color = c;
        }
    }

    public float GetAlpha()
    {
        float a = (Mathf.PingPong(Time.time * 500, 255.0f - 50.0f) + 50.0f);
        return a;
    }
}
