using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    public static Bootstrapper Instance;

    public LoadLevelController loadLevelController { get; private set; }
    public GameController gameController { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        loadLevelController = GetComponentInChildren<LoadLevelController>();
        gameController = GetComponentInChildren<GameController>();

        loadLevelController.LoadSceneAsync(Scenes.MAIN_MENU);

        if (Application.platform == RuntimePlatform.Android)
        {
            Application.targetFrameRate = 60;
        }
        else
        {
            Application.targetFrameRate = -1; // -1 means no limit
        }
    }
}
