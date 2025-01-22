using UnityEngine;

public class HospitalBuilding : RepairableBuilding
{
    [field: SerializeField] public int PeopleToGiveMedicine { get; private set; }
    [field: SerializeField] public int TurnsToToGiveMedicine { get; private set; }
    [field: SerializeField] public int TurnsToRestFromMedicineJob { get; private set; }
    [field: SerializeField] public int MedicineToGive { get; private set; }


    [field: SerializeField] public int StabilityNegativeRemoveValue { get; private set; }
    [field: SerializeField] public int OriginalDaysToGiveMedicine { get; private set; }
    public int DaysToGiveMedicine { get; private set; }

    private bool _medicineWasGivenAwayInLastTwoDay = false;

    private void Awake()
    {
        DaysToGiveMedicine = OriginalDaysToGiveMedicine;
    }


    public void SendPeopleToGiveMedicine()
    {
        _medicineWasGivenAwayInLastTwoDay = true;

        ControllersManager.Instance.peopleUnitsController.AssignUnitsToTask(PeopleToGiveMedicine, TurnsToToGiveMedicine, TurnsToRestFromMedicineJob);
        ControllersManager.Instance.resourceController.AddOrRemoveMedicine(-MedicineToGive);
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
                ControllersManager.Instance.resourceController.AddOrRemoveStability(StabilityNegativeRemoveValue);
                return false;
            }
        }
        else
            return false;
    }
}
