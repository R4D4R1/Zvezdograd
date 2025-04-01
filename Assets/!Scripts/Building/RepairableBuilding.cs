using System.Threading.Tasks;
using UnityEngine;
using UniRx;
using UnityEngine.Serialization;

public class RepairableBuilding : ChangeMaterialsBuiliding
{
    [FormerlySerializedAs("_repairableConfig")]
    [Header("REPAIRABLE SETTINGS")]
    [SerializeField] private RepairableBuildingConfig repairableConfig;

    public string DamagedBuildingLabel { get; private set; }
    public string DamagedBuildingDescription { get; private set; }
    public int PeopleToRepair { get; private set; }
    public int BuildingMaterialsToRepair { get; private set; }
    public int TurnsToRestFromRepair { get; private set; }
    
    protected int TurnsToRepair { get; set; }

    public State CurrentState;
    public BuildingType Type => repairableConfig.BuildingType;

    public enum State
    {
        Intact,
        Damaged,
        Repairing
    }

    public enum BuildingType
    {
        LivingArea,
        Hospital,
        FoodTrucks,
        Factory,
        CityHall
    }

    public override void Init()
    {
        base.Init();

        CurrentState = repairableConfig.State;

        FindBuildingModels();

        TimeController.OnNextTurnBtnClickBetween
            .Subscribe(_ => TryTurnOnBuilding())
            .AddTo(this);
        TimeController.OnNextTurnBtnClickBetween
            .Subscribe(_ => UpdateAmountOfTurnsNeededToDoSMTH())
            .AddTo(this);

        UpdateBuildingModel();
        UpdateAmountOfTurnsNeededToDoSMTH();

        PeopleToRepair = repairableConfig.PeopleToRepair;
        BuildingMaterialsToRepair = repairableConfig.BuildingMaterialsToRepair;
        TurnsToRestFromRepair = repairableConfig.TurnsToRestFromRepair;
        DamagedBuildingLabel = repairableConfig.DamagedBuildingLabel;
        DamagedBuildingDescription = repairableConfig.DamagedBuildingDescription;
    }

    private void UpdateAmountOfTurnsNeededToDoSMTH()
    {
        if (CurrentState != State.Repairing)
            TurnsToRepair = UpdateAmountOfTurnsNeededToDoSmth(repairableConfig.TurnsToRepairOriginal);
    }

    protected virtual void TryTurnOnBuilding()
    {
        if (CurrentState == State.Repairing)
        {
            TurnsToRepair--;

            if (TurnsToRepair == 0)
            {
                buildingIsSelectable = true;
                RestoreOriginalMaterials();
                CurrentState = State.Intact;
                UpdateBuildingModel();
            }
        }
    }

    public void RepairBuilding()
    {
        if (CurrentState == State.Damaged)
        {
            PeopleUnitsController.AssignUnitsToTask(repairableConfig.PeopleToRepair, TurnsToRepair, repairableConfig.TurnsToRestFromRepair);
            ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.ReadyMaterials, -repairableConfig.BuildingMaterialsToRepair));

            buildingIsSelectable = false;
            SetGreyMaterials();
            CurrentState = State.Repairing;
        }
    }

    public void BombBuilding()
    {
        if (CurrentState == State.Intact)
        {
            buildingIsSelectable = true;
            CurrentState = State.Damaged;
            UpdateBuildingModel();
        }
    }

    private void FindBuildingModels()
    {
        IntactBuilding intactComponent = GetComponentInChildren<IntactBuilding>();
        DamagedBuilding damagedComponent = GetComponentInChildren<DamagedBuilding>();

        if (intactComponent)
        {
            _intactBuildingModel = intactComponent.gameObject;
        }
        else
        {
            Debug.LogError("IntactBuilding component not found on any child object.");
        }

        if (damagedComponent)
        {
            _damagedBuildingModel = damagedComponent.gameObject;
        }
        else
        {
            Debug.LogError("DamagedBuilding component not found on any child object.");
        }
    }

    private void UpdateBuildingModel()
    {
        SetModelActive(_intactBuildingModel, CurrentState == State.Intact);
        SetModelActive(_damagedBuildingModel, CurrentState != State.Intact);
    }

    private void SetModelActive(GameObject model, bool isActive)
    {
        if (model)
            model.SetActive(isActive);
    }

    private GameObject _intactBuildingModel;
    private GameObject _damagedBuildingModel;
}
