using UnityEditor;
using UnityEngine;
using Zenject;

public class UIController : MonoBehaviour
{

    [Inject] private LoadLevelController _loadLevelController;
    [Inject] private SettingsController _settingsController;

    // ReSharper disable once AsyncVoidMethod
    public async void StartNewGame()
    {
        await _loadLevelController.LoadSceneAsync(Scenes.GAME_SCENE);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

    public void OpenSettingsMenu()
    {
        _settingsController.Activate();
    }
    
    public void CloseSettingsMenu()
    {
        _settingsController.Deactivate();
    }
}
