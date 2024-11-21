using UnityEditor;
using UnityEngine;

public class UIController : MonoBehaviour
{
    private Bootstrapper entryPoint = Bootstrapper.Instance;

    public void LoadStartScene()
    {
        entryPoint.loadLevelController.LoadSceneAsync(Scenes.GAME_SCENE);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}
