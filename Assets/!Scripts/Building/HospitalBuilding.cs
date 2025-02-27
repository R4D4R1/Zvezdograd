using UnityEngine;

public class HospitalBuilding : RepairableBuilding
{
    [field: SerializeField] public int PeopleToGiveMedicine { get; private set; }
    [SerializeField] private int TurnsToGiveMedicineOriginal;
    public int TurnsToToGiveMedicine { get; private set; }
    [field: SerializeField] public int TurnsToRestFromMedicineJob { get; private set; }
    [field: SerializeField] public int MedicineToGive { get; private set; }
    [field: SerializeField] public int StabilityAddValue { get; private set; }
    [field: SerializeField] public int StabilityRemoveValue { get; private set; }
    [field: SerializeField] public int OriginalDaysToGiveMedicine { get; private set; }

    // SAVE DATA
    public int DaysToGiveMedicine { get; private set; }
    private bool _medicineWasGivenAwayInLastTwoDay = false;


    protected override void Awake()
    {
        base.Awake();
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += UpdateAmountOfTurnsNeededToDoSMTH;
        DaysToGiveMedicine = OriginalDaysToGiveMedicine;
        UpdateAmountOfTurnsNeededToDoSMTH();
    }

    //private void Start()
    //{
    //    ControllersManager.Instance.timeController.OnNextTurnBtnPressed += UpdateAmountOfTurnsNeededToDoSMTH;
    //    DaysToGiveMedicine = OriginalDaysToGiveMedicine;
    //    UpdateAmountOfTurnsNeededToDoSMTH();
    //}

    private void UpdateAmountOfTurnsNeededToDoSMTH()
    {
        TurnsToToGiveMedicine = UpdateAmountOfTurnsNeededToDoSMTH(TurnsToGiveMedicineOriginal);
    }

    public void SendPeopleToGiveMedicine()
    {
        _medicineWasGivenAwayInLastTwoDay = true;

        ControllersManager.Instance.peopleUnitsController.AssignUnitsToTask(PeopleToGiveMedicine, TurnsToToGiveMedicine, TurnsToRestFromMedicineJob);
        ControllersManager.Instance.resourceController.AddOrRemoveMedicine(-MedicineToGive);
        ControllersManager.Instance.resourceController.AddOrRemoveStability(StabilityAddValue);

    }

    public bool MedicineWasGiven()
    {
        DaysToGiveMedicine--;

        if (DaysToGiveMedicine == 0)
        {
            DaysToGiveMedicine = OriginalDaysToGiveMedicine;

            if (_medicineWasGivenAwayInLastTwoDay)
            {
                return true;
            }
            else
            {
                ControllersManager.Instance.resourceController.AddOrRemoveStability(-StabilityRemoveValue);
                return false;
            }
        }
        else
            return false;
    }
}
