using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    void Awake()
    {
        Manager.Instance.sceneLoadManager = this;
    }

    public void GotoMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void GotoGame()
    {
        SceneManager.LoadScene(1);
    }
}
