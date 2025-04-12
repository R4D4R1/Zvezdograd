using System.Threading.Tasks;
using UnityEngine;
using UniRx;
using UnityEngine.Serialization;

public class FoodTrucksBuilding : RepairableBuilding,ISaveableBuilding
{
    [Header("FOOD TRUCKS SETTINGS")]
    [SerializeField] private FoodTrucksBuildingConfig foodTrucksConfig;
    public FoodTrucksBuildingConfig FoodTrucksBuildingConfig => foodTrucksConfig;

    private int TurnsToToGiveProvision { get; set; }
    private bool IsFoodGivenAwayToday { get; set; }

    public override void Init()
    {
        base.Init();
        TimeController.OnNextTurnBtnClickBetween
            .Subscribe(_ => UpdateAmountOfTurnsNeededToDoSmth())
            .AddTo(this);

        UpdateAmountOfTurnsNeededToDoSmth();
        
        IsFoodGivenAwayToday = false;
    }

    private void UpdateAmountOfTurnsNeededToDoSmth()
    {
        TurnsToToGiveProvision = UpdateAmountOfTurnsNeededToDoSmth(foodTrucksConfig.TurnsToGiveProvisionOriginal);
    }

    public void SendPeopleToGiveProvision()
    {
        IsFoodGivenAwayToday = true;
        PeopleUnitsController.AssignUnitsToTask(foodTrucksConfig.PeopleToGiveProvision,
            TurnsToToGiveProvision, foodTrucksConfig.TurnsToRestFromProvisionJob);

        ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Provision, -foodTrucksConfig.FoodToGive));
        ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Stability, foodTrucksConfig.StabilityAddValue));
    }

    public bool FoodWasGivenAwayToday()
    {
        if (IsFoodGivenAwayToday)
        {
            return true;
        }
        
        IsFoodGivenAwayToday = false;
        ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Stability, -foodTrucksConfig.StabilityRemoveValue));
        return false;
    }
    
    public new int BuildingID => base.BuildingID;

    public override BuildingSaveData GetSaveData()
    {
        return new FoodTrucksBuildingSaveData
        {
            buildingID = BuildingID,
            buildingIsSelectable = BuildingIsSelectable,
            turnsToToGiveProvision = TurnsToToGiveProvision,
            isFoodGivenAwayToday = IsFoodGivenAwayToday,
            turnsToRepair = TurnsToRepair,
            currentState = CurrentState
        };
    }
    
    public override void LoadFromSaveData(BuildingSaveData data)
    {
        var save = data as FoodTrucksBuildingSaveData;
        if (save == null) return;
        
        BuildingIsSelectable = save.buildingIsSelectable;
        TurnsToToGiveProvision = save.turnsToToGiveProvision;
        IsFoodGivenAwayToday = save.isFoodGivenAwayToday;
        TurnsToRepair = save.turnsToRepair;
        CurrentState = save.currentState;
        
        if (save.buildingIsSelectable)
            RestoreOriginalMaterials();
        else
            SetGreyMaterials();
        
        // PopUpsController.FoodTrucksPopUp.SetButtonState
        //     (PopUpsController.FoodTrucksPopUp.GiveFoodBtnParent,!IsFoodGivenAwayToday);
    }

}
