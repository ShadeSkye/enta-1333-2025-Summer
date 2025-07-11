using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    private AsyncOperation _operation;

    private void Awake()
    {
        instance = this;
        LoadScene("MainGame");
    }

    private void LoadScene(string sceneName)
    {
        _operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        _operation.allowSceneActivation = false;
        _operation.completed += (AsyncOperation obj) =>
        {
            Scene loadedScene = SceneManager.GetSceneByPath(sceneName);
            Debug.Log($"{sceneName} finished loading (build index: {loadedScene.buildIndex}).");
            Debug.Log($"It has {loadedScene.rootCount} root(s).");
            Debug.Log($"There are now {SceneManager.loadedSceneCount} Scenes open.");
        };
    }

    private void LoadScreenActive()
    {
        _operation.allowSceneActivation = true;
    }
}
