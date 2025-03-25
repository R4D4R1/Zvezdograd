using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;
using UnityEngine.Serialization;
using Zenject;


public class MainGameController : MonoInit
{
    [FormerlySerializedAs("_startPopUp")] [SerializeField] private InfoPopUp startPopUp;
    [FormerlySerializedAs("_tutorialPupUp")] [SerializeField] private InfoPopUp tutorialPupUp;
    [FormerlySerializedAs("_blackImage")] [SerializeField] private Image blackImage;

    [FormerlySerializedAs("_blackoutTime")]
    [Range(0f, 3f)]
    [SerializeField] private float blackoutTime;

    [FormerlySerializedAs("_showCityDuration")]
    [Range(0.1f, 5f)]
    [SerializeField] private float showCityDuration = 1f;

    [FormerlySerializedAs("_cameraCityShowY")] [SerializeField] private float cameraCityShowY;
    [FormerlySerializedAs("_cameraCityHideY")] [SerializeField] private float cameraCityHideY;
    [FormerlySerializedAs("_easeType")] [SerializeField] private Ease easeType;
    public GameOverStateEnum GameOverState { get; private set; }

    public readonly Subject<Unit> OnGameStarted = new();

    public enum GameOverStateEnum
    {
        Playing,
        Win,
        Lose
    }
    
    private BuildingController _buildingController;
    private Camera _mainCamera;

    [Inject]
    public void Construct(BuildingController buildingController,Camera mainCamera)
    {
        _buildingController = buildingController;
        _mainCamera = mainCamera;
    }

    public override void Init()
    {
        base.Init();
        foreach (var building in _buildingController.AllBuildings)
        {
            building.Init();
        }

        OnGameStarted.OnNext(Unit.Default);

        blackImage.color = Color.black;

        blackImage.DOFade(0, blackoutTime).OnComplete(() =>
        {
            startPopUp.ShowPopUp();
        });
    }

    public void GameWin()
    {
        Debug.Log("WIN");
        GameOverState = GameOverStateEnum.Win;
    }

    public void GameLost()
    {
        Debug.Log("LOSE");
        GameOverState = GameOverStateEnum.Lose;
    }

    public void ShowCity()
    {
        AnimateCamera(cameraCityShowY);
    }

    public void HideCity()
    {
        AnimateCamera(cameraCityHideY);
    }

    private void AnimateCamera(float targetYPosition)
    {
        _mainCamera.transform.DOLocalMoveY(targetYPosition, showCityDuration).SetEase(easeType);
    }
}
