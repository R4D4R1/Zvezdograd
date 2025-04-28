using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;
using Zenject;
using System;

public class MainGameController : MonoInit
{
    [SerializeField] private InfoPopUp startPopUp;
    [SerializeField] private InfoPopUp tutorialPopUp;
    [SerializeField] private Image blackImage;
    [SerializeField] private GameObject notificationsParent;

    [HideInInspector]
    public GameOverStateEnum GameOverState;

    public readonly Subject<Unit> OnGameStarted = new();

    public enum GameOverStateEnum
    {
        Playing,
        Win,
        StabilityLose,
        NoTimeLeftLose,
    }

    private Camera _mainCamera;
    public MainGameControllerConfig MainGameControllerConfig { get; private set; }

    [Inject]
    public void Construct(Camera mainCamera, MainGameControllerConfig mainGameControllerConfig)
    {
        _mainCamera = mainCamera;
        MainGameControllerConfig = mainGameControllerConfig;
    }

    [Inject] private TimeController _timeController;
    [Inject] private MainGameUIController _mainGameUIController;

    public override async UniTask Init()
    {
        await base.Init();

        notificationsParent.GetComponent<CanvasGroup>().alpha = 0.0f;
        OnGameStarted.OnNext(Unit.Default);

        blackImage.color = Color.black;

        await blackImage
            .DOFade(0, MainGameControllerConfig.BlackoutTime)
            .SetEase(Ease.Linear)
            .AsyncWaitForCompletion();

        if (_timeController.CurrentPeriod == TimeController.PeriodOfDay.Morning && _timeController.CurrentDate == new DateTime(1942, 10, 30))
            startPopUp.ShowPopUp();
        else
        {
            Destroy(startPopUp.gameObject);
            Destroy(tutorialPopUp.gameObject);
            ShowCity();
            _mainGameUIController.TurnOnUI();
            _timeController.EnableNextTurnLogic();
        }

        await UniTask.Delay(5000);
        notificationsParent.GetComponent<CanvasGroup>().alpha = 1.0f;
    }

    public void ShowCity() => AnimateCamera(MainGameControllerConfig.CameraCityShowY);

    public void HideCity() => AnimateCamera(MainGameControllerConfig.CameraCityHideY);

    private void AnimateCamera(float targetYPosition)
    {
        _mainCamera.transform
            .DOLocalMoveY(targetYPosition, MainGameControllerConfig.ShowCityDuration)
            .SetEase(MainGameControllerConfig.EaseType);
    }
}