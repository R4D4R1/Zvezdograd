using System;
using System.Linq;
using UnityEngine;

public class FactoryBuilding : RepairableBuilding
{
    [Header("FACTORY SETTINGS")]
    [SerializeField] private int _peopleToCreateReadyMaterials;
    public int PeopleToCreateReadyMaterials => _peopleToCreateReadyMaterials;

    [SerializeField] private int _peopleToCreateArmyMaterials;
    public int PeopleToCreateArmyMaterials => _peopleToCreateArmyMaterials;

    [SerializeField] private int _turnsToCreateReadyMaterialsOriginal;
    public int TurnsToCreateReadyMaterialsOriginal => _turnsToCreateReadyMaterialsOriginal;

    [SerializeField] private int _turnsToCreateArmyMaterialsOriginal;
    public int TurnsToCreateArmyMaterialsOriginal => _turnsToCreateArmyMaterialsOriginal;

    [SerializeField] private int _turnsToRestFromReadyMaterialsJob;
    public int TurnsToRestFromReadyMaterialsJob => _turnsToRestFromReadyMaterialsJob;

    [SerializeField] private int _turnsToRestFromArmyMaterialsJob;
    public int TurnsToRestFromArmyMaterialsJob => _turnsToRestFromArmyMaterialsJob;

    [SerializeField] private int _rawMaterialsToCreateReadyMaterials;
    public int RawMaterialsToCreateReadyMaterials => _rawMaterialsToCreateReadyMaterials;

    [SerializeField] private int _readyMaterialsGet;
    public int ReadyMaterialsGet => _readyMaterialsGet;

    [SerializeField] private int _rawMaterialsToCreateArmyMaterials;
    public int RawMaterialsToCreateArmyMaterials => _rawMaterialsToCreateArmyMaterials;


    // SAVE DATA
    public int TurnsToCreateArmyMaterials { get; private set; }
    public int TurnsToCreateReadyMaterials { get; private set; }
    private bool _isWorking;
    private bool _isCreatingReadyMaterials;
    private int _turnsToWork;

    // TO DO - заменить синглтон

    protected override void Start()
    {
        base.Start();
        _controllersManager.TimeController.OnNextTurnBtnPressed += UpdateAmountOfTurnsNeededToDoSMTH;

        UpdateAmountOfTurnsNeededToDoSMTH();
    }

    private void UpdateAmountOfTurnsNeededToDoSMTH()
    {
        TurnsToCreateReadyMaterials = UpdateAmountOfTurnsNeededToDoSMTH(TurnsToCreateReadyMaterialsOriginal);
        TurnsToCreateArmyMaterials = UpdateAmountOfTurnsNeededToDoSMTH(TurnsToCreateArmyMaterialsOriginal);
    }

    protected override void TryTurnOnBuilding()
    {
        if (!BuildingIsSelectable)
        {
            if (!_isWorking)
            {
                TurnsToRepair--;

                if (TurnsToRepair == 0)
                {
                    BuildingIsSelectable = true;
                    CurrentState = State.Intact;

                    RestoreOriginalMaterials();
                }
            }
            if (_isWorking)
            {
                _turnsToWork--;

                if (_turnsToWork == 0)
                {
                    if (_isCreatingReadyMaterials)
                    {
                        _resourceViewModel.ModifyResource(ResourceModel.ResourceType.ReadyMaterials, ReadyMaterialsGet);
                    }
                    else
                    {
                        _controllersManager.BuildingController.GetCityHallBuilding().ArmyMaterialsSent();
                    }

                    BuildingIsSelectable = true;
                    _isWorking = false;
                    RestoreOriginalMaterials();
                }
            }
        }
    }

    public void CreateReadyMaterials()
    {
        _controllersManager.PeopleUnitsController.AssignUnitsToTask(PeopleToCreateReadyMaterials, TurnsToCreateReadyMaterials, TurnsToRestFromReadyMaterialsJob);
        _resourceViewModel.ModifyResource(ResourceModel.ResourceType.RawMaterials, -RawMaterialsToCreateReadyMaterials);

        BuildingIsSelectable = false;
        _isWorking = true;
        _isCreatingReadyMaterials = true;
        _turnsToWork = TurnsToCreateReadyMaterials;

        SetGreyMaterials();
    }

    public void CreateArmyMaterials()
    {
        _controllersManager.PeopleUnitsController.AssignUnitsToTask(PeopleToCreateArmyMaterials, TurnsToCreateArmyMaterials, TurnsToRestFromArmyMaterialsJob);
        _resourceViewModel.ModifyResource(ResourceModel.ResourceType.RawMaterials, -RawMaterialsToCreateArmyMaterials);

        BuildingIsSelectable = false;
        _isWorking = true;
        _isCreatingReadyMaterials = false;
        _turnsToWork = TurnsToCreateArmyMaterials;

        SetGreyMaterials();
    }
}
