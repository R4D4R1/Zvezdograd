using UniRx;
using UnityEngine;

public class HospitalBuilding : RepairableBuilding
{
    [Header("Hospital Config")]
    [SerializeField] private HospitalBuildingConfig _hospitalConfig;

    public int TurnsToGiveMedicine { get; private set; }
    public int DaysToGiveMedicine { get; private set; }
    public int PeopleToGiveMedicine { get; private set; }
    public int MedicineToGive { get; private set; }

    private bool _medicineWasGivenAwayInLastTwoDays = false;

    public override void Init()
    {
        base.Init();

        _controllersManager.TimeController.OnNextTurnBtnClickBetween
            .Subscribe(_ => UpdateAmountOfTurnsNeededToDoSMTH())
            .AddTo(this);

        UpdateAmountOfTurnsNeededToDoSMTH();

        DaysToGiveMedicine = _hospitalConfig.OriginalDaysToGiveMedicine;
        PeopleToGiveMedicine = _hospitalConfig.PeopleToGiveMedicine;
        MedicineToGive = _hospitalConfig.MedicineToGive;
    }

    private void UpdateAmountOfTurnsNeededToDoSMTH()
    {
        TurnsToGiveMedicine = UpdateAmountOfTurnsNeededToDoSMTH(_hospitalConfig.TurnsToGiveMedicineOriginal);
    }

    public void SendPeopleToGiveMedicine()
    {
        _medicineWasGivenAwayInLastTwoDays = true;

        _controllersManager.PeopleUnitsController.AssignUnitsToTask(_hospitalConfig.PeopleToGiveMedicine, TurnsToGiveMedicine, _hospitalConfig.TurnsToRestFromMedicineJob);
        _resourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Medicine, -_hospitalConfig.MedicineToGive));
        _resourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Stability, _hospitalConfig.StabilityAddValue));
    }

    public bool MedicineWasGiven()
    {
        DaysToGiveMedicine--;

        if (DaysToGiveMedicine == 0)
        {
            DaysToGiveMedicine = _hospitalConfig.OriginalDaysToGiveMedicine;

            if (_medicineWasGivenAwayInLastTwoDays)
            {
                return true;
            }
            else
            {
                _resourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Stability, -_hospitalConfig.StabilityRemoveValue));

                return false;
            }
        }

        return false;
    }
}
