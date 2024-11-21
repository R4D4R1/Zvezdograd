using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadLevelController : MonoBehaviour
{
    [SerializeField] private GameObject _loaderCanvas;
    [SerializeField] private Slider _progressBar;

    private float _target;

    // Асинхронная загрузка сцены
    public async void LoadSceneAsync(string sceneName)
    {
        var bootstrapper = Bootstrapper.Instance; // Получение экземпляра Bootstrapper

        _target = 0;
        _progressBar.value = 0;

        // Ожидание перед началом загрузки
        await UniTask.Delay((int)(bootstrapper.gameController.GameStartDelay * 1000));

        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false; // Отключение автоматической активации сцены

        _loaderCanvas.SetActive(true); // Активация UI-канваса загрузки

        do
        {
            await UniTask.Delay(100); // Ожидание для обновления прогресса

            // Обновление целевого значения прогресса
            _target = Mathf.Clamp01(scene.progress / 0.9f);

        } while (scene.progress < 0.9f); // Ожидание загрузки до 90%

        _target = 1; // Установка целевого значения прогресса в 100%

        // Ожидание после завершения загрузки
        await UniTask.Delay((int)(bootstrapper.gameController.GameAfterLoadDelay * 1000));

        scene.allowSceneActivation = true; // Разрешение активации сцены

        await UniTask.Yield();
        _loaderCanvas.SetActive(false); // Отключение UI-канваса загрузки
    }

    private void Update()
    {
        // Плавное обновление значения прогресса на слайдере
        _progressBar.value = Mathf.MoveTowards(_progressBar.value, _target, 3 * Time.deltaTime);
    }
}
