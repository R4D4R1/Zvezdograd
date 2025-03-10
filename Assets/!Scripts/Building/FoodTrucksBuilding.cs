using UnityEngine;

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

    protected override void Start()
    {
        base.Start();
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += UpdateAmountOfTurnsNeededToDoSMTH;

        UpdateAmountOfTurnsNeededToDoSMTH();
    }

    private void UpdateAmountOfTurnsNeededToDoSMTH()
    {
        TurnsToToGiveProvision = UpdateAmountOfTurnsNeededToDoSMTH(_turnsToToGiveProvisionOriginal);
    }

    public void SendPeopleToGiveProvision()
    {
        IsFoodGivenAwayToday = true;

        ControllersManager.Instance.peopleUnitsController.AssignUnitsToTask(PeopleToGiveProvision, TurnsToToGiveProvision, TurnsToRestFromProvisionJob);
        ControllersManager.Instance.resourceController.ModifyResource(ResourceController.ResourceType.Provision,-FoodToGive);
        ControllersManager.Instance.resourceController.ModifyResource(ResourceController.ResourceType.Stability, StabilityAddValue);
    }

    public bool FoodWasGivenAwayToday()
    {
        if (IsFoodGivenAwayToday)
        {
            return true;
        }
        else
        {
            ControllersManager.Instance.resourceController.ModifyResource(ResourceController.ResourceType.Stability, -StabilityRemoveValue);
            return false;
        }
    }
}