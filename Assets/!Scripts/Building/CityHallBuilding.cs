using UnityEngine;
using UniRx;
using UnityEngine.Serialization;

public class CityHallBuilding : RepairableBuilding
{
    [FormerlySerializedAs("_cityHallConfig")]
    [Header("CITY HALL SETTINGS")]
    [SerializeField] private CityHallBuildingConfig cityHallConfig;

    public int ReadyMaterialsToCreateNewPeopleUnit { get; private set; }
    public int RelationWithGoverment { get; private set; }
    public int DaysLeftToRecieveGovHelp { get; private set; }
    public int DaysLeftToSendArmyMaterials { get; private set; }
    public bool IsMaterialsSent { get; private set; }
    
    private int _amountOfHelpSent = 0;
    private int _turnsToCreateNewUnit;
    private bool _isWorking = false;

    public readonly Subject<Unit> OnCityHallUnitCreated = new();

    public override void Init()
    {
        base.Init();

        _controllersManager.TimeController.OnNextDayEvent
            .Subscribe(_ => OnNextDayEvent())
            .AddTo(this);

        _controllersManager.TimeController.OnNextTurnBtnClickBetween
            .Subscribe(_ => CheckIfCreatedNewUnit())
            .AddTo(this);

        ReadyMaterialsToCreateNewPeopleUnit = cityHallConfig.ReadyMaterialsToCreateNewPeopleUnit;
        RelationWithGoverment = cityHallConfig.RelationWithGoverment;
        DaysLeftToRecieveGovHelp = cityHallConfig.DaysLeftToRecieveGovHelpOriginal;
        DaysLeftToSendArmyMaterials = cityHallConfig.DaysLeftToSendArmyMaterialsOriginal;
    }

    private void CheckIfCreatedNewUnit()
    {
        if (_isWorking)
        {
            _turnsToCreateNewUnit--;
            if (_turnsToCreateNewUnit == 0)
            {
                OnCityHallUnitCreated.OnNext(Unit.Default);
                _isWorking = false;
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
        if (--DaysLeftToRecieveGovHelp <= 0)
        {
            DaysLeftToRecieveGovHelp = cityHallConfig.DaysLeftToRecieveGovHelpOriginal;
            ReceiveHelpFromGov();
        }
    }

    public bool DayPassed()
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
            _controllersManager.PopUpsController.FactoryPopUp.UpdateCreateArmyButtonState();
            IsMaterialsSent = false;
            return false;
        }

        if (RelationWithGoverment > 1)
        {
            RelationWithGoverment -= 2;
            Debug.Log("DID NOT SEND ARMY MATERIALS");
            return true;
        }

        Debug.Log("GAME OVER");
        return false;
    }

    private void ReceiveHelpFromGov()
    {
        int foodAmount = RelationWithGoverment < 4 ? 2 : RelationWithGoverment < 8 ? 3 : 4;
        int medicineAmount = RelationWithGoverment < 4 ? 1 : 2;

        _resourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Provision, foodAmount));
        _resourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Medicine, medicineAmount));
    }

    public void ArmyMaterialsSent()
    {
        _amountOfHelpSent++;
        IsMaterialsSent = true;
        ModifyRelationWithGov(2);

        if (_amountOfHelpSent >= cityHallConfig.AmountOfHelpNeededToSend)
        {
            _controllersManager.MainGameController.GameWin();
        }
    }

    public void NewUnitStartedCreating()
    {
        _isWorking = true;
        _turnsToCreateNewUnit = cityHallConfig.TurnsToCreateNewUnitOriginal;
        _resourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.ReadyMaterials, -ReadyMaterialsToCreateNewPeopleUnit));
    }

    public void ModifyRelationWithGov(int amount)
    {
        RelationWithGoverment = Mathf.Clamp(RelationWithGoverment + amount, 1, 10);
    }
}
