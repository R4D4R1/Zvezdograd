using UnityEngine;
using TMPro;
using DG.Tweening;
using Zenject;
using UniRx;

public class InfoPopUp : MonoBehaviour
{
    [Header("INFO POPUP SETTINGS")]
    public TextMeshProUGUI LabelText;
    public TextMeshProUGUI DescriptionText;
    [SerializeField] private CanvasGroup alphaCanvas;

    public bool IsActive { get; protected set; }

    protected const float SCALE_DURATION = 0.25f;
    private const float FADE_DURATION = 0.25f;

    public readonly Subject<Unit> OnPopUpHide = new();


    // Dependencies
    protected ResourceViewModel _resourceViewModel;
    protected PopUpFactory _popUpFactory;
    protected PeopleUnitsController _peopleUnitsController;
    protected TimeController _timeController;
    protected BuildingsController _buildingsController;
    protected MainGameController _mainGameController;
    protected MainGameUIController _mainGameUIController;
    protected TutorialController _tutorialController;
    protected PopUpsController _popUpsController;
    protected EventController _eventController;

    [Inject]
    public void Construct(
        ResourceViewModel resourceViewModel,
        PopUpFactory popUpFactory,
        PeopleUnitsController peopleUnitsController,
        TimeController timeController,
        BuildingsController buildingsController,
        MainGameController mainGameController,
        MainGameUIController mainGameUIController,
        TutorialController tutorialController,
        PopUpsController popUpsController,
        EventController eventController)
    {
        _resourceViewModel = resourceViewModel;
        _popUpFactory = popUpFactory;
        _peopleUnitsController = peopleUnitsController;
        _timeController = timeController;
        _buildingsController = buildingsController;
        _mainGameController = mainGameController;
        _mainGameUIController = mainGameUIController;
        _tutorialController = tutorialController;
        _popUpsController = popUpsController;
        _eventController = eventController;
    }

    private void OnEnable()
    {
        IsActive = false;
        transform.localScale = Vector3.zero;
        SetAlpha(0);
    }

    public virtual void ShowPopUp()
    {
        transform.DOScale(Vector3.one, SCALE_DURATION).OnComplete(() =>
        {
            IsActive = true;
            SetAlpha(1);
        });
    }

    public void ShowPopUp(string label, string description)
    {
        IsActive = true;

        LabelText.text = "";
        DescriptionText.text = "";

        transform.DOScale(Vector3.one, SCALE_DURATION).OnComplete(() =>
        {
            LabelText.text = label;
            DescriptionText.text = description;
            SetAlpha(1);
        });
    }

    public virtual void HidePopUp()
    {
        if (!IsActive) return;

        transform.DOScale(Vector3.zero, SCALE_DURATION).OnComplete(() =>
        {
            IsActive = false;
        });

        SetAlpha(0);
        //OnPopUpHide.OnNext(Unit.Default);
        _mainGameUIController.TurnOnUI();
    }

    public void HideStartInfoPopUpPopUp()
    {
        if (!IsActive) return;

        transform.DOScale(Vector3.zero, SCALE_DURATION).OnComplete(() =>
        {
            IsActive = false;
        });

        SetAlpha(0);
    }

    protected void SetAlpha(float alpha)
    {
        alphaCanvas.DOFade(alpha, FADE_DURATION);
    }
}
