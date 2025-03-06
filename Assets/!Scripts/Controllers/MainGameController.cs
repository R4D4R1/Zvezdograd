using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;


public class MainGameController : MonoBehaviour
{
    [SerializeField] private InfoPopUp _startPopUpInfo;
    [SerializeField] private Image _blackImage;

    [Range(0f, 3f)]
    [SerializeField] private float _blackoutTime;

    [Range(0.1f, 5f)]
    [SerializeField] private float animationDuration = 1f;

    [SerializeField] private AnimationTypeEnum animationType;
    public GameOverStateEnum GameOverState {  get; private set; }

    [SerializeField] private float cameraCityShowY;
    [SerializeField] private float cameraCityHideY;

    [Header("Outline")]
    [SerializeField] private Color OutlineColor;
    [Range(0.1f, 1f)]
    [SerializeField] private float OutlineWidth;

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

    private async void Start()
    {
        Outline[] outlines = Object.FindObjectsByType<Outline>(FindObjectsSortMode.None);

        foreach (Outline outline in outlines)
        {
            outline.enabled = true;
        }

        await UniTask.Delay(0);

        foreach (Outline outline in outlines)
        {
            outline.OutlineColor = OutlineColor;
            outline.OutlineWidth = OutlineWidth;
            outline.enabled = false;
        }

        foreach (RepairableBuilding building in ControllersManager.Instance.buildingController.SpecialBuildings)
        {
            building.InitBuilding();
        }

        foreach (RepairableBuilding building in ControllersManager.Instance.buildingController.RegularBuildings)
        {
            building.InitBuilding();
        }

        ControllersManager.Instance.blurController.BlurBackGroundNow();
        ControllersManager.Instance.selectionController.enabled = false;

        _blackImage.color = Color.black;

        _blackImage.DOFade(0, _blackoutTime).OnComplete(() =>
        {
            _startPopUpInfo.ShowPopUp();
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
        // ����� ���� ��������
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

        // ������ ��������
        ControllersManager.Instance.MainCamera.transform.DOLocalMoveY(targetYPosition, animationDuration).SetEase(easeType);
    }

    public int GetAnimDuration()
    {
        return (int)(animationDuration*1000);
    }
}
