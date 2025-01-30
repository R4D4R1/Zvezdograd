using System;
using System.Collections.Generic;
using UnityEngine;

public class RepairableBuilding : BuildingDependingOnStability
{
    public enum State
    {
        Intact,
        Damaged,
        Repairing // Добавлено новое состояние
    }

    public enum BuildingType
    {
        LivingArea,
        Hospital,
        FoodTrucks,
        Factory,
        CityHall
    }

    public State CurrentState
    {
        get => _state;
        set
        {
            if (_state != value)
            {
                _state = value;
                UpdateBuildingModel();
                OnStateChanged?.Invoke();
            }
        }
    }

    public BuildingType Type => _buildingType;

    [field: SerializeField] public string DamagedBuildingNameText { get; private set; }
    [field: SerializeField] public string DamagedDescriptionText { get; private set; }
    [field: SerializeField] public int BuildingMaterialsToRepair { get; private set; }
    [field: SerializeField] public int PeopleToRepair { get; private set; }

    [SerializeField] private int TurnsToRepairOriginal;
    public int TurnsToRepair { get; private set; }

    [field: SerializeField] public int TurnsToRestFromRepair { get; private set; }

    [SerializeField] protected State _state;
    [SerializeField] protected BuildingType _buildingType;
    [SerializeField] protected Material _greyMaterial;

    private List<Material[]> _originalMaterials = new List<Material[]>();

    public event Action OnStateChanged;

    protected int _turnsToRepair = 0;

    public void InitBuilding()
    {
        FindBuildingModels();
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += TryTurnOnBuilding;
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += UpdateAmountOfTurnsNeededToDoSMTH;

        _turnsToRepair = TurnsToRepairOriginal;

        UpdateBuildingModel();
        UpdateAmountOfTurnsNeededToDoSMTH();
    }

    private void UpdateAmountOfTurnsNeededToDoSMTH()
    {
        TurnsToRepair = UpdateAmountOfTurnsNeededToDoSMTH(TurnsToRepairOriginal);
    }

    protected virtual void TryTurnOnBuilding()
    {
        if (CurrentState == State.Repairing)
        {
            _turnsToRepair--;
            Debug.Log(_turnsToRepair);

            if (_turnsToRepair == 0)
            {
                BuildingIsSelactable = true;
                RestoreOriginalMaterials();

                CurrentState = State.Intact;
            }
        }
    }

    public void RepairBuilding()
    {
        if (CurrentState == State.Damaged)
        {
            ControllersManager.Instance.peopleUnitsController.AssignUnitsToTask(PeopleToRepair, TurnsToRepair, TurnsToRestFromRepair);
            ControllersManager.Instance.resourceController.AddOrRemoveReadyMaterials(-BuildingMaterialsToRepair);

            _turnsToRepair = TurnsToRepair;
            BuildingIsSelactable = false;

            ReplaceMaterialsWithGrey();

            CurrentState = State.Repairing; // Устанавливаем состояние ремонта
        }
    }

    public void BombBuilding()
    {
        if (CurrentState == State.Intact)
        {
            BuildingIsSelactable = true;
            CurrentState = State.Damaged;
        }
    }

    protected void ReplaceMaterialsWithGrey()
    {
        var renderers = GetComponentsInChildren<MeshRenderer>();
        _originalMaterials.Clear();

        foreach (var renderer in renderers)
        {
            _originalMaterials.Add(renderer.materials);

            var greyMaterials = new Material[renderer.materials.Length];
            for (int i = 0; i < greyMaterials.Length; i++)
            {
                greyMaterials[i] = _greyMaterial;
            }
            renderer.materials = greyMaterials;
        }
    }

    protected void RestoreOriginalMaterials()
    {
        var renderers = GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            if (i < _originalMaterials.Count)
            {
                renderers[i].materials = _originalMaterials[i];
            }
        }
        _originalMaterials.Clear();
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
        if (_intactBuildingModel != null && _damagedBuildingModel != null)
        {
            _intactBuildingModel.SetActive(CurrentState == State.Intact);
            _damagedBuildingModel.SetActive(CurrentState == State.Damaged || CurrentState == State.Repairing);
        }
    }

    private GameObject _intactBuildingModel;
    private GameObject _damagedBuildingModel;
}
