using UnityEngine;
using UniRx;

public class RepairableBuilding : ChangeMaterialsBuilding, ISaveableBuilding
{
    [Header("REPAIRABLE SETTINGS")]
    [SerializeField] private RepairableBuildingConfig repairableConfig;
    public RepairableBuildingConfig RepairableBuildingConfig => repairableConfig;

    protected int TurnsToRepair { get; set; }

    private State _currentState;
    public State CurrentState => _currentState;

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

        FindBuildingModels();

        SetState(State.Intact);

        TimeController.OnNextTurnBtnClickBetween
            .Subscribe(_ => TryTurnOnBuilding())
            .AddTo(this);

        TimeController.OnNextTurnBtnClickBetween
            .Subscribe(_ => UpdateAmountOfTurnsNeededToDoSMTH())
            .AddTo(this);

        UpdateAmountOfTurnsNeededToDoSMTH();
    }

    private void UpdateAmountOfTurnsNeededToDoSMTH()
    {
        if (_currentState != State.Repairing)
            TurnsToRepair = UpdateAmountOfTurnsNeededToDoSmth(repairableConfig.TurnsToRepairOriginal);
    }

    protected virtual void TryTurnOnBuilding()
    {
        if (_currentState == State.Repairing)
        {
            TurnsToRepair--;

            if (TurnsToRepair == 0)
            {
                BuildingIsSelectable = true;
                RestoreOriginalMaterials();
                SetState(State.Intact);
            }
        }
    }

    public void RepairBuilding()
    {
        if (_currentState == State.Damaged)
        {
            PeopleUnitsController.AssignUnitsToTask(
                repairableConfig.PeopleToRepair,
                TurnsToRepair,
                repairableConfig.TurnsToRestFromRepair
            );

            ResourceViewModel.ModifyResourceCommand.Execute(
                (ResourceModel.ResourceType.ReadyMaterials, -repairableConfig.BuildingMaterialsToRepair)
            );

            BuildingIsSelectable = false;
            SetGreyMaterials();
            SetState(State.Repairing);
        }
    }

    public void BombBuilding()
    {
        if (_currentState == State.Intact)
        {
            BuildingIsSelectable = true;
            SetState(State.Damaged);
        }
    }

    protected void SetState(State newState)
    {
        _currentState = newState;
        UpdateBuildingModel();
    }

    private void FindBuildingModels()
    {
        IntactBuilding intactComponent = GetComponentInChildren<IntactBuilding>();
        DamagedBuilding damagedComponent = GetComponentInChildren<DamagedBuilding>();

        if (intactComponent)
            _intactBuildingModel = intactComponent.gameObject;
        else
            Debug.LogError("IntactBuilding component not found on any child object.");

        if (damagedComponent)
            _damagedBuildingModel = damagedComponent.gameObject;
        else
            Debug.LogError("DamagedBuilding component not found on any child object.");
    }

    private void UpdateBuildingModel()
    {
        SetModelActive(_intactBuildingModel, _currentState == State.Intact);
        SetModelActive(_damagedBuildingModel, _currentState != State.Intact);
    }

    private void SetModelActive(GameObject model, bool isActive)
    {
        if (model)
            model.SetActive(isActive);
    }

    private GameObject _intactBuildingModel;
    private GameObject _damagedBuildingModel;

    public new int BuildingID => base.BuildingId;

    public virtual BuildingSaveData SaveData()
    {
        return new RepairableBuildingSaveData
        {
            buildingID = BuildingID,
            buildingIsSelectable = BuildingIsSelectable,
            turnsToRepair = TurnsToRepair,
            currentState = _currentState
        };
    }

    public virtual void LoadData(BuildingSaveData data)
    {
        var save = data as RepairableBuildingSaveData;
        if (save == null) return;

        BuildingIsSelectable = save.buildingIsSelectable;
        TurnsToRepair = save.turnsToRepair;
        SetState(save.currentState);

        if (BuildingIsSelectable)
            RestoreOriginalMaterials();
        else
            SetGreyMaterials();
    }
}
