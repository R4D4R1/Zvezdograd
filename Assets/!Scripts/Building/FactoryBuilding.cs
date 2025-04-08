using System.Threading.Tasks;
using UnityEngine;
using UniRx;
using UnityEngine.Serialization;

public class FactoryBuilding : RepairableBuilding
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
        if (!buildingIsSelectable)
        {
            if (!_isWorking)
            {
                TurnsToRepair--;

                if (TurnsToRepair == 0)
                {
                    buildingIsSelectable = true;
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

                    buildingIsSelectable = true;
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

        buildingIsSelectable = false;
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

        buildingIsSelectable = false;
        _isWorking = true;
        _isCreatingReadyMaterials = false;
        _turnsToWork = TurnsToCreateArmyMaterials;

        SetGreyMaterials();
    }
}
