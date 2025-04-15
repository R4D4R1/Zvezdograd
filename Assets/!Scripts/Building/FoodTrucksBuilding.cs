using UnityEngine;
using UniRx;

public class FoodTrucksBuilding : RepairableBuilding,ISaveableBuilding
{
    [Header("FOOD TRUCKS SETTINGS")]
    [SerializeField] private FoodTrucksBuildingConfig foodTrucksConfig;
    public FoodTrucksBuildingConfig FoodTrucksBuildingConfig => foodTrucksConfig;

    private int _turnsToToGiveProvision { get; set; }
    private bool _isFoodGivenAwayToday { get; set; }

    public override void Init()
    {
        base.Init();
        TimeController.OnNextTurnBtnClickBetween
            .Subscribe(_ => UpdateAmountOfTurnsNeededToDoSmth())
            .AddTo(this);

        UpdateAmountOfTurnsNeededToDoSmth();
        
        _isFoodGivenAwayToday = false;
    }

    private void UpdateAmountOfTurnsNeededToDoSmth()
    {
        _turnsToToGiveProvision = UpdateAmountOfTurnsNeededToDoSmth(foodTrucksConfig.TurnsToGiveProvisionOriginal);
    }

    public void SendPeopleToGiveProvision()
    {
        _isFoodGivenAwayToday = true;
        PeopleUnitsController.AssignUnitsToTask(foodTrucksConfig.PeopleToGiveProvision,
            _turnsToToGiveProvision, foodTrucksConfig.TurnsToRestFromProvisionJob);

        ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Provision, -foodTrucksConfig.FoodToGive));
        ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Stability, foodTrucksConfig.StabilityAddValue));
    }

    public bool FoodWasGivenAwayToday()
    {
        if (_isFoodGivenAwayToday)
        {
            return true;
        }
        
        _isFoodGivenAwayToday = false;
        ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Stability, -foodTrucksConfig.StabilityRemoveValue));
        return false;
    }
    
    public new int BuildingID => base.BuildingID;

    public override BuildingSaveData SaveData()
    {
        return new FoodTrucksBuildingSaveData
        {
            buildingID = BuildingID,
            buildingIsSelectable = BuildingIsSelectable,
            turnsToToGiveProvision = _turnsToToGiveProvision,
            isFoodGivenAwayToday = _isFoodGivenAwayToday,
            turnsToRepair = TurnsToRepair,
            currentState = CurrentState
        };
    }
    
    public override void LoadData(BuildingSaveData data)
    {
        var save = data as FoodTrucksBuildingSaveData;
        if (save == null) return;
        
        BuildingIsSelectable = save.buildingIsSelectable;
        _turnsToToGiveProvision = save.turnsToToGiveProvision;
        _isFoodGivenAwayToday = save.isFoodGivenAwayToday;
        TurnsToRepair = save.turnsToRepair;
        SetState(save.currentState);

        if (save.buildingIsSelectable)
            RestoreOriginalMaterials();
        else
            SetGreyMaterials();
        
        // PopUpsController.FoodTrucksPopUp.SetButtonState
        //     (PopUpsController.FoodTrucksPopUp.GiveFoodBtnParent,!IsFoodGivenAwayToday);
    }

}
