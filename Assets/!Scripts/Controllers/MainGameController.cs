using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using UniRx;
using Zenject;
using System;
using UnityEngine.UI;

public class MainGameController : MonoInit
{
    [SerializeField] private InfoPopUp startPopUp;
    [SerializeField] private InfoPopUp tutorialPopUp;
    [SerializeField] private Image blackImage;
    [SerializeField] private GameObject notificationsParent;

    public GameOverStateEnum GameOverState { get; private set; }

    public readonly Subject<Unit> OnNewGameStarted = new();
    public readonly Subject<Unit> OnGameStartedFromLoad = new();

    public enum GameOverStateEnum
    {
        Playing,
        WinBySendingArmyMaterials,
        StabilityLose,
        NoTimeLeftLose,
    }

    public MainGameControllerConfig _mainGameControllerConfig {  get; private set; }
    private Camera _mainCamera;
    private TimeController _timeController;
    private MainGameUIController _mainGameUIController;
    private BuildingsController _buildingsController;
    private ResourceModel _resourceModel;
    private EventController _eventController;


    [Inject]
    public void Construct(
        MainGameControllerConfig mainGameControllerConfig, Camera mainCamera,
        TimeController timeController, MainGameUIController mainGameUIController,
        BuildingsController buildingsController, ResourceModel resourceModel,
        EventController eventController
        )
    {
        _mainGameControllerConfig = mainGameControllerConfig;
        _mainCamera = mainCamera;
        _timeController = timeController;
        _mainGameUIController = mainGameUIController;
        _buildingsController = buildingsController;
        _resourceModel = resourceModel;
        _eventController = eventController;
    }

    public override async UniTask Init()
    {
        await base.Init();

        notificationsParent.GetComponent<CanvasGroup>().alpha = 0.0f;
        blackImage.color = Color.black;

        await blackImage
            .DOFade(0, _mainGameControllerConfig.BlackoutTime)
            .SetEase(Ease.InOutSine)
            .AsyncWaitForCompletion();

        _mainGameUIController.OnMenuTurnOn
            .Subscribe(_ => HideCity())
            .AddTo(this);

        _mainGameUIController.OnMenuTurnOff
            .Subscribe(_ => ShowCity())
            .AddTo(this);

        _buildingsController.GetCityHallBuilding().OnArmyMaterialsSentWin
            .Subscribe(SetGameOverState)
            .AddTo(this);

        _resourceModel.OnStabilityLose
            .Subscribe(SetGameOverState)
            .AddTo(this);

        _eventController.OnGameOverStarted
            .Subscribe(_ =>
            {
                //    blackImage
                //.DOFade(0, blackoutTime)
                //.SetEase(Ease.InOutSine);
                notificationsParent.SetActive(false);
            })
            .AddTo(this);

        if (_timeController.CurrentPeriod == TimeController.PeriodOfDay.Утро && _timeController.CurrentDate == new DateTime(1942, 10, 30))
        {
            OnNewGameStarted.OnNext(Unit.Default);
            startPopUp.ShowPopUp();
        }
        else
        {
            Destroy(startPopUp.gameObject);
            Destroy(tutorialPopUp.gameObject);
            ShowCity();
            OnGameStartedFromLoad.OnNext(Unit.Default);
        }

        await UniTask.Delay(5000);
        notificationsParent.GetComponent<CanvasGroup>().alpha = 1.0f;
    }

    private void ShowCity() => AnimateCamera(_mainGameControllerConfig.CameraCityShowY);

    private void HideCity() => AnimateCamera(_mainGameControllerConfig.CameraCityHideY);

    private void AnimateCamera(float targetYPosition)
    {
        _mainCamera.transform
            .DOLocalMoveY(targetYPosition, _mainGameControllerConfig.ShowCityDuration)
            .SetEase(_mainGameControllerConfig.EaseType);
    }

    public void SetGameOverState(GameOverStateEnum state)
    {
        GameOverState = state;
    }
}