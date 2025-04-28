using DG.Tweening;
using TMPro;
using UnityEngine;

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

    [HideInInspector] public PopUpFuncs CurrentFunc;
    [HideInInspector] public RepairableBuilding RepairableBuilding;
    [HideInInspector] public CollectableBuilding CollectableBuilding;
    [HideInInspector] public FactoryBuilding FactoryBuilding;

    private CollectPopUp _collectPopUp;
    private RepairPopUp _repairPopUp;
    private FactoryPopUp _factoryPopUp;
    private CityHallPopUp _cityHallPopUp;
    private FoodTrucksPopUp _foodTrucksPopUp;
    private HospitalPopUp _hospitalPopUp;

    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        SetAlpha(0);
    }
    
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _collectPopUp = _popUpsController.CollectPopUp;
        _repairPopUp = _popUpsController.RepairPopUp;
        _factoryPopUp = _popUpsController.FactoryPopUp;
        _cityHallPopUp = _popUpsController.CityHallPopUp;
        _foodTrucksPopUp = _popUpsController.FoodTrucksPopUp;
        _hospitalPopUp = _popUpsController.HospitalPopUp;
    }

    public void ShowPopUp(string label, string description, string button)
    {
        IsActive = true;

        LabelText.text = "";
        DescriptionText.text = "";
        ButtonText.text = "";

        transform.DOScale(Vector3.one, SCALE_DURATION).OnComplete(() =>
        {
            LabelText.text = label;
            DescriptionText.text = description;
            ButtonText.text = button;
            SetAlpha(1);
        });
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
                _foodTrucksPopUp.ShowPopUp();
                break;
            case PopUpFuncs.OpenHospitalMenu:
                _hospitalPopUp.ShowPopUp();
                break;
            case PopUpFuncs.OpenNextTutorialPopUp:
                _tutorialController.ShowTutorial();
                break;
        }

        if (CurrentFunc != PopUpFuncs.OpenNextTutorialPopUp)
        {
            _mainGameUIController.TurnOffUI();
        }

        HidePopUp();
    }
}
