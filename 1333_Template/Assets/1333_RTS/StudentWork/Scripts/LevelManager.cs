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
        DontDestroyOnLoad(gameObject);
        LoadScene(1);
    }

    public void LoadScene(int index)
    {
        _operation = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
        //_operation.allowSceneActivation = false;
    }

    private void LoadScreenActive()
    {
        _operation.allowSceneActivation = true;
    }
}
