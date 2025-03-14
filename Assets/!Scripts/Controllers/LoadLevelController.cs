using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class LoadLevelController : MonoBehaviour
{
    [SerializeField] private GameObject _loaderCanvas;
    [SerializeField] private Slider _progressBar;

    private float _target;
    private GameController _gameController;

    [Inject]
    public void Construct(GameController gameController)
    {
        _gameController = gameController;
    }

    public async UniTask LoadSceneAsync(string sceneName)
    {
        _target = 0;
        _progressBar.value = 0;

        await UniTask.Delay((int)(_gameController.GameStartDelay * 1000));

        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        _loaderCanvas.SetActive(true);

        // Загрузка сцены с прогрессом
        while (scene.progress < 0.9f)
        {
            _target = Mathf.Clamp01(scene.progress / 0.9f);
            _progressBar.value = _target;
            await UniTask.Delay(100); // ожидание на следующее обновление
        }

        _target = 1;
        _progressBar.value = _target;

        await UniTask.Delay((int)(_gameController.GameAfterLoadDelay * 1000));

        scene.allowSceneActivation = true;

        await UniTask.Yield();
        _loaderCanvas.SetActive(false);
    }

    private void Update()
    {
        _progressBar.value = Mathf.MoveTowards(_progressBar.value, _target, 3 * Time.deltaTime);
    }
}
