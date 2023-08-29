using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LoadingNextScene : MonoBehaviour
{
    public int sceneNumber = 2;

    public Slider loadingBar;
    public TMP_Text loadingText;

    private void Start()
    {
        StartCoroutine(AsyncNextScene(sceneNumber));
    }

    IEnumerator AsyncNextScene(int num)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(num);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            loadingBar.value = asyncOperation.progress;
            loadingText.text = (asyncOperation.progress * 100.0f) + " %";

            if(asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
