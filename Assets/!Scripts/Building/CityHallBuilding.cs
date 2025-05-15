using UnityEngine;
using UniRx;
using Unit = UniRx.Unit;

public class CityHallBuilding : RepairableBuilding,ISaveableBuilding
{
    [Header("CITY HALL SETTINGS")]
    [SerializeField] private CityHallBuildingConfig cityHallConfig;
    public CityHallBuildingConfig CityHallBuildingConfig => cityHallConfig;
    public int RelationWithGovernment { get; private set; }
    public int DaysLeftToReceiveGovHelp { get; private set; }
    public int DaysLeftToSendArmyMaterials { get; private set; }
    public bool IsMaterialsSent { get; private set; }
    
    private int _amountOfHelpSent;
    private int _turnsToCreateNewUnit;
    private int _turnsToCreateNewActionPoints;
    private bool _isWorking;
    private bool _isCreatingActionPoints;

    public readonly Subject<Unit> OnCityHallUnitCreated = new();
    public readonly Subject<int> OnNewActionPointsStartedCreating = new();
    public readonly Subject<Unit> OnNewActionPointsCreated = new();
    public readonly Subject<Unit> OnLastMilitaryHelpSent = new();
    public readonly Subject<Unit> OnMilitaryHelpSent = new();
    public readonly Subject<MainGameController.GameOverStateEnum> OnArmyMaterialsSentWin = new();
    public readonly Subject<Unit> OnDidntSendArmyMaterialsInTime = new();

    public override void Init()
    {
        base.Init();

        _timeController.OnNextDayEvent
            .Subscribe(_ => OnNextDayEvent())
            .AddTo(this);

        _timeController.OnNextTurnBtnClickBetween
            .Subscribe(_ => OnNextTurnEvent())
            .AddTo(this);

        RelationWithGovernment = cityHallConfig.RelationWithGovernmentOriginal;
        DaysLeftToReceiveGovHelp = cityHallConfig.DaysLeftToReceiveGovHelpOriginal;
        DaysLeftToSendArmyMaterials = cityHallConfig.DaysLeftToSendArmyMaterialsOriginal;
            
        _isWorking = false;
        _isCreatingActionPoints = false;
        _amountOfHelpSent = 0;
    }
    
    private void OnNextTurnEvent()
    {
        CheckEveryTurn();
    }
    
    private void CheckEveryTurn()
    {
        if (_isWorking)
        {
            if (!_isCreatingActionPoints)
            {
                _turnsToCreateNewUnit--;

                if (_turnsToCreateNewUnit == 0)
                {
                    OnCityHallUnitCreated.OnNext(Unit.Default);
                    _isWorking = false;
                }
            }
            else
            {
                _turnsToCreateNewActionPoints--;

                if (_turnsToCreateNewActionPoints == 0)
                {
                    OnNewActionPointsCreated.OnNext(Unit.Default);
                    _isWorking = false;
                }
            }
        }

        if (_timeController.DaysSinceStart >= (cityHallConfig.DaysLeftToSendArmyMaterialsOriginal * cityHallConfig.AmountOfHelpNeededToSend + 2))
        {
            _mainGameController.SetGameOverState(MainGameController.GameOverStateEnum.NoTimeLeftLose);
        }
    }

    private void OnNextDayEvent()
    {
        ProcessGovHelp();
        DayPassed();
    }

    private void ProcessGovHelp()
    {
        if (--DaysLeftToReceiveGovHelp <= 0)
        {
            DaysLeftToReceiveGovHelp = cityHallConfig.DaysLeftToReceiveGovHelpOriginal;
            ReceiveHelpFromGov();
        }
    }

    private bool DayPassed()
    {
        if (--DaysLeftToSendArmyMaterials <= 0)
        {
            DaysLeftToSendArmyMaterials = cityHallConfig.DaysLeftToSendArmyMaterialsOriginal;
            return HandleArmyMaterials();
        }
        return false;
    }

    private bool HandleArmyMaterials()
    {
        if (IsMaterialsSent)
        {
            OnMilitaryHelpSent.OnNext(Unit.Default);
            IsMaterialsSent = false;
            return false;
        }

        if (RelationWithGovernment > 1)
        {
            RelationWithGovernment -= 2;
            return true;
        }
        return false;
    }

