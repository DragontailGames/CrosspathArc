using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public void SelectPlayer_01()
    {
        PlayerPrefs.SetString("PLAYER", "1");
        Manager.Instance.sceneLoadManager.GotoGame();
    }

    public void SelectPlayer_02()
    {
        PlayerPrefs.SetString("PLAYER", "2");
        Manager.Instance.sceneLoadManager.GotoGame();
    }
}
