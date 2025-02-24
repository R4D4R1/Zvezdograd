using System;
using System.Linq;
using UnityEngine;

public class FactoryBuilding : RepairableBuilding
{
    [field: SerializeField] public int PeopleToCreateReadyMaterials { get; private set; }
    [field: SerializeField] public int PeopleToCreateArmyMaterials { get; private set; }
    [field: SerializeField] public int TurnsToCreateReadyMaterialsOriginal { get; private set; }
    [field: SerializeField] public int TurnsToCreateArmyMaterialsOriginal { get; private set; }
    [field: SerializeField] public int TurnsToRestFromReadyMaterialsJob { get; private set; }
    [field: SerializeField] public int TurnsToRestFromArmyMaterialsJob { get; private set; }
    [field: SerializeField] public int RawMaterialsToCreateReadyMaterials { get; private set; }
    [field: SerializeField] public int ReadyMaterialsGet { get; private set; }
    [field: SerializeField] public int RawMaterialsToCreateArmyMaterials { get; private set; }

    // SAVE DATA
    public int TurnsToCreateArmyMaterials { get; private set; }
    public int TurnsToCreateReadyMaterials { get; private set; }
    private bool _isWorking;
    private bool _isCreatingReadyMaterials;
    private int _turnsToWork;

    // TO DO - заменить синглтон

    private void Start()
    {
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += UpdateAmountOfTurnsNeededToDoSMTH;

        UpdateAmountOfTurnsNeededToDoSMTH();
    }

    private void UpdateAmountOfTurnsNeededToDoSMTH()
    {
        TurnsToCreateReadyMaterials = UpdateAmountOfTurnsNeededToDoSMTH(TurnsToCreateReadyMaterialsOriginal);
        TurnsToCreateArmyMaterials = UpdateAmountOfTurnsNeededToDoSMTH(TurnsToCreateArmyMaterialsOriginal);
    }

    protected override void TryTurnOnBuilding()
    {
        if (!BuildingIsSelactable)
        {
            if (!_isWorking)
            {
                TurnsToRepair--;

                if (TurnsToRepair == 0)
                {
                    BuildingIsSelactable = true;
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
                        ControllersManager.Instance.resourceController.AddOrRemoveReadyMaterials(ReadyMaterialsGet);
                    }
                    else
                    {
                        ControllersManager.Instance.buildingController.GetCityHallBuilding().AddRelationWithGov(2);
                        ControllersManager.Instance.buildingController.GetCityHallBuilding().ArmyMaterialsSent();
                    }

                    BuildingIsSelactable = true;
                    _isWorking = false;
                    RestoreOriginalMaterials();
                }
            }
        }
    }

    public void CreateReadyMaterials()
    {
        ControllersManager.Instance.peopleUnitsController.AssignUnitsToTask(PeopleToCreateReadyMaterials, TurnsToCreateReadyMaterials, TurnsToRestFromReadyMaterialsJob);
        ControllersManager.Instance.resourceController.AddOrRemoveRawMaterials(-RawMaterialsToCreateReadyMaterials);

        BuildingIsSelactable = false;
        _isWorking = true;
        _isCreatingReadyMaterials = true;
        _turnsToWork = TurnsToCreateReadyMaterials;

        SetGreyMaterials();
    }

    public void CreateArmyMaterials()
    {
        ControllersManager.Instance.peopleUnitsController.AssignUnitsToTask(PeopleToCreateArmyMaterials, TurnsToCreateArmyMaterials, TurnsToRestFromArmyMaterialsJob);
        ControllersManager.Instance.resourceController.AddOrRemoveRawMaterials(-RawMaterialsToCreateArmyMaterials);

        BuildingIsSelactable = false;
        _isWorking = true;
        _isCreatingReadyMaterials = false;
        _turnsToWork = TurnsToCreateArmyMaterials;

        SetGreyMaterials();
    }
}
