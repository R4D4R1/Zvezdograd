using UnityEngine;
using DG.Tweening;
using Zenject;
using System;
using UniRx;

public class MainGameUIController : MonoBehaviour
{ 
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private GameObject _turnOffUIParent;
    [SerializeField] private float fadeDuration = 0.5f;

    private CanvasGroup _turnOffUICanvasGroup;
    private InfoPopUp _popUpToClose;

    protected ControllersManager _controllersManager;
    private LoadLevelController _loadLevelController;

    public readonly Subject<Unit> OnUITurnOn = new();
    public readonly Subject<Unit> OnUITurnOff = new();

    [Inject]
    public void Construct(ControllersManager controllersManager, LoadLevelController loadLevelController)
    {
        _controllersManager = controllersManager;
        _loadLevelController = loadLevelController;
    }

    public void Init()
    {
        _controllersManager.TutorialController.OnTutorialStarted
           .Subscribe(_ => TurnOnUIForTutorial())
           .AddTo(this);

        _turnOffUICanvasGroup = _turnOffUIParent.GetComponent<CanvasGroup>();

        if (_turnOffUICanvasGroup == null)
        {
            _turnOffUICanvasGroup = _turnOffUIParent.AddComponent<CanvasGroup>();
        }

        _turnOffUICanvasGroup.alpha = 0;
        _turnOffUICanvasGroup.interactable = false;
        _turnOffUICanvasGroup.blocksRaycasts = false;
    }

    public void TurnOnMenu()
    {
        _settingsMenu.SetActive(true);
        TurnOffUI();
        _controllersManager.MainGameController.HideCity();
    }

    public void TurnOffMenu()
    {
        _settingsMenu.SetActive(false);
        _controllersManager.MainGameController.ShowCity();
        TurnOnUI();
    }

    public void TurnOnUIForTutorial()
    {
        _turnOffUICanvasGroup.DOFade(1f, fadeDuration);
    }

    public void TurnOnUI()
    {
        OnUITurnOn.OnNext(Unit.Default);

        _turnOffUICanvasGroup.DOFade(1f, fadeDuration).OnComplete(() => {
            _turnOffUICanvasGroup.interactable = true;
            _turnOffUICanvasGroup.blocksRaycasts = true;
        });
    }

    public void TurnOffUI()
    {
        OnUITurnOff.OnNext(Unit.Default);

        _turnOffUICanvasGroup.DOFade(0f, fadeDuration).OnComplete(() => {
            _turnOffUICanvasGroup.interactable = false;
            _turnOffUICanvasGroup.blocksRaycasts = false;
        });
    }

    public void LoadMainMenu()
    {
        _loadLevelController?.LoadSceneAsync(Scenes.MAIN_MENU);
    }
}
