using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

public class SpecialPopUp : ReturnToPoolPopUp
{
    public TextMeshProUGUI ButtonText;

    public enum PopUpFuncs
    {
        OpenRepairMenu,
        OpenCollectMenu,
        OpenFactoryMenu,
        OpenCityHallMenu,
        OpenFoodTrucksMenu,
        OpenHospitalMenu,
        OpenNextTutorialPopUp,
    }

    [HideInInspector]
    public RepairableBuilding RepairableBuilding;
    [HideInInspector]
    public CollectableBuilding CollectableBuilding;
    [HideInInspector]
    public FactoryBuilding FactoryBuilding;
    [HideInInspector]
    public FoodTrucksBuilding FoodTrucksBuilding;
    [HideInInspector]
    public HospitalBuilding HospitalBuilding;

    [HideInInspector]
    public PopUpFuncs CurrentFunc;

    private CollectPopUp _collectPopUp;
    private RepairPopUp _repairPopUp;
    private FactoryPopUp _factoryPopUp;
    private CityHallPopUp _cityHallPopUp;
    private FoodTrucksPopUp _foodTrucksPopUp;
    private HospitalPopUp _hospitalPopUp;

    public readonly Subject<Unit> OnSpecialPopUpHide;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _collectPopUp = _controllersManager.PopUpsController.CollectPopUp;
        _repairPopUp = _controllersManager.PopUpsController.RepairPopUp;
        _factoryPopUp = _controllersManager.PopUpsController.FactoryPopUp;
        _cityHallPopUp = _controllersManager.PopUpsController.CityHallPopUp;
        _foodTrucksPopUp = _controllersManager.PopUpsController.FoodTrucksPopUp;
        _hospitalPopUp = _controllersManager.PopUpsController.HospitalPopUp;
    }

    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        SetAlpha(0);
    }

    public void UseButton()
    {
        switch (CurrentFunc)
        {
            case PopUpFuncs.OpenCollectMenu:
                _collectPopUp.ShowCollectPopUp(CollectableBuilding);
                break;
            case PopUpFuncs.OpenRepairMenu:
                _repairPopUp.ShowRepairPopUp(RepairableBuilding);
                break;
            case PopUpFuncs.OpenFactoryMenu:
                _factoryPopUp.ShowFactoryPopUp(FactoryBuilding);
                break;
            case PopUpFuncs.OpenCityHallMenu:
                _cityHallPopUp.ShowCityHallPopUp();
                break;
            case PopUpFuncs.OpenFoodTrucksMenu:
                _foodTrucksPopUp.ShowFoodTruckPopUp();
                break;
            case PopUpFuncs.OpenHospitalMenu:
                _hospitalPopUp.ShowHospitalPopUp();
                break;
            case PopUpFuncs.OpenNextTutorialPopUp:
                _controllersManager.TutorialController.ShowTutorial();
                break;
            default:
                break;
        }

        if (CurrentFunc != PopUpFuncs.OpenNextTutorialPopUp)
        {
            _controllersManager.MainGameUIController.TurnOffUI();
        }

        HidePopUp();
    }

    public void ShowPopUp(string Label, string Description, string Button)
    {
        IsActive = true;

        LabelText.text = "";
        DescriptionText.text = "";
        ButtonText.text = "";

        transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            LabelText.text = Label;
            DescriptionText.text = Description;
            ButtonText.text = Button;

            SetAlpha(1);
        });
    }
}