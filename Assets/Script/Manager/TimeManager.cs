using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public int nextTimeChange = 0;
    public bool nightShift = false;

    public Image glow;
    public RawImage sky;

    public GameObject lowVisionPanel;

    //public DayDurationController dayDurationController;

    private Color dayColor = new Color(1,0.94f,0.64f,1);
    private Color nightColor = new Color(0.76f, 0.81f, 0.9f, 1);

    public void Awake()
    {
        Manager.Instance.timeManager = this;
    }

    private void Start()
    {
        nextTimeChange = Manager.Instance.configManager.dayTurns;
    }

    public void StartNewTurn()
    {
        nextTimeChange--;
        if(nextTimeChange <= 0)
        {
            Rect rect = sky.uvRect;
            if (nightShift == false)
            {
                nightShift = true;
                nextTimeChange = Manager.Instance.configManager.nigthTurns;
                glow.color = nightColor;
                rect.x = 0.5f;
                lowVisionPanel.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
            else
            {
                nightShift = false;
                nextTimeChange = Manager.Instance.configManager.dayTurns;
                glow.color = dayColor;
                rect.x = 0;
                lowVisionPanel.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
            }
            sky.uvRect = rect;
        }
        if (nextTimeChange <= 5)
        {
            int alpha = 0;
            float percent = 0;
            Rect rect = sky.uvRect;
            if (!nightShift)
            {
                percent = (5 - nextTimeChange) / 5.0f;
                alpha = Mathf.RoundToInt(255 * percent);
            }
            else
            {
                percent = nextTimeChange / 5.0f;
                alpha = Mathf.RoundToInt(255 * percent);
            }

            glow.color = Color.Lerp(dayColor, nightColor, percent);
            rect.x = Mathf.Lerp(0f, 0.5f, percent);
            sky.uvRect = rect;
            lowVisionPanel.GetComponent<Image>().color = new Color32(255, 255, 255, (byte)alpha);
        }
    }
}
