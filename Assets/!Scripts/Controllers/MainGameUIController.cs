using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using Zenject;
using UniRx;
using UnityEngine.Serialization;

public class MainGameUIController : MonoInit
{
    [FormerlySerializedAs("_settingsMenu")] [SerializeField] private GameObject settingsMenu;
    [FormerlySerializedAs("_turnOffUIParent")] [SerializeField] private GameObject turnOffUIParent;
    [SerializeField] private float fadeDuration = 0.5f;

    private CanvasGroup _turnOffUICanvasGroup;
    private InfoPopUp _popUpToClose;

    public readonly Subject<Unit> OnUITurnOn = new();
    public readonly Subject<Unit> OnUITurnOff = new();
    public readonly Subject<Unit> OnMainMenuLoad = new();

    private TutorialController _tutorialController;
    private LoadLevelController _loadLevelController;
    private MainGameController _mainGameController;
    private SettingsController _settingsController;

    [Inject]
    public void Construct(
        TutorialController tutorialController,
        LoadLevelController loadLevelController,
        MainGameController mainGameController,
        SettingsController settingsController)
    {
        _tutorialController = tutorialController;
        _loadLevelController = loadLevelController;
        _mainGameController = mainGameController;
        _settingsController = settingsController;
    }

    public override UniTask Init()
    {
        base.Init();

        _tutorialController.OnTutorialStarted
            .Subscribe(_ => TurnOnUIForTutorial())
            .AddTo(this);

        _turnOffUICanvasGroup = turnOffUIParent.GetComponent<CanvasGroup>() 
                                ?? turnOffUIParent.AddComponent<CanvasGroup>();

        // Устанавливаем начальное состояние UI
        SetCanvasGroupState(_turnOffUICanvasGroup, false);
        _turnOffUICanvasGroup.alpha = 0;
        return UniTask.CompletedTask;
    }

    public void TurnOnMenu()
    {
        settingsMenu.SetActive(true);
        TurnOffUI();
        _mainGameController.HideCity();
    }

    public void TurnOffMenu()
    {
        settingsMenu.SetActive(false);
        _mainGameController.ShowCity();
        TurnOnUI();
    }

    private void TurnOnUIForTutorial()
    {
        _turnOffUICanvasGroup.DOFade(1f, fadeDuration)
            .OnComplete(() => SetCanvasGroupState(_turnOffUICanvasGroup, false));
    }

    public void TurnOnUI()
    {
        OnUITurnOn.OnNext(Unit.Default);

        _turnOffUICanvasGroup.DOFade(1f, fadeDuration)
            .OnComplete(() => SetCanvasGroupState(_turnOffUICanvasGroup, true));
    }

    public void TurnOffUI()
    {
        if(_mainGameController.GameOverState == MainGameController.GameOverStateEnum.Playing)
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

    private void SetCanvasGroupState(CanvasGroup canvasGroup, bool interactive)
    {
        canvasGroup.interactable = interactive;
        canvasGroup.blocksRaycasts = interactive;
    }
}
