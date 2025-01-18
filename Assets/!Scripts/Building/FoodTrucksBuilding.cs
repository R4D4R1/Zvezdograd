using UnityEngine;

public class FoodTrucksBuilding : RepairableBuilding
{
    [field: SerializeField] public int PeopleToGiveProvision { get; private set; }
    [field: SerializeField] public int TurnsToToGiveProvision { get; private set; }
    [field: SerializeField] public int TurnsToRestFromProvisionJob { get; private set; }
    [field: SerializeField] public int FoodToGive { get; private set; }
    [field: SerializeField] public int StabilityNegativeRemoveValue { get; private set; }


    public void SendPeopleToGiveProvision()
    {
        ControllersManager.Instance.peopleUnitsController.AssignUnitsToTask(PeopleToGiveProvision, TurnsToToGiveProvision, TurnsToRestFromProvisionJob);
        ControllersManager.Instance.resourceController.AddOrRemoveProvision(-FoodToGive);
    }
}