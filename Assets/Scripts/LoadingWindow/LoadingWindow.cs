using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingWindow : MonoBehaviour
{
    private static LoadingWindow instance;
    private static bool shouldPlayOpeningAnimation = false;
    private Animator _animator;
    private Canvas _canvas;

    [SerializeField] private TextMeshProUGUI _percents;
    [SerializeField] private Image _progressImage;

    private AsyncOperation loadingSceneOperation;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        _animator = GetComponent<Animator>();
        _canvas = GetComponent<Canvas>();

        SceneManager.sceneLoaded += (_, __) => { OnSceneLoaded(); } ;
    }
    private void Update()
    {
        if(loadingSceneOperation != null)
        {
            _progressImage.fillAmount = loadingSceneOperation.progress;
            _percents.text = ((int)Math.Round(loadingSceneOperation.progress * 100)).ToString() + "%";
        }
    }
    private void OnSceneLoaded()
    {
        if (shouldPlayOpeningAnimation && _animator != null)
        {
            _animator.SetTrigger("LoadingEnd");
            Debug.Log("Degub");
        }
        _percents.text = "0%";
    }

    public static void SwitchScene(string nameScene)
    {
        instance._animator.SetTrigger("LoadingStart");
        instance._canvas.sortingOrder = 1000;
        Time.timeScale = 0;

        instance.loadingSceneOperation = SceneManager.LoadSceneAsync(nameScene);
        instance.loadingSceneOperation.allowSceneActivation = false;

    }
    public static void SwitchScene(int indexScene)
    {
        instance._animator.SetTrigger("LoadingStart");
        instance._canvas.sortingOrder = 1000;
        Time.timeScale = 0;

        instance.loadingSceneOperation = SceneManager.LoadSceneAsync(indexScene);
        instance.loadingSceneOperation.allowSceneActivation = false;

    }

    public void OnLoadStartingOver()
    {
        Time.timeScale = 1f;
        loadingSceneOperation.allowSceneActivation = true;
        shouldPlayOpeningAnimation = true;
        
    }
    public void OnLoadEndingOver()
    {
        instance._canvas.sortingOrder = -1000;
    }
}
