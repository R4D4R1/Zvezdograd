using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;


public class MainGameController : MonoBehaviour
{
    [SerializeField] private InfoPopUp _startPopUp;
    [SerializeField] private InfoPopUp _tutorialPupUp;
    [SerializeField] private Image _blackImage;

    [Range(0f, 3f)]
    [SerializeField] private float _blackoutTime;

    [Range(0.1f, 5f)]
    [SerializeField] private float animationDuration = 1f;

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

    private void Start()
    {
        foreach (RepairableBuilding building in ControllersManager.Instance.buildingController.RepairableBuildings)
        {
            building.InitBuilding();
        }

        if (SaveLoadManager.IsStartedFromMainMenu)
        {
            _startPopUp.gameObject.SetActive(false);
            _tutorialPupUp.gameObject.SetActive(false);

            ShowCity();
            ControllersManager.Instance.mainGameUIController.TurnOnUI();

            SaveLoadManager.LoadDataFromCurrentSlot();
        }
        else
        {
            ControllersManager.Instance.blurController.BlurBackGroundNow();
            ControllersManager.Instance.selectionController.enabled = false;

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
        ControllersManager.Instance.MainCamera.transform.DOLocalMoveY(targetYPosition, animationDuration).SetEase(easeType);
    }

    public int GetAnimDuration()
    {
        return (int)(animationDuration*1000);
    }
}