    private void ReceiveHelpFromGov()
    {
        var foodAmount = RelationWithGovernment < 4 ? 2 : RelationWithGovernment < 9 ? 3 : 4;
        var medicineAmount = RelationWithGovernment < 4 ? 1 : 2;

        _resourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Provision, foodAmount));
        _resourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Medicine, medicineAmount));
    }

    public void ArmyMaterialsSent()
    {
        _amountOfHelpSent++;
        IsMaterialsSent = true;
        ModifyRelationWithGov(cityHallConfig.AmountOfRelationAddForArmySent);

        if (_amountOfHelpSent >= cityHallConfig.AmountOfHelpNeededToSend)
        {
            OnLastMilitaryHelpSent.OnNext(Unit.Default);
            _timeController.WaitDaysAndExecute(_mainGameController._mainGameControllerConfig.DayAfterLastArmyMaterialSendWin, () =>
            {
                OnArmyMaterialsSentWin.OnNext(MainGameController.GameOverStateEnum.WinBySendingArmyMaterials);
                Debug.Log($"Победа спустя {_mainGameController._mainGameControllerConfig.DayAfterLastArmyMaterialSendWin} игровых дня");
            });
        }
    }

    public void NewUnitStartedCreating()
    {
        _isWorking = true;
        _turnsToCreateNewUnit = cityHallConfig.TurnsToCreateNewUnitOriginal;
        _resourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.ReadyMaterials, -cityHallConfig.ReadyMaterialsToCreateNewPeopleUnit));
    }
    
    public void ActionPointsStartedCreating(int amountOfReadyUnitsToCreateNewActionPoints)
    {
        _isWorking = true;
        _isCreatingActionPoints = true;
        _turnsToCreateNewActionPoints = cityHallConfig.TurnsToCreateNewActionPoints;
        OnNewActionPointsStartedCreating.OnNext(amountOfReadyUnitsToCreateNewActionPoints);
    }

    public void ModifyRelationWithGov(int amount)
    {
        RelationWithGovernment = Mathf.Clamp(RelationWithGovernment + amount, 1, 10);
    }

    public new int BuildingID => base.BuildingID;

    public override BuildingSaveData SaveData()
    {
        return new CityHallBuildingSaveData
        {
            buildingID = BuildingID,
            buildingIsSelectable = BuildingIsSelectable,
            relationWithGovernment = RelationWithGovernment,
            daysLeftToReceiveGovHelp = DaysLeftToReceiveGovHelp,
            daysLeftToSendArmyMaterials = DaysLeftToSendArmyMaterials,
            isMaterialsSent = IsMaterialsSent,
            amountOfHelpSent = _amountOfHelpSent,
            turnsToCreateNewUnit = _turnsToCreateNewUnit,
            turnsToCreateNewActionPoints = _turnsToCreateNewActionPoints,
            isWorking = _isWorking,
            isCreatingActionPoints = _isCreatingActionPoints,
            turnsToRepair = TurnsToRepair,
            currentState = CurrentState
        };
    }

    public override void LoadData(BuildingSaveData data)
    {
        var save = data as CityHallBuildingSaveData;
        if (save == null) return;

        RelationWithGovernment = save.relationWithGovernment;
        DaysLeftToReceiveGovHelp = save.daysLeftToReceiveGovHelp;
        DaysLeftToSendArmyMaterials = save.daysLeftToSendArmyMaterials;
        IsMaterialsSent = save.isMaterialsSent;
        BuildingIsSelectable = save.buildingIsSelectable;
        _amountOfHelpSent = save.amountOfHelpSent;
        _turnsToCreateNewUnit = save.turnsToCreateNewUnit;
        _turnsToCreateNewActionPoints = save.turnsToCreateNewActionPoints;
        _isWorking = save.isWorking;
        _isCreatingActionPoints = save.isCreatingActionPoints;
        TurnsToRepair = save.turnsToRepair;
        SetState(save.currentState);
        
        if (save.buildingIsSelectable)
            RestoreOriginalMaterials();
        else
            SetGreyMaterials();
    }
}