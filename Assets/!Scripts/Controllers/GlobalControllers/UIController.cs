using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Zenject;
using UniRx;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuBtns;

    public readonly Subject<Unit> OnOpenSaveMenuBtnClicked = new();

    [Inject] private LoadLevelController _loadLevelController;
    [Inject] private SettingsController _settingsController;
    [Inject] private SaveLoadController _saveLoadController;

    private void Start()
    {
        _saveLoadController.OnCloseSaveMenuBtnClicked
            .Subscribe(_ => CloseSaveMenu())
            .AddTo(this);
    }

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

    public void OpenSaveMenu()
    {
        mainMenuBtns.SetActive(false);
        OnOpenSaveMenuBtnClicked.OnNext(Unit.Default);
    }

    private void CloseSaveMenu()
    {
        mainMenuBtns.SetActive(true);
    }

    public void LoadLastSave()
    {
        _saveLoadController.LoadLastSave();
    }
}
