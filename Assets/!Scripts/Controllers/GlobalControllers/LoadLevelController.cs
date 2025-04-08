using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadLevelController : MonoBehaviour
{
    [SerializeField] private GameObject loaderCanvas;
    [SerializeField] private Slider progressBar;
    
    [SerializeField] private bool testMode;
    
    public readonly Subject<Unit> OnMainMenuSceneLoaded = new();
    public readonly Subject<Unit> OnSceneChanged = new();
    
    private void Start()
    {
        if (testMode)
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
            float targetProgress = Mathf.Clamp01(scene.progress / 0.9f);
            await progressBar.DOValue(targetProgress, 0.25f)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion();
        }

        await progressBar.DOValue(1, 0.25f)
            .SetEase(Ease.Linear)
            .AsyncWaitForCompletion();
        
        OnSceneChanged.OnNext(Unit.Default);

        if (sceneName.Equals(Scenes.MAIN_MENU))
        {
            OnMainMenuSceneLoaded.OnNext(Unit.Default);
        }

        scene.allowSceneActivation = true;
        await UniTask.Yield();

        loaderCanvas.SetActive(false);
    }
}