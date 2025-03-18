using UnityEngine;
using UniRx;

public class FoodTrucksBuilding : RepairableBuilding
{
    [Header("FOOD TRUCKS SETTINGS")]
    [SerializeField] private int _peopleToGiveProvision;

    public int PeopleToGiveProvision => _peopleToGiveProvision;

    [SerializeField] private int _turnsToToGiveProvisionOriginal;
    public int TurnsToToGiveProvision { get; private set; }

    [SerializeField] private int _turnsToRestFromProvisionJob;
    public int TurnsToRestFromProvisionJob => _turnsToRestFromProvisionJob;


    [SerializeField] private int _foodToGive;
    public int FoodToGive => _foodToGive;


    [SerializeField] private int _stabilityAddValue;
    public int StabilityAddValue => _stabilityAddValue;


    [SerializeField] private int _stabilityRemoveValue;
    public int StabilityRemoveValue => _stabilityRemoveValue;

    // SAVE DATA
    public bool IsFoodGivenAwayToday { get; private set; } = false;

    public override void Init()
    {
        base.Init();
        _controllersManager.TimeController.OnNextTurnBtnPressed
            .Subscribe(_ => UpdateAmountOfTurnsNeededToDoSMTH())
            .AddTo(this);

        UpdateAmountOfTurnsNeededToDoSMTH();
    }

    private void UpdateAmountOfTurnsNeededToDoSMTH()
    {
        TurnsToToGiveProvision = UpdateAmountOfTurnsNeededToDoSMTH(_turnsToToGiveProvisionOriginal);
    }

    public void SendPeopleToGiveProvision()
    {
        IsFoodGivenAwayToday = true;

        _controllersManager.PeopleUnitsController.AssignUnitsToTask(PeopleToGiveProvision, TurnsToToGiveProvision, TurnsToRestFromProvisionJob);
        _resourceViewModel.ModifyResource(ResourceModel.ResourceType.Provision, -FoodToGive);
        _resourceViewModel.ModifyResource(ResourceModel.ResourceType.Stability, StabilityAddValue);
    }

    public bool FoodWasGivenAwayToday()
    {
        if (IsFoodGivenAwayToday)
        {
            return true;
        }
        else
        {
            _resourceViewModel.ModifyResource(ResourceModel.ResourceType.Stability, -StabilityRemoveValue);

            return false;
        }
    }
}