using System;
using UnityEngine;

public class FactoryBuilding : RepairableBuilding
{
    [field: SerializeField] public int PeopleToCreateReadyMaterials { get; private set; }
    [field: SerializeField] public int PeopleToCreateArmyMaterials { get; private set; }
    [field: SerializeField] public int TurnsToCreateReadyMaterials { get; private set; }
    [field: SerializeField] public int TurnsToCreateArmyMaterials { get; private set; }
    [field: SerializeField] public int TurnsToRestFromReadyMaterialsJob { get; private set; }
    [field: SerializeField] public int TurnsToRestFromArmyMaterialsJob { get; private set; }
    [field: SerializeField] public int RawMaterialsToCreateReadyMaterials { get; private set; }
    [field: SerializeField] public int ReadyMaterialsGet { get; private set; }
    [field: SerializeField] public int RawMaterialsToCreateArmyMaterials { get; private set; }

    private bool _isWorking;
    private bool _isCreatingReadyMaterials;
    private int _turnsToWork;

    protected override void TryTurnOnBuilding()
    {
        if (!BuildingIsSelactable)
        {
            if (!_isWorking)
            {
                _turnsToRepair--;
                if (_turnsToRepair == 0)
                {
                    BuildingIsSelactable = true;

                    ControllersManager.Instance.resourceController.AddOrRemoveReadyMaterials(ReadyMaterialsGet);

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
                        CityHallBuilding.Instance.AddRelationWithGov(2);
                        CityHallBuilding.Instance.ArmyMaterialsSent();
                        Debug.Log("Army Materials Created");
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

        ReplaceMaterialsWithGrey();
    }

    public void CreateArmyMaterials()
    {
        ControllersManager.Instance.peopleUnitsController.AssignUnitsToTask(PeopleToCreateArmyMaterials, TurnsToCreateArmyMaterials, TurnsToRestFromArmyMaterialsJob);
        
        BuildingIsSelactable = false;
        _isWorking = true;
        _isCreatingReadyMaterials = false;
        _turnsToWork = TurnsToCreateArmyMaterials;

        ReplaceMaterialsWithGrey();
    }
}
