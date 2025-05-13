using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using Zenject;
using UniRx;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainGameUIController : MonoInit
{
    [SerializeField] private GameObject mainMenuBtns;
    [FormerlySerializedAs("_settingsMenu")] [SerializeField] private GameObject settingsMenu;
    [FormerlySerializedAs("_turnOffUIParent")] [SerializeField] private GameObject turnOffUIParent;
    [SerializeField] private float fadeDuration = 0.5f;

    private CanvasGroup _turnOffUICanvasGroup;

    public readonly Subject<Unit> OnMenuTurnOn = new();
    public readonly Subject<Unit> OnMenuTurnOff = new();

    public readonly Subject<Unit> OnUITurnOn = new();
    public readonly Subject<Unit> OnUITurnOff = new();
    public readonly Subject<Unit> OnMainMenuLoad = new();
    public readonly Subject<Unit> OnOpenSaveMenuBtnClicked = new();

    private TutorialController _tutorialController;
    private LoadLevelController _loadLevelController;
    private MainGameController _mainGameController;
    private SettingsController _settingsController;
    private TimeController _timeController;

    [Inject]
    public void Construct(
        TutorialController tutorialController, LoadLevelController loadLevelController,
        MainGameController mainGameController, SettingsController settingsController,
        TimeController timeController)
    {
        _tutorialController = tutorialController;
        _loadLevelController = loadLevelController;
        _mainGameController = mainGameController;
        _settingsController = settingsController;
        _timeController = timeController;
    }


    public override async UniTask Init()
    {
        await base.Init();

        _tutorialController.OnTutorialStarted
            .Subscribe(_ => TurnOnUIForTutorial())
            .AddTo(this);

        _timeController.OnNextTurnBtnClickStarted
            .Subscribe(_ => TurnOffUI())
            .AddTo(this);

        _timeController.OnNextTurnBtnClickEnded
            .Subscribe(_ => TurnOnUI())
            .AddTo(this);

        _mainGameController.OnGameStartedFromLoad
            .Subscribe(_ => TurnOnUI())
            .AddTo(this);

        var saveLoadController = FindFirstObjectByType<SaveLoadController>();
        saveLoadController.OnCloseSaveMenuBtnClicked
            .Subscribe(_ => CloseSaveMenu())
            .AddTo(this);

        _turnOffUICanvasGroup = turnOffUIParent.GetComponent<CanvasGroup>() 
                                ?? turnOffUIParent.AddComponent<CanvasGroup>();

        // Устанавливаем начальное состояние UI
        SetCanvasGroupState(_turnOffUICanvasGroup, false);
        _turnOffUICanvasGroup.alpha = 0;
    }



    public void TurnOnMenu()
    {
        settingsMenu.SetActive(true);
        TurnOffUI();
        OnMenuTurnOn.OnNext(Unit.Default);
    }

    public void TurnOffMenu()
    {
        settingsMenu.SetActive(false);
        TurnOnUI();
        OnMenuTurnOff.OnNext(Unit.Default);
    }

    private void TurnOnUIForTutorial()
    {
        _turnOffUICanvasGroup.DOFade(1f, fadeDuration)
            .OnComplete(() => SetCanvasGroupState(_turnOffUICanvasGroup, false));
    }

    public void TurnOnUI()
    {
        Debug.Log("TurnOnUI");

        OnUITurnOn.OnNext(Unit.Default);

        _turnOffUICanvasGroup.DOFade(1f, fadeDuration)
            .OnComplete(() => SetCanvasGroupState(_turnOffUICanvasGroup, true));
    }

    public void TurnOffUI()
    {
        Debug.Log("TurnOffUI");

        if (_mainGameController.GameOverState == MainGameController.GameOverStateEnum.Playing)
            OnUITurnOff.OnNext(Unit.Default);

        _turnOffUICanvasGroup.DOFade(0f, fadeDuration)
            .OnComplete(() => SetCanvasGroupState(_turnOffUICanvasGroup, false));
    }

    public void LoadMainMenu()
    {
        OnMainMenuLoad.OnNext(Unit.Default);
        _loadLevelController?.LoadSceneAsync(Scenes.MAIN_MENU);
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

    private void SetCanvasGroupState(CanvasGroup canvasGroup, bool interactive)
    {
        canvasGroup.interactable = interactive;
        canvasGroup.blocksRaycasts = interactive;
    }
}
