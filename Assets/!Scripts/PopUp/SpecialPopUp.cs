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
    }

    [HideInInspector]
    public RepairableBuilding RepairableBuilding;
    [HideInInspector]
    public CollectableBuilding CollectableBuilding;
    [HideInInspector]
    public FactoryBuilding FactoryBuilding;
    [HideInInspector]
    public CityHallBuilding CityHallBuilding;
    [HideInInspector]
    public FoodTrucksBuilding FoodTrucksBuilding;
    [HideInInspector]
    public HospitalBuilding HospitalBuilding;

    public PopUpFuncs CurrentFunc;

    private CollectPopUp _collectPopUp;
    private RepairPopUp _repairPopUp;
    private FactoryPopUp _factoryPopUp;
    private CityHallPopUp _cityHallPopUp;
    private FoodTruckPopUp _foodTrucksPopUp;
    private HospitalPopUp _hospitalPopUp;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _collectPopUp = ControllersManager.Instance.selectionController._collectPopUp;
        _repairPopUp = ControllersManager.Instance.selectionController._repairPopUp;
        _factoryPopUp = ControllersManager.Instance.selectionController._factoryPopUp;
        _cityHallPopUp = ControllersManager.Instance.selectionController._cityHallPopUp;
        _foodTrucksPopUp = ControllersManager.Instance.selectionController._foodTrucksPopUp;
        _hospitalPopUp = ControllersManager.Instance.selectionController._hospitalPopUp;
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
                break;
            case PopUpFuncs.OpenRepairMenu:
                _repairPopUp.ShowRepairPopUp(RepairableBuilding);
                break;
            case PopUpFuncs.OpenFactoryMenu:
                _factoryPopUp.ShowFactoryPopUp(FactoryBuilding);
                break;
            case PopUpFuncs.OpenCityHallMenu:
                _cityHallPopUp.ShowCityHallPopUp(CityHallBuilding);
                break;
            //case PopUpFuncs.OpenFoodTrucksMenu:
            //    _foodTrucksPopUp.ShowFactoryPopUp(FactoryBuilding);
            //    break;
            //case PopUpFuncs.OpenHospitalMenu:
            //    _hospitalPopUp.ShowFactoryPopUp(FactoryBuilding);
            //    break;
            default:
                break;
        }

        ControllersManager.Instance.mainGameUIController.DisableEscapeMenuToggle();
        ControllersManager.Instance.blurController.BlurBackGroundSmoothly();
        ControllersManager.Instance.mainGameUIController.TurnOffUI();

        HidePopUp();
    }

    public void ShowPopUp(string Label, string Description, string Button)
    {
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