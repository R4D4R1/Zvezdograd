using System.Threading.Tasks;
using UnityEngine;
using UniRx;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using Unit = UniRx.Unit;

public class CityHallBuilding : RepairableBuilding
{
    [FormerlySerializedAs("_cityHallConfig")]
    [Header("CITY HALL SETTINGS")]
    [SerializeField] private CityHallBuildingConfig cityHallConfig;

    public int ReadyMaterialsToCreateNewPeopleUnit { get; private set; }
    public int RelationWithGovernment { get; private set; }
    public int DaysLeftToReceiveGovHelp { get; private set; }
    public int DaysLeftToSendArmyMaterials { get; private set; }
    public bool IsMaterialsSent { get; private set; }
    
    private int _amountOfHelpSent;
    private int _turnsToCreateNewUnit;
    private int _turnsToCreateNewActionPoints;
    private bool _isWorking;

    public readonly Subject<Unit> OnCityHallUnitCreated = new();
    public readonly Subject<int> OnNewActionPointsStartedCreating = new();
    public readonly Subject<Unit> OnNewActionPointsCreated = new();
    private bool _isCreatingActionPoints;

    public override void Init()
    {
        base.Init();

        TimeController.OnNextDayEvent
            .Subscribe(_ => OnNextDayEvent())
            .AddTo(this);

        TimeController.OnNextTurnBtnClickBetween
            .Subscribe(_ => OnNextTurnEvent())
            .AddTo(this);

        ReadyMaterialsToCreateNewPeopleUnit = cityHallConfig.ReadyMaterialsToCreateNewPeopleUnit;
        RelationWithGovernment = cityHallConfig.RelationWithGovernment;
        DaysLeftToReceiveGovHelp = cityHallConfig.DaysLeftToReceiveGovHelpOriginal;
        DaysLeftToSendArmyMaterials = cityHallConfig.DaysLeftToSendArmyMaterialsOriginal;
        _isWorking = false;
        _isCreatingActionPoints = false;
        _amountOfHelpSent = 0;
    }
    
    private void OnNextTurnEvent()
    {
        CheckIfCreatedNewUnit();
    }
    
    private void CheckIfCreatedNewUnit()
    {
        if (_isWorking)
        {
            if (!_isCreatingActionPoints)
            {
                _turnsToCreateNewUnit--;
                Debug.Log(_turnsToCreateNewUnit);

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
            PopUpsController.FactoryPopUp.UpdateCreateArmyButtonState();
            IsMaterialsSent = false;
            return false;
        }

        if (RelationWithGovernment > 1)
        {
            RelationWithGovernment -= 2;
            Debug.Log("DID NOT SEND ARMY MATERIALS");
            return true;
        }

        Debug.Log("GAME OVER");
        return false;
    }

    private void ReceiveHelpFromGov()
    {
        var foodAmount = RelationWithGovernment < 4 ? 2 : RelationWithGovernment < 9 ? 3 : 4;
        var medicineAmount = RelationWithGovernment < 4 ? 1 : 2;

        ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Provision, foodAmount));
        ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Medicine, medicineAmount));
    }

    public void ArmyMaterialsSent()
    {
        _amountOfHelpSent++;
        IsMaterialsSent = true;
        ModifyRelationWithGov(2);

        if (_amountOfHelpSent >= cityHallConfig.AmountOfHelpNeededToSend)
        {
            MainGameController.GameWin();
        }
    }

    public void NewUnitStartedCreating()
    {
        _isWorking = true;
        _turnsToCreateNewUnit = cityHallConfig.TurnsToCreateNewUnitOriginal;
        ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.ReadyMaterials, -ReadyMaterialsToCreateNewPeopleUnit));
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
}
