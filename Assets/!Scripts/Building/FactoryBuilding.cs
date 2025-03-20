using UnityEngine;
using UniRx;

public class FactoryBuilding : RepairableBuilding
{
    [Header("FACTORY CONFIG")]
    [SerializeField] private FactoryBuildingConfig _factoryConfig;

    public int TurnsToCreateArmyMaterials { get; private set; }
    public int TurnsToCreateReadyMaterials { get; private set; }
    public int RawMaterialsToCreateReadyMaterials { get; private set; }
    public int PeopleToCreateReadyMaterials { get; private set; }
    public int RawMaterialsToCreateArmyMaterials { get; private set; }
    public int PeopleToCreateArmyMaterials { get; private set; }
    public int ReadyMaterialsGet { get; private set; }

    private bool _isWorking;
    private bool _isCreatingReadyMaterials;
    private int _turnsToWork;

    public override void Init()
    {
        base.Init();
        _controllersManager.TimeController.OnNextTurnBtnClickBetween
            .Subscribe(_ => UpdateAmountOfTurnsNeededToDoSMTH())
            .AddTo(this);

        UpdateAmountOfTurnsNeededToDoSMTH();

        RawMaterialsToCreateReadyMaterials = _factoryConfig.RawMaterialsToCreateReadyMaterials;
        RawMaterialsToCreateArmyMaterials = _factoryConfig.RawMaterialsToCreateArmyMaterials;
        PeopleToCreateReadyMaterials = _factoryConfig.PeopleToCreateReadyMaterials;
        PeopleToCreateArmyMaterials = _factoryConfig.PeopleToCreateArmyMaterials;
        ReadyMaterialsGet = _factoryConfig.ReadyMaterialsGet;
    }

    private void UpdateAmountOfTurnsNeededToDoSMTH()
    {
        TurnsToCreateReadyMaterials = UpdateAmountOfTurnsNeededToDoSMTH(_factoryConfig.TurnsToCreateReadyMaterialsOriginal);
        TurnsToCreateArmyMaterials = UpdateAmountOfTurnsNeededToDoSMTH(_factoryConfig.TurnsToCreateArmyMaterialsOriginal);
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
                        _resourceViewModel.ModifyResource(ResourceModel.ResourceType.ReadyMaterials, _factoryConfig.ReadyMaterialsGet);
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
        _controllersManager.PeopleUnitsController.AssignUnitsToTask(
            _factoryConfig.PeopleToCreateReadyMaterials,
            TurnsToCreateReadyMaterials,
            _factoryConfig.TurnsToRestFromReadyMaterials
        );
        _resourceViewModel.ModifyResource(ResourceModel.ResourceType.RawMaterials, -_factoryConfig.RawMaterialsToCreateReadyMaterials);

        BuildingIsSelectable = false;
        _isWorking = true;
        _isCreatingReadyMaterials = true;
        _turnsToWork = TurnsToCreateReadyMaterials;

        SetGreyMaterials();
    }

    public void CreateArmyMaterials()
    {
        _controllersManager.PeopleUnitsController.AssignUnitsToTask(
            _factoryConfig.PeopleToCreateArmyMaterials,
            TurnsToCreateArmyMaterials,
            _factoryConfig.TurnsToRestFromArmyMaterials
        );
        _resourceViewModel.ModifyResource(ResourceModel.ResourceType.RawMaterials, -_factoryConfig.RawMaterialsToCreateArmyMaterials);

        BuildingIsSelectable = false;
        _isWorking = true;
        _isCreatingReadyMaterials = false;
        _turnsToWork = TurnsToCreateArmyMaterials;

        SetGreyMaterials();
    }
}
