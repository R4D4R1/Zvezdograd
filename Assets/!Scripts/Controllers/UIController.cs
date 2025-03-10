using UnityEditor;
using UnityEngine;

public class UIController : MonoBehaviour
{
    private Bootstrapper _entryPoint = Bootstrapper.Instance;

    public async void StartNewGame()
    {
        SaveLoadManager.SetStartedNewGame();
        await _entryPoint.loadLevelController.LoadSceneAsync(Scenes.GAME_SCENE);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}
