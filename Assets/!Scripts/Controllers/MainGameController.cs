using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;
using Zenject;

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
    public void Construct(Camera mainCamera,MainGameControllerConfig mainGameControllerConfig)
    {
        _mainCamera = mainCamera;
        MainGameControllerConfig = mainGameControllerConfig;
    }

    public override void Init()
    {
        base.Init();

        notificationsParent.SetActive(false);
        OnGameStarted.OnNext(Unit.Default);

        blackImage.color = Color.black;
        blackImage.DOFade(0, MainGameControllerConfig.BlackoutTime).OnComplete(() =>
        {
            startPopUp.ShowPopUp();
            notificationsParent.SetActive(true); // Показ после попапа
        });
    }

    public void ShowCity()
    {
        AnimateCamera(MainGameControllerConfig.CameraCityShowY);
    }

    public void HideCity()
    {
        AnimateCamera(MainGameControllerConfig.CameraCityHideY);
    }

    private void AnimateCamera(float targetYPosition)
    {
        _mainCamera.transform
            .DOLocalMoveY(targetYPosition, MainGameControllerConfig.ShowCityDuration)
            .SetEase(MainGameControllerConfig.EaseType);
    }
}