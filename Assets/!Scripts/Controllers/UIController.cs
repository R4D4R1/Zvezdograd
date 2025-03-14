using UnityEditor;
using UnityEngine;
using Zenject;

public class UIController : MonoBehaviour
{

    [Inject] private LoadLevelController _loadLevelController;

    public async void StartNewGame()
    {
        //SaveLoadManager.SetStartedNewGame();

        await _loadLevelController.LoadSceneAsync(Scenes.GAME_SCENE);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}
