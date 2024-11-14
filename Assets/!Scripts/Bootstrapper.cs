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
            Destroy(gameObject);

        loadLevelController = GetComponentInChildren<LoadLevelController>();
        gameController = GetComponentInChildren<GameController>();

        loadLevelController.LoadSceneAsync(Scenes.MAIN_MENU);
    }
}
