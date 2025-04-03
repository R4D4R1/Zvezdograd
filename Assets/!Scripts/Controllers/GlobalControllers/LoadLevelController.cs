using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

public class LoadLevelController : MonoBehaviour
{
    [FormerlySerializedAs("_loaderCanvas")] [SerializeField] private GameObject loaderCanvas;
    [FormerlySerializedAs("_progressBar")] [SerializeField] private Slider progressBar;
    
    [SerializeField] private bool Test;

    private void Start()
    {
        if (Test)
        {
            Destroy(gameObject);
        }
    }

    public async UniTask LoadSceneAsync(string sceneName)
    {
        progressBar.value = 0;
        loaderCanvas.SetActive(true);

        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        while (scene.progress < 0.9f)
        {
            float target = Mathf.Clamp01(scene.progress / 0.9f);
            Tween tween = progressBar.DOValue(target, 0.25f).SetEase(Ease.Linear);
            await UniTask.WaitUntil(() => !tween.IsActive()); 
        }

        Tween finalTween = progressBar.DOValue(1, 0.25f).SetEase(Ease.Linear);
        await UniTask.WaitUntil(() => !finalTween.IsActive());

        scene.allowSceneActivation = true;
        await UniTask.Yield();

        loaderCanvas.SetActive(false);
    }
}