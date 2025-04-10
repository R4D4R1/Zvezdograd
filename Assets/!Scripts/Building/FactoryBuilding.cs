using System.Threading.Tasks;
using UnityEngine;
using UniRx;
using UnityEngine.Serialization;

public class FactoryBuilding : RepairableBuilding,ISaveableBuilding
{
    [FormerlySerializedAs("_factoryConfig")]
    [Header("FACTORY CONFIG")]
    [SerializeField] private FactoryBuildingConfig factoryConfig;
    public FactoryBuildingConfig FactoryBuildingConfig => factoryConfig;

    public int TurnsToCreateArmyMaterials { get; private set; }
    public int TurnsToCreateReadyMaterials { get; private set; }

    private bool _isWorking;
    private bool _isCreatingReadyMaterials;
    private int _turnsToWork;

    public override void Init()
    {
        base.Init();

        TimeController.OnNextTurnBtnClickBetween
            .Subscribe(_ => UpdateAmountOfTurnsNeededToDoSMTH())
            .AddTo(this);

        UpdateAmountOfTurnsNeededToDoSMTH();
    }

    private void UpdateAmountOfTurnsNeededToDoSMTH()
    {
        TurnsToCreateReadyMaterials = UpdateAmountOfTurnsNeededToDoSmth(factoryConfig.TurnsToCreateReadyMaterialsOriginal);
        TurnsToCreateArmyMaterials = UpdateAmountOfTurnsNeededToDoSmth(factoryConfig.TurnsToCreateArmyMaterialsOriginal);
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
                        ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.ReadyMaterials, factoryConfig.ReadyMaterialsGet));
                    }
                    else
                    {
                        BuildingController.GetCityHallBuilding().ArmyMaterialsSent();
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
        PeopleUnitsController.AssignUnitsToTask(
            factoryConfig.PeopleToCreateReadyMaterials,
            TurnsToCreateReadyMaterials,
            factoryConfig.TurnsToRestFromReadyMaterials
        );
        ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.RawMaterials, -factoryConfig.RawMaterialsToCreateReadyMaterials));

        BuildingIsSelectable = false;
        _isWorking = true;
        _isCreatingReadyMaterials = true;
        _turnsToWork = TurnsToCreateReadyMaterials;

        SetGreyMaterials();
    }

    public void CreateArmyMaterials()
    {
        PeopleUnitsController.AssignUnitsToTask(
            factoryConfig.PeopleToCreateArmyMaterials,
            TurnsToCreateArmyMaterials,
            factoryConfig.TurnsToRestFromArmyMaterials
        );
        ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.RawMaterials, -factoryConfig.RawMaterialsToCreateArmyMaterials));

        BuildingIsSelectable = false;
        _isWorking = true;
        _isCreatingReadyMaterials = false;
        _turnsToWork = TurnsToCreateArmyMaterials;
        
        SetGreyMaterials();
    }
    
    public new int BuildingId => base.BuildingId;
    
    public override BuildingSaveData GetSaveData()
    {
        return new FactoryBuildingSaveData
        {
            buildingId = BuildingId,
            buildingIsSelectable = BuildingIsSelectable,
            turnsToCreateArmyMaterials = TurnsToCreateArmyMaterials,
            turnsToCreateReadyMaterials = TurnsToCreateReadyMaterials,
            turnsToWork = _turnsToWork,
            isWorking = _isWorking,
            turnsToRepair = TurnsToRepair,
            currentState = CurrentState
        };
    }

    public override void LoadFromSaveData(BuildingSaveData data)
    {
        var save = data as FactoryBuildingSaveData;
        if (save == null) return;

        BuildingIsSelectable = save.buildingIsSelectable;
        TurnsToCreateArmyMaterials = save.turnsToCreateArmyMaterials;
        TurnsToCreateReadyMaterials = save.turnsToCreateReadyMaterials;
        _turnsToWork = save.turnsToWork;
        _isWorking = save.isWorking;
        TurnsToRepair = save.turnsToRepair;
        CurrentState = save.currentState;
        
        if (save.buildingIsSelectable)
            RestoreOriginalMaterials();
        else
            SetGreyMaterials();
    }
}
