using UnityEngine;
using TMPro;
using DG.Tweening;
using Zenject;

public class InfoPopUp : MonoBehaviour
{
    [Header("INFO POPUP SETTINGS")]
    public TextMeshProUGUI LabelText;
    public TextMeshProUGUI DescriptionText;
    [SerializeField] private CanvasGroup alphaCanvas;

    public bool IsActive { get; protected set; }

    protected const float SCALE_DURATION = 0.25f;
    private const float FADE_DURATION = 0.25f;

    // Dependencies
    protected ResourceViewModel ResourceViewModel;
    protected PopUpFactory PopUpFactory;
    protected PeopleUnitsController PeopleUnitsController;
    protected TimeController TimeController;
    protected BuildingController BuildingController;
    protected MainGameController MainGameController;
    protected MainGameUIController MainGameUIController;
    protected TutorialController TutorialController;
    protected PopUpsController PopUpsController;
    protected EventController EventController;

    [Inject]
    public void Construct(
        ResourceViewModel resourceViewModel,
        PopUpFactory popUpFactory,
        PeopleUnitsController peopleUnitsController,
        TimeController timeController,
        BuildingController buildingController,
        MainGameController mainGameController,
        MainGameUIController mainGameUIController,
        TutorialController tutorialController,
        PopUpsController popUpsController,
        EventController eventController)
    {
        ResourceViewModel = resourceViewModel;
        PopUpFactory = popUpFactory;
        PeopleUnitsController = peopleUnitsController;
        TimeController = timeController;
        BuildingController = buildingController;
        MainGameController = mainGameController;
        MainGameUIController = mainGameUIController;
        TutorialController = tutorialController;
        PopUpsController = popUpsController;
        EventController = eventController;
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
        MainGameUIController.TurnOnUI();
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
