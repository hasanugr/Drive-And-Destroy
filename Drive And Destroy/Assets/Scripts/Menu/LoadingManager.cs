using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class LoadingManager : MonoBehaviour
{
    public GameObject LoadingCanvas;
    public Slider Slider;
    public TextMeshProUGUI ProgressText;

    public void LoadLevel(int levelIndex)
    {
        StartCoroutine(LoadProgress(levelIndex));
    }

    IEnumerator LoadProgress(int levelIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelIndex);
        LoadingCanvas.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp(operation.progress / 0.9f, 0, 1);
            Slider.value = progress;
            ProgressText.text = "%" + Mathf.CeilToInt(progress * 100);


            yield return null;
        }
    }
}
