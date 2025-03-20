using UnityEngine;
using UniRx;

public class FoodTrucksBuilding : RepairableBuilding
{
    [Header("FOOD TRUCKS SETTINGS")]
    [SerializeField] private FoodTrucksBuildingConfig _foodTrucksConfig;

    public int TurnsToToGiveProvision { get; private set; }
    public int PeopleToGiveProvision { get; private set; }
    public int FoodToGive { get; private set; }

    // SAVE DATA
    public bool IsFoodGivenAwayToday { get; private set; } = false;

    public override void Init()
    {
        base.Init();
        _controllersManager.TimeController.OnNextTurnBtnClickBetween
            .Subscribe(_ => UpdateAmountOfTurnsNeededToDoSMTH())
            .AddTo(this);

        UpdateAmountOfTurnsNeededToDoSMTH();

        PeopleToGiveProvision = _foodTrucksConfig.PeopleToGiveProvision;
        FoodToGive = _foodTrucksConfig.FoodToGive;

    }

    private void UpdateAmountOfTurnsNeededToDoSMTH()
    {
        TurnsToToGiveProvision = UpdateAmountOfTurnsNeededToDoSMTH(_foodTrucksConfig.TurnsToGiveProvisionOriginal);
    }

    public void SendPeopleToGiveProvision()
    {
        IsFoodGivenAwayToday = true;
        _controllersManager.PeopleUnitsController.AssignUnitsToTask(_foodTrucksConfig.PeopleToGiveProvision,
            TurnsToToGiveProvision, _foodTrucksConfig.TurnsToRestFromProvisionJob);

        _resourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Provision, -_foodTrucksConfig.FoodToGive));
        _resourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Stability, _foodTrucksConfig.StabilityAddValue));
    }

    public bool FoodWasGivenAwayToday()
    {
        if (IsFoodGivenAwayToday)
        {
            return true;
        }
        else
        {
            _resourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Stability, -_foodTrucksConfig.StabilityRemoveValue));
            return false;
        }
    }
}
