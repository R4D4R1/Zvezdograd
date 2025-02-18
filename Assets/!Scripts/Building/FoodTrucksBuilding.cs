using UnityEngine;

public class FoodTrucksBuilding : RepairableBuilding
{
    [field: SerializeField] public int PeopleToGiveProvision { get; private set; }

    [SerializeField] private int TurnsToToGiveProvisionOriginal;
    public int TurnsToToGiveProvision { get; private set; }
    [field: SerializeField] public int TurnsToRestFromProvisionJob { get; private set; }
    [field: SerializeField] public int FoodToGive { get; private set; }
    [field: SerializeField] public int StabilityAddValue { get; private set; }
    [field: SerializeField] public int StabilityRemoveValue { get; private set; }

    // SAVE DATA
    public bool IsFoodGivenAwayToday { get; private set; } = false;

    private void Start()
    {
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += UpdateAmountOfTurnsNeededToDoSMTH;

        UpdateAmountOfTurnsNeededToDoSMTH();
    }

    private void UpdateAmountOfTurnsNeededToDoSMTH()
    {
        TurnsToToGiveProvision = UpdateAmountOfTurnsNeededToDoSMTH(TurnsToToGiveProvisionOriginal);
    }

    public void SendPeopleToGiveProvision()
    {
        IsFoodGivenAwayToday = true;

        ControllersManager.Instance.peopleUnitsController.AssignUnitsToTask(PeopleToGiveProvision, TurnsToToGiveProvision, TurnsToRestFromProvisionJob);
        ControllersManager.Instance.resourceController.AddOrRemoveProvision(-FoodToGive);
        ControllersManager.Instance.resourceController.AddOrRemoveStability(StabilityAddValue);
    }

    public bool FoodWasGivenAwayToday()
    {
        if (IsFoodGivenAwayToday)
        {
            return true;
        }
        else
        {
            ControllersManager.Instance.resourceController.AddOrRemoveStability(-StabilityRemoveValue);
            return false;
        }
    }
}