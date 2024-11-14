using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadLevelController : MonoBehaviour
{
    [SerializeField] private GameObject _loaderCanvas;
    [SerializeField] private Slider _progressBar;

    private float _target;

    // ����������� �������� �����
    public async void LoadSceneAsync(string sceneName)
    {
        var bootstrapper = Bootstrapper.Instance; // ��������� ���������� Bootstrapper

        _target = 0;
        _progressBar.value = 0;

        // �������� ����� ������� ��������
        await UniTask.Delay((int)(bootstrapper.gameController.GameStartDelay * 1000));

        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false; // ���������� �������������� ��������� �����

        _loaderCanvas.SetActive(true); // ��������� UI-������� ��������

        do
        {
            await UniTask.Delay(100); // �������� ��� ���������� ���������

            // ���������� �������� �������� ���������
            _target = Mathf.Clamp01(scene.progress / 0.9f);

        } while (scene.progress < 0.9f); // �������� �������� �� 90%

        _target = 1; // ��������� �������� �������� ��������� � 100%

        // �������� ����� ���������� ��������
        await UniTask.Delay((int)(bootstrapper.gameController.GameAfterLoadDelay * 1000));

        scene.allowSceneActivation = true; // ���������� ��������� �����

        await UniTask.Yield();
        _loaderCanvas.SetActive(false); // ���������� UI-������� ��������
    }

    private void Update()
    {
        // ������� ���������� �������� ��������� �� ��������
        _progressBar.value = Mathf.MoveTowards(_progressBar.value, _target, 3 * Time.deltaTime);
    }
}
