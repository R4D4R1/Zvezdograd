using System;
using UnityEngine;

public class CityHallBuilding : RepairableBuilding
{
    [Header("CITY HALL SETTINGS")]
    [SerializeField] private int _daysLeftToSendArmyMaterialsOriginal;
    [SerializeField] private int _daysLeftToRecieveGovHelpOriginal;
    [SerializeField, Range(0, 10)] private int _amountOfHelpNeededToSend;

    [field: SerializeField] public int ReadyMaterialsToCreateNewPeopleUnit { get; private set; }
    //[field: SerializeField] public int PeopleToCreateNewPeopleUnit { get; private set; }

    [SerializeField] private int _turnsToCreateNewUnitOriginal;
    private int _turnsToCreateNewUnit;
    private bool _isWorking = false;

    [field: SerializeField, Range(0, 10)] public int RelationWithGoverment { get; private set; }
    public int DaysLeftToRecieveGovHelp { get; private set; }
    public int DaysLeftToSendArmyMaterials { get; private set; }
    public bool IsMaterialsSent { get; private set; }

    private TimeController _timeController;
    private ResourceController _resourceController;
    private int _amountOfHelpSent = 0;

    public event Action OnCityHallUnitCreated;

    private void Start()
    {
        InitializeControllers();
        InitializeTimers();
        _timeController.OnNextDayEvent += OnNextDayEvent;

        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += CheckIfCreatedNewUnit;

    }

    private void CheckIfCreatedNewUnit()
    {
        if (_isWorking)
        {
            _turnsToCreateNewUnit--;
            if( _turnsToCreateNewUnit == 0 )
            {
                OnCityHallUnitCreated?.Invoke();
                _isWorking = false;
            }
        }
    }

    private void InitializeControllers()
    {
        _timeController = ControllersManager.Instance.timeController;
        _resourceController = ControllersManager.Instance.resourceController;
    }

    private void InitializeTimers()
    {
        DaysLeftToRecieveGovHelp = _daysLeftToRecieveGovHelpOriginal;
        DaysLeftToSendArmyMaterials = _daysLeftToSendArmyMaterialsOriginal;
    }

    private void OnNextDayEvent()
    {
        ProcessGovHelp();
    }

    private void ProcessGovHelp()
    {
        if (--DaysLeftToRecieveGovHelp <= 0)
        {
            DaysLeftToRecieveGovHelp = _daysLeftToRecieveGovHelpOriginal;
            ReceiveHelpFromGov();
        }
    }

    public bool DayPassed()
    {
        if (--DaysLeftToSendArmyMaterials <= 0)
        {
            DaysLeftToSendArmyMaterials = _daysLeftToSendArmyMaterialsOriginal;
            return HandleArmyMaterials();
        }
        return false;
    }

    private bool HandleArmyMaterials()
    {
        if (IsMaterialsSent)
        {
            ControllersManager.Instance.popUpsController.FactoryPopUp.UpdateCreateArmyButtonState();
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

        _resourceController.ModifyResource(ResourceController.ResourceType.Provision, foodAmount);
        _resourceController.ModifyResource(ResourceController.ResourceType.Medicine, medicineAmount);
    }

    public void ArmyMaterialsSent()
    {
        _amountOfHelpSent++;
        IsMaterialsSent = true;
        ModifyRelationWithGov(2);

        if (_amountOfHelpSent >= _amountOfHelpNeededToSend)
        {
            ControllersManager.Instance.mainGameController.GameWin();
        }
    }

    public void NewUnitStartedCreating()
    {
        _isWorking = true;
        _turnsToCreateNewUnit = _turnsToCreateNewUnitOriginal;
        ControllersManager.Instance.resourceController.ModifyResource(ResourceController.ResourceType.ReadyMaterials, -ReadyMaterialsToCreateNewPeopleUnit);
    }

    public void ModifyRelationWithGov(int value)
    {
        RelationWithGoverment = Mathf.Clamp(RelationWithGoverment + Mathf.Abs(value), 0, 10);
    }
}
