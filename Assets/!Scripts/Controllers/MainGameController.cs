using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;
using Zenject;


public class MainGameController : MonoBehaviour
{
    [SerializeField] private InfoPopUp _startPopUp;
    [SerializeField] private InfoPopUp _tutorialPupUp;
    [SerializeField] private Image _blackImage;

    [Range(0f, 3f)]
    [SerializeField] private float _blackoutTime;

    [Range(0.1f, 5f)]
    [SerializeField] private float _showCityDuration = 1f;

    [SerializeField] private float _cameraCityShowY;
    [SerializeField] private float _cameraCityHideY;
    [SerializeField] private Ease _easeType;
    public GameOverStateEnum GameOverState { get; private set; }

    public readonly Subject<Unit> OnGameStarted = new();

    public enum GameOverStateEnum
    {
        Playing,
        Win,
        Lose
    }

    protected ControllersManager _controllersManager;
    protected ResourceViewModel _resourceViewModel;
    protected Camera _mainCamera;

    [Inject]
    public void Construct(ControllersManager controllersManager, ResourceViewModel resourceViewModel,Camera mainCamera)
    {
        _controllersManager = controllersManager;
        _resourceViewModel = resourceViewModel;
        _mainCamera = mainCamera;
    }

    public void Init()
    {

        foreach (SelectableBuilding building in _controllersManager.BuildingController.AllBuildings)
        {
            building.Init();
        }

        OnGameStarted.OnNext(Unit.Default);

        _blackImage.color = Color.black;

        _blackImage.DOFade(0, _blackoutTime).OnComplete(() =>
        {
            _startPopUp.ShowPopUp();
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
        AnimateCamera(_cameraCityShowY);
    }

    public void HideCity()
    {
        AnimateCamera(_cameraCityHideY);
    }

    private void AnimateCamera(float targetYPosition)
    {
        _mainCamera.transform.DOLocalMoveY(targetYPosition, _showCityDuration).SetEase(_easeType);
    }

    public int GetAnimDuration()
    {
        return (int)(_showCityDuration*1000);
    }
}
