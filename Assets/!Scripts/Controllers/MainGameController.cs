using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;


public class MainGameController : MonoBehaviour
{
    [Header("StartGame")]
    [SerializeField] private InfoPopUp _startPopUpInfo;
    [SerializeField] private GameObject _gameUI;
    [SerializeField] private Image _blackImage;

    [Range(0f, 3f)]
    [SerializeField] private float _blackoutTime;

    [Range(0.1f, 5f)]
    [SerializeField] private float animationDuration = 1f;

    [SerializeField] private AnimationType animationType;

    [Header("Outline")]
    [SerializeField] private Color OutlineColor;
    [Range(0.1f, 1f)]
    [SerializeField] private float OutlineWidth;

    [SerializeField] private float cameraCityShowY;
    [SerializeField] private float cameraCityHideY;

    public enum AnimationType
    {
        EaseInOut,
        EaseIn,
        EaseOut,
        Linear
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
        _gameUI.SetActive(false);
        ControllersManager.Instance.selectionController.enabled = false;

        _blackImage.color = Color.black;

        _blackImage.DOFade(0, _blackoutTime).OnComplete(() =>
        {
            _startPopUpInfo.ShowPopUp();
        });
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
            case AnimationType.EaseInOut:
                easeType = Ease.InOutQuad;
                break;
            case AnimationType.EaseIn:
                easeType = Ease.InQuad;
                break;
            case AnimationType.EaseOut:
                easeType = Ease.OutQuad;
                break;
            case AnimationType.Linear:
                easeType = Ease.Linear;
                break;
        }

        // Запуск анимации
        ControllersManager.Instance.MainCamera.transform.DOMoveY(targetYPosition, animationDuration).SetEase(easeType);
    }

    public int GetAnimDuration()
    {
        return (int)(animationDuration*1000);
    }
}
