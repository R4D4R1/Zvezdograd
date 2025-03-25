using System;
using Cysharp.Threading.Tasks;
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

    private float _target;
    
    private GameController _gameController;

    [Inject]
    public void Construct(GameController gameController)
    {
        _gameController = gameController;
    }

    private void Start()
    {
        if (Test)
        {
            Destroy(gameObject);
        }
    }

    public async UniTask LoadSceneAsync(string sceneName)
    {
        _target = 0;
        progressBar.value = 0;

        await UniTask.Delay((int)(_gameController.GameStartDelay * 1000));

        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        loaderCanvas.SetActive(true);

        while (scene.progress < 0.9f)
        {
            _target = Mathf.Clamp01(scene.progress / 0.9f);
            progressBar.value = _target;
            await UniTask.Delay(100);
        }

        _target = 1;
        progressBar.value = _target;

        await UniTask.Delay((int)(_gameController.GameAfterLoadDelay * 1000));

        scene.allowSceneActivation = true;

        await UniTask.Yield();
        loaderCanvas.SetActive(false);
    }

    private void Update()
    {
        Debug.Log("TEST");
        progressBar.value = Mathf.MoveTowards(progressBar.value, _target, 3 * Time.deltaTime);
    }
}
