using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Serialization;
using Zenject;

public class InfoPopUp : MonoBehaviour
{
    protected const float SCALE_DURATION = 0.25f;
    private const float FADE_DURATION = 0.25f;

    public TextMeshProUGUI LabelText;
    public TextMeshProUGUI DescriptionText;
    [FormerlySerializedAs("_canvasGroup")] [SerializeField] private CanvasGroup canvasGroup;

    public bool IsActive { get; protected set; }

    protected ResourceViewModel ResourceViewModel;
    protected PopUpFactory PopUpFactory;
    protected PeopleUnitsController PeopleUnitsController;
    protected TimeController TimeController;
    protected BuildingController BuildingController;
    protected MainGameController MainGameController;
    protected MainGameUIController MainGameUIController;
    protected TutorialController TutorialController;
    protected PopUpsController PopUpsController;
    
    [Inject]
    public void Construct(
        ResourceViewModel resourceViewModel,PopUpFactory popUpFactory,
        PeopleUnitsController peopleUnitsController, TimeController timeController,
        BuildingController buildingController, MainGameController mainGameController,
        MainGameUIController mainGameUIController,TutorialController tutorialController,
        PopUpsController popUpsController)
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
        if (IsActive)
        {
            transform.DOScale(Vector3.zero, SCALE_DURATION).OnComplete(() =>
            {
                IsActive = false;
            });

            MainGameUIController.TurnOnUI();

            SetAlpha(0);
        }
    }

    public void HideStartInfoPopUpPopUp()
    {
        if (IsActive)
        {
            transform.DOScale(Vector3.zero, SCALE_DURATION).OnComplete(() =>
            {
                IsActive = false;
            });

            SetAlpha(0);
        }
    }

    protected void SetAlpha(float alpha)
    {
        canvasGroup.DOFade(alpha, FADE_DURATION);
    }
}