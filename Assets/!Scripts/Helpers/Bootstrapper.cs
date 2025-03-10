using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    public static Bootstrapper Instance;

    public LoadLevelController loadLevelController { get; private set; }
    public GameController gameController { get; private set; }
    public SoundController SoundController { get; private set; }


    private async void Awake()
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
        SoundController = GetComponentInChildren<SoundController>();


        await loadLevelController.LoadSceneAsync(Scenes.MAIN_MENU);

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
