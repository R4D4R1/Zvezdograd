using UnityEngine;
using Zenject;
using System.Threading.Tasks;

public class Bootstrapper : MonoBehaviour
{
    private LoadLevelController _loadLevelController;

    [Inject]
    private void Construct(LoadLevelController loadLevelController)
    {
        _loadLevelController = loadLevelController;
    }

    private async void Awake()
    {
        await _loadLevelController.LoadSceneAsync(Scenes.MAIN_MENU);

        Application.targetFrameRate = (Application.platform == RuntimePlatform.Android) ? 60 : -1;
    }
}
