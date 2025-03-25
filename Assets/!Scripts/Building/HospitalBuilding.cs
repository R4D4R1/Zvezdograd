using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

public class HospitalBuilding : RepairableBuilding
{
    [FormerlySerializedAs("_hospitalConfig")]
    [Header("Hospital Config")]
    [SerializeField] private HospitalBuildingConfig hospitalConfig;

    public int MedicineToHealInjuredUnit { get; private set; }
    private int TurnsToGiveMedicine { get; set; }
    public int DaysToGiveMedicine { get; private set; }
    public int PeopleToGiveMedicine { get; private set; }
    public int MedicineToGive { get; private set; }

    private bool _medicineWasGivenAwayInLastTwoDays;
    private bool _isWorking;
    private int _turnsToCreateNewUnit;
    
    public readonly Subject<Unit> OnHospitalUnitHealed = new();


    public override void Init()
    {
        base.Init();

        TimeController.OnNextTurnBtnClickBetween
            .Subscribe(_ => OnNextTurnEvent())
            .AddTo(this);

        UpdateAmountOfTurnsNeededToDoSmth();

        MedicineToHealInjuredUnit = hospitalConfig.MedicineToHealInjuredUnit;
        DaysToGiveMedicine = hospitalConfig.OriginalDaysToGiveMedicine;
        PeopleToGiveMedicine = hospitalConfig.PeopleToGiveMedicine;
        MedicineToGive = hospitalConfig.MedicineToGive;
        
        _medicineWasGivenAwayInLastTwoDays = false;
        _isWorking = false;
    }

    private void OnNextTurnEvent()
    {
        UpdateAmountOfTurnsNeededToDoSmth();
        CheckIfHealedInjuredUnit();
    }
    
    private void UpdateAmountOfTurnsNeededToDoSmth()
    {
        TurnsToGiveMedicine = UpdateAmountOfTurnsNeededToDoSmth(hospitalConfig.TurnsToGiveMedicineOriginal);
    }

    private void CheckIfHealedInjuredUnit()
    {
        if (_isWorking)
        {
            _turnsToCreateNewUnit--;
            if (_turnsToCreateNewUnit == 0)
            {
                OnHospitalUnitHealed.OnNext(Unit.Default);
                _isWorking = false;
            }
        }
    }
    
    public void SendPeopleToGiveMedicine()
    {
        _medicineWasGivenAwayInLastTwoDays = true;

        PeopleUnitsController.AssignUnitsToTask(hospitalConfig.PeopleToGiveMedicine, TurnsToGiveMedicine, hospitalConfig.TurnsToRestFromMedicine);
        ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Medicine, -hospitalConfig.MedicineToGive));
        ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Stability, hospitalConfig.StabilityAddValue));
    }

    public bool MedicineWasGiven()
    {
        DaysToGiveMedicine--;

        if (DaysToGiveMedicine != 0) return false;
        
        DaysToGiveMedicine = hospitalConfig.OriginalDaysToGiveMedicine;

        if (_medicineWasGivenAwayInLastTwoDays)
        {
            _medicineWasGivenAwayInLastTwoDays = false;

            return true;
        }
        _medicineWasGivenAwayInLastTwoDays = false;

        ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Stability,
            -hospitalConfig.StabilityRemoveValue));
        return false;
    }
    
    public void InjuredUnitStartedHealing()
    {
        _isWorking = true;
        _turnsToCreateNewUnit = hospitalConfig.TurnsToHealInjuredUnit;
        ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Medicine, -MedicineToHealInjuredUnit));
    }
}
