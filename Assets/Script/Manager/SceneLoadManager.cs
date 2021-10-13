using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadManager : MonoBehaviour
{
    public Image fillLoading;

    public GameObject loadingObject;

    void Awake()
    {
        Manager.Instance.sceneLoadManager = this;
        QualitySettings.asyncUploadTimeSlice = 4;
        QualitySettings.asyncUploadBufferSize = 16;
        QualitySettings.asyncUploadPersistentBuffer = true;
    }

    public void GotoMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void GotoGame()
    {
        StartCoroutine(LoadAsynchronously(1));
    }

    public void GotoScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        loadingObject.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);

            Debug.Log(operation.progress);

            fillLoading.fillAmount = progress;

            yield return null;
        }
    }
}
