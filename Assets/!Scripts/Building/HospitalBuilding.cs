using UnityEngine;
using UniRx;

public class HospitalBuilding : RepairableBuilding
{
    [Header("HOSPITAL SETTINGS")]

    [SerializeField] private int _peopleToGiveMedicine;
    public int PeopleToGiveMedicine => _peopleToGiveMedicine;

    [SerializeField] private int _medicineToGive;
    public int MedicineToGive => _medicineToGive;
    [SerializeField] private int _turnsToGiveMedicineOriginal;
    public int TurnsToToGiveMedicine { get; private set; }

    [SerializeField] private int _turnsToRestFromMedicineJob;
    public int TurnsToRestFromMedicineJob => _turnsToRestFromMedicineJob;

    [SerializeField] private int _originalDaysToGiveMedicine;
    public int OriginalDaysToGiveMedicine => _originalDaysToGiveMedicine;

    [SerializeField] private int _stabilityAddValue;
    public int StabilityAddValue => _stabilityAddValue;

    [SerializeField] private int _stabilityRemoveValue;
    public int StabilityRemoveValue => _stabilityRemoveValue;

    // SAVE DATA
    public int DaysToGiveMedicine { get; private set; }
    private bool _medicineWasGivenAwayInLastTwoDay = false;


    protected void Awake()
    {
        _controllersManager.TimeController.OnNextTurnBtnPressed
            .Subscribe(_ => UpdateAmountOfTurnsNeededToDoSMTH())
            .AddTo(this);

        DaysToGiveMedicine = OriginalDaysToGiveMedicine;
        UpdateAmountOfTurnsNeededToDoSMTH();
    }

    private void UpdateAmountOfTurnsNeededToDoSMTH()
    {
        TurnsToToGiveMedicine = UpdateAmountOfTurnsNeededToDoSMTH(_turnsToGiveMedicineOriginal);
    }

    public void SendPeopleToGiveMedicine()
    {
        _medicineWasGivenAwayInLastTwoDay = true;

        _controllersManager.PeopleUnitsController.AssignUnitsToTask(PeopleToGiveMedicine, TurnsToToGiveMedicine, TurnsToRestFromMedicineJob);
        _resourceViewModel.ModifyResource(ResourceModel.ResourceType.Medicine, -MedicineToGive);
        _resourceViewModel.ModifyResource(ResourceModel.ResourceType.Stability, StabilityAddValue);
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
                _resourceViewModel.ModifyResource(ResourceModel.ResourceType.Stability, -StabilityRemoveValue);
                return false;
            }
        }
        else
            return false;
    }
}
