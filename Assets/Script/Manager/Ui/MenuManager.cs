using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class MenuManager : MonoBehaviour
{
    private bool open = false;

    GameManager gameManager;

    private void Start()
    {
        gameManager = Manager.Instance.gameManager;
        CloseMenu();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (open)
            {
                CloseMenu();
            }
            else if (!gameManager.InPause)
            {
                OpenMenu();
            }
        }
    }

    public void OpenMenu()
    {
        gameManager.SetupPause(true);
        open = true;

        this.GetComponent<Image>().enabled = true;
        this.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        gameManager.SetupPause(false);
        open = false;

        this.GetComponent<Image>().enabled = false;
        this.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void Btn_Continue()
    {
        CloseMenu();
    }

    public void Btn_Save()
    {
        Debug.Log("Not implemented yet");
    }

    public void Btn_Load()
    {
        Debug.Log("Not implemented yet");
    }

    public void Btn_Options()
    {
        Debug.Log("Not implemented yet");
    }

    public void Btn_Exit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif            
        Application.Quit();
    }
}
