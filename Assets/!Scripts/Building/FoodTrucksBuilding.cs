using UnityEngine;

public class FoodTrucksBuilding : RepairableBuilding
{
    public static FoodTrucksBuilding Instance;

    [field: SerializeField] public int PeopleToGiveProvision { get; private set; }
    [field: SerializeField] public int TurnsToToGiveProvision { get; private set; }
    [field: SerializeField] public int TurnsToRestFromProvisionJob { get; private set; }
    [field: SerializeField] public int FoodToGive { get; private set; }

    private void Start()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SendPeopleToGiveProvision()
    {
        ControllersManager.Instance.peopleUnitsController.AssignUnitsToTask(PeopleToGiveProvision, TurnsToToGiveProvision, TurnsToRestFromProvisionJob);
        ControllersManager.Instance.resourceController.AddOrRemoveProvision(-FoodToGive);
    }
}