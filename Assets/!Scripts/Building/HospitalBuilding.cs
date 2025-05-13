using UniRx;
using UnityEngine;

public class HospitalBuilding : RepairableBuilding,ISaveableBuilding
{
    [Header("Hospital Config")]
    [SerializeField] private HospitalBuildingConfig hospitalConfig;
    public HospitalBuildingConfig HospitalBuildingConfig => hospitalConfig;

    private int _turnsToGiveMedicine { get; set; }
    public int DaysToGiveMedicine { get; private set; }

    private bool _medicineWasGivenAwayInLastTwoDays;
    private bool _isWorking;
    private int _turnsToCreateNewUnit;
    
    public readonly Subject<Unit> OnHospitalUnitHealed = new();

    public override void Init()
    {
        base.Init();

        _timeController.OnNextTurnBtnClickBetween
            .Subscribe(_ => OnNextTurnEvent())
            .AddTo(this);

        UpdateAmountOfTurnsNeededToDoSmth();
        
        DaysToGiveMedicine = hospitalConfig.OriginalDaysToGiveMedicine;
        
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
        _turnsToGiveMedicine = UpdateAmountOfTurnsNeededToDoSmth(hospitalConfig.TurnsToGiveMedicineOriginal);
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

        _peopleUnitsController.AssignUnitsToTask(hospitalConfig.PeopleToGiveMedicine, _turnsToGiveMedicine, hospitalConfig.TurnsToRestFromMedicine);
        _resourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Medicine, -hospitalConfig.MedicineToGive));
        _resourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Stability, hospitalConfig.StabilityAddValue));
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

        _resourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Stability,
            -hospitalConfig.StabilityRemoveValue));
        return false;
    }
    
    public void InjuredUnitStartedHealing()
    {
        _isWorking = true;
        _turnsToCreateNewUnit = hospitalConfig.TurnsToHealInjuredUnit;
        _resourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Medicine, -hospitalConfig.MedicineToHealInjuredUnit));
    }
    
    public new int BuildingID => base.BuildingID;

    public override BuildingSaveData SaveData()
    {
        return new HospitalBuildingSaveData
        {
            buildingID = BuildingID,
            buildingIsSelectable = BuildingIsSelectable,
            turnsToGiveMedicine = _turnsToGiveMedicine,
            daysToGiveMedicine = DaysToGiveMedicine,
            medicineWasGivenAwayInLastTwoDays = _medicineWasGivenAwayInLastTwoDays,
            isWorking = _isWorking,
            turnsToCreateNewUnit = _turnsToCreateNewUnit,
            turnsToRepair = TurnsToRepair,
            currentState = CurrentState
        };
    }

    public override void LoadData(BuildingSaveData data)
    {
        var save = data as HospitalBuildingSaveData;
        if (save == null) return;

        BuildingIsSelectable = save.buildingIsSelectable;
        _turnsToGiveMedicine = save.turnsToGiveMedicine;
        DaysToGiveMedicine = save.daysToGiveMedicine;
        _medicineWasGivenAwayInLastTwoDays = save.medicineWasGivenAwayInLastTwoDays;
        _isWorking = save.isWorking;
        _turnsToCreateNewUnit = save.turnsToCreateNewUnit;
        TurnsToRepair = save.turnsToRepair;
        SetState(save.currentState);

        if (save.buildingIsSelectable)
            RestoreOriginalMaterials();
        else
            SetGreyMaterials();
        
        // PopUpsController.HospitalPopUp.SetButtonState
        //     (PopUpsController.HospitalPopUp.GiveMedicineBtnParent,!_medicineWasGivenAwayInLastTwoDays);
    }

}
