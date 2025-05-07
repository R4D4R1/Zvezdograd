using UnityEngine;
using UniRx;
using System.Collections.Generic;

public class RepairableBuilding : ChangeMaterialsBuilding, ISaveableBuilding
{
    [Header("REPAIRABLE SETTINGS")]
    [SerializeField] private RepairableBuildingConfig repairableConfig;
    public RepairableBuildingConfig RepairableBuildingConfig => repairableConfig;

    [Header("Smoke Effect")]
    [SerializeField] private List<GameObject> smokeEffects = new();

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

            if (TurnsToRepair <= 0)
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
        UpdateSmokeEffect();
    }

    private void UpdateSmokeEffect()
    {
        if (smokeEffects != null)
        {
            bool shouldShowSmoke = _currentState == State.Damaged || _currentState == State.Repairing;
            foreach ( var smokeEffect in smokeEffects)
                smokeEffect.SetActive(shouldShowSmoke);
        }
        else
        {
            Debug.LogWarning("Smoke effect is not assigned in the inspector.");
        }
    }

    public int BuildingID => base.BuildingId;

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
