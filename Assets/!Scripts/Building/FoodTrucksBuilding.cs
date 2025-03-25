using UnityEngine;
using UniRx;
using UnityEngine.Serialization;

public class FoodTrucksBuilding : RepairableBuilding
{
    [FormerlySerializedAs("_foodTrucksConfig")]
    [Header("FOOD TRUCKS SETTINGS")]
    [SerializeField] private FoodTrucksBuildingConfig foodTrucksConfig;

    private int TurnsToToGiveProvision { get; set; }
    public int PeopleToGiveProvision { get; private set; }
    public int FoodToGive { get; private set; }

    // SAVE DATA
    private bool IsFoodGivenAwayToday { get; set; }

    public override void Init()
    {
        base.Init();
        TimeController.OnNextTurnBtnClickBetween
            .Subscribe(_ => UpdateAmountOfTurnsNeededToDoSmth())
            .AddTo(this);

        UpdateAmountOfTurnsNeededToDoSmth();

        PeopleToGiveProvision = foodTrucksConfig.PeopleToGiveProvision;
        FoodToGive = foodTrucksConfig.FoodToGive;
        
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
        IsFoodGivenAwayToday = false;

        if (IsFoodGivenAwayToday)
        {
            return true;
        }
        else
        {
            ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Stability, -foodTrucksConfig.StabilityRemoveValue));
            return false;
        }
    }
}
