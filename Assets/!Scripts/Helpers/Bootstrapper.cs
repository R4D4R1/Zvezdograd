using UnityEngine;
using Zenject;

public class Bootstrapper : MonoBehaviour
{
    private LoadLevelController _loadLevelController;

    [Inject]
    private void Construct(LoadLevelController loadLevelController)
    {
        _loadLevelController = loadLevelController;
    }

    // ReSharper disable once AsyncVoidMethod
    private async void Start()
    {
        if(Application.platform == RuntimePlatform.Android)
        {
            Application.targetFrameRate = 60;
        }
        else
        {
            Application.targetFrameRate = -1;
        }

        await _loadLevelController.LoadSceneAsync(Scenes.MAIN_MENU);
    }
}
