using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignsController : MonoBehaviour
{
    public bool hasSignOpen = false;

    public void CloseSign()
    {
        if(hasSignOpen)
        {
            Manager.Instance.gameManager.SetupPause(false);
            this.gameObject.SetActive(false);
            foreach (Transform aux in this.transform)
            {
                aux.gameObject.SetActive(false);
            }

            hasSignOpen = false;
            this.GetComponent<Image>().enabled = false;
        }
    }

    public void OpenSign()
    {
        Manager.Instance.gameManager.SetupPause(true);
        this.gameObject.SetActive(true);
        StartCoroutine(DelayToOpen());
        this.GetComponent<Image>().enabled = true;
    }

    public IEnumerator DelayToOpen()
    {
        yield return new WaitForSeconds(0.2f);
        hasSignOpen = true;
    }


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseSign();
        }
    }
}
