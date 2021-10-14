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
        operation.allowSceneActivation = false;

        float count = 0;

        while (!operation.isDone && count<=10)
        {
            yield return new WaitForSeconds(0.1f);
            float progress = count / 10.0f;

            fillLoading.fillAmount = progress;

            count += Random.Range(0.2f,0.6f);
        }

        operation.allowSceneActivation = true;
    }
}
