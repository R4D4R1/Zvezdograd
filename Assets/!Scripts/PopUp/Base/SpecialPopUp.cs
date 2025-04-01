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

    [HideInInspector]
    public RepairableBuilding RepairableBuilding;
    [HideInInspector]
    public CollectableBuilding CollectableBuilding;
    [HideInInspector]
    public FactoryBuilding FactoryBuilding;

    [HideInInspector]
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
        _collectPopUp = PopUpsController.CollectPopUp;
        _repairPopUp = PopUpsController.RepairPopUp;
        _factoryPopUp = PopUpsController.FactoryPopUp;
        _cityHallPopUp = PopUpsController.CityHallPopUp;
        _foodTrucksPopUp = PopUpsController.FoodTrucksPopUp;
        _hospitalPopUp = PopUpsController.HospitalPopUp;
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
                _foodTrucksPopUp.ShowPopUp();
                break;
            case PopUpFuncs.OpenHospitalMenu:
                _hospitalPopUp.ShowPopUp();
                break;
            case PopUpFuncs.OpenNextTutorialPopUp:
                TutorialController.ShowTutorial();
                break;
        }

        if (CurrentFunc != PopUpFuncs.OpenNextTutorialPopUp)
        {
            Debug.Log("Turn off ui");
            MainGameUIController.TurnOffUI();
        }

        HidePopUp();
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
}