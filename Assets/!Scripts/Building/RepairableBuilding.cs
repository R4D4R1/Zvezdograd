using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class RepairableBuilding : ChangeMaterialsBuiliding
{
    [Header("REPAIRABLE SETTINGS")]
    [SerializeField] private RepairableBuildingConfig _repairableConfig;

    public int PeopleToRepair { get; private set; }
    public int BuildingMaterialsToRepair { get; private set; }
    public int TurnsToRestFromRepair { get; private set; }


    private List<Material[]> _originalMaterials = new List<Material[]>();
    public int TurnsToRepair { get;protected set; }

    public State CurrentState;
    public BuildingType Type => _repairableConfig.BuildingType;

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

        CurrentState = _repairableConfig.State;
        FindBuildingModels();

        _controllersManager.TimeController.OnNextTurnBtnClickBetween
            .Subscribe(_ => TryTurnOnBuilding())
            .AddTo(this);
        _controllersManager.TimeController.OnNextTurnBtnClickBetween
            .Subscribe(_ => UpdateAmountOfTurnsNeededToDoSMTH())
            .AddTo(this);

        UpdateBuildingModel();
        UpdateAmountOfTurnsNeededToDoSMTH();

        PeopleToRepair = _repairableConfig.PeopleToRepair;
        BuildingMaterialsToRepair = _repairableConfig.BuildingMaterialsToRepair;
        TurnsToRestFromRepair = _repairableConfig.TurnsToRestFromRepair;
    }

    private void UpdateAmountOfTurnsNeededToDoSMTH()
    {
        if (CurrentState != State.Repairing)
            TurnsToRepair = UpdateAmountOfTurnsNeededToDoSMTH(_repairableConfig.TurnsToRepairOriginal);
    }

    protected virtual void TryTurnOnBuilding()
    {
        if (CurrentState == State.Repairing)
        {
            TurnsToRepair--;

            if (TurnsToRepair == 0)
            {
                BuildingIsSelectable = true;
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
            _controllersManager.PeopleUnitsController.AssignUnitsToTask(_repairableConfig.PeopleToRepair, TurnsToRepair, _repairableConfig.TurnsToRestFromRepair);
            _resourceViewModel.ModifyResource(ResourceModel.ResourceType.ReadyMaterials, -_repairableConfig.BuildingMaterialsToRepair);

            BuildingIsSelectable = false;
            SetGreyMaterials();
            CurrentState = State.Repairing;
        }
    }

    public void BombBuilding()
    {
        if (CurrentState == State.Intact)
        {
            BuildingIsSelectable = true;
            CurrentState = State.Damaged;
            UpdateBuildingModel();
        }
    }

    private void FindBuildingModels()
    {
        IntactBuilding intactComponent = GetComponentInChildren<IntactBuilding>();
        DamagedBuilding damagedComponent = GetComponentInChildren<DamagedBuilding>();

        if (intactComponent != null)
        {
            _intactBuildingModel = intactComponent.gameObject;
        }
        else
        {
            Debug.LogError("IntactBuilding component not found on any child object.");
        }

        if (damagedComponent != null)
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
        if (model != null)
            model.SetActive(isActive);
    }

    private GameObject _intactBuildingModel;
    private GameObject _damagedBuildingModel;
}
