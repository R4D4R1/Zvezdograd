using DG.Tweening;
using TMPro;
using UnityEngine;

public class SpecialPopUp : InfoPopUp
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

    public PopUpFuncs CurrentFunc;

    private CollectPopUp _collectPopUp;
    private RepairPopUp _repairPopUp;
    private FactoryPopUp _factoryPopUp;
    private CityHallPopUp _cityHallPopUp;
    private FoodTrucksPopUp _foodTrucksPopUp;
    private HospitalPopUp _hospitalPopUp;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _collectPopUp = ControllersManager.Instance.popUpsController.CollectPopUp;
        _repairPopUp = ControllersManager.Instance.popUpsController.RepairPopUp;
        _factoryPopUp = ControllersManager.Instance.popUpsController.FactoryPopUp;
        _cityHallPopUp = ControllersManager.Instance.popUpsController.CityHallPopUp;
        _foodTrucksPopUp = ControllersManager.Instance.popUpsController.FoodTrucksPopUp;
        _hospitalPopUp = ControllersManager.Instance.popUpsController.HospitalPopUp;
    }

    private void OnEnable()
    {
        _bgImage.transform.localScale = Vector3.zero;
        SetAlpha(0);
    }

    public void UseButton()
    {
        switch (CurrentFunc)
        {
            case PopUpFuncs.OpenCollectMenu:
                _collectPopUp.ShowCollectPopUp(CollectableBuilding);
                ControllersManager.Instance.mainGameUIController.TurnOffUI();

                break;
            case PopUpFuncs.OpenRepairMenu:
                _repairPopUp.ShowRepairPopUp(RepairableBuilding);
                ControllersManager.Instance.mainGameUIController.TurnOffUI();

                break;
            case PopUpFuncs.OpenFactoryMenu:
                _factoryPopUp.ShowFactoryPopUp(FactoryBuilding);
                ControllersManager.Instance.mainGameUIController.TurnOffUI();

                break;
            case PopUpFuncs.OpenCityHallMenu:
                _cityHallPopUp.ShowCityHallPopUp();
                ControllersManager.Instance.mainGameUIController.TurnOffUI();

                break;
            case PopUpFuncs.OpenFoodTrucksMenu:
                _foodTrucksPopUp.ShowFoodTruckPopUp();
                ControllersManager.Instance.mainGameUIController.TurnOffUI();

                break;
            case PopUpFuncs.OpenHospitalMenu:
                _hospitalPopUp.ShowHospitalPopUp();
                ControllersManager.Instance.mainGameUIController.TurnOffUI();

                break;
            case PopUpFuncs.OpenNextTutorialPopUp:
                ControllersManager.Instance.tutorialController.ShowTutorial();
                break;
            default:
                break;
        }

        HidePopUp();
    }

    public void ShowPopUp(string Label, string Description, string Button)
    {
        IsActive = true;

        LabelText.text = "";
        DescriptionText.text = "";
        ButtonText.text = "";

        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            LabelText.text = Label;
            DescriptionText.text = Description;
            ButtonText.text = Button;

            SetAlpha(1);
        });
    }
}