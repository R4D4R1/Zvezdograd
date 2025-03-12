using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
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

    [SerializeField] private AnimationTypeEnum animationType;
    public GameOverStateEnum GameOverState {  get; private set; }

    [SerializeField] private float cameraCityShowY;
    [SerializeField] private float cameraCityHideY;

    public enum AnimationTypeEnum
    {
        EaseInOut,
        EaseIn,
        EaseOut,
        Linear
    }

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

    private void Start()
    {
        foreach (RepairableBuilding building in _controllersManager.BuildingController.RepairableBuildings)
        {
            building.InitBuilding();
        }

        if (SaveLoadManager.IsStartedFromMainMenu)
        {
            _blackImage.DOFade(0, _blackoutTime).OnComplete(() =>
            {
                _startPopUp.gameObject.SetActive(false);
                _tutorialPupUp.gameObject.SetActive(false);

                ShowCity();
                _controllersManager.MainGameUIController.TurnOnUI();

                SaveLoadManager.LoadDataFromCurrentSlot();
            });
        }

        else
        {
            _controllersManager.BlurController.BlurBackGroundNow();
            _controllersManager.SelectionController.enabled = false;

            _blackImage.color = Color.black;

            _blackImage.DOFade(0, _blackoutTime).OnComplete(() =>
            {
                _startPopUp.ShowPopUp();
            });
        }
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
        // Выбор типа анимации
        Ease easeType = Ease.Linear;
        switch (animationType)
        {
            case AnimationTypeEnum.EaseInOut:
                easeType = Ease.InOutQuad;
                break;
            case AnimationTypeEnum.EaseIn:
                easeType = Ease.InQuad;
                break;
            case AnimationTypeEnum.EaseOut:
                easeType = Ease.OutQuad;
                break;
            case AnimationTypeEnum.Linear:
                easeType = Ease.Linear;
                break;
        }

        // Запуск анимации
        _mainCamera.transform.DOLocalMoveY(targetYPosition, _showCityDuration).SetEase(easeType);
    }

    public int GetAnimDuration()
    {
        return (int)(_showCityDuration*1000);
    }
}
