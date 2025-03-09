using System;
using System.Collections.Generic;
using UnityEngine;

public class RepairableBuilding : BuildingDependingOnStability
{
    [field: SerializeField] public string DamagedBuildingNameText { get; private set; }
    [field: SerializeField] public string DamagedDescriptionText { get; private set; }
    [field: SerializeField] public int BuildingMaterialsToRepair { get; private set; }
    [field: SerializeField] public int PeopleToRepair { get; private set; }

    [SerializeField] private int TurnsToRepairOriginal;
    public int TurnsToRepair;

    //protected int _turnsToRepair = 0;

    [field: SerializeField] public int TurnsToRestFromRepair { get; private set; }

    [SerializeField] protected State _state;
    [SerializeField] protected BuildingType _buildingType;
    [SerializeField] protected Material _greyMaterial;

    private List<Material[]> _originalMaterials = new List<Material[]>();

    public event Action OnStateChanged;
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
                if (_state == State.Repairing)
                {
                    SetGreyMaterials();
                }
            }
        }
    }

    public BuildingType Type => _buildingType;

    public void InitBuilding()
    {
        FindBuildingModels();
        SaveOriginalMaterials();
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += TryTurnOnBuilding;
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += UpdateAmountOfTurnsNeededToDoSMTH;

        //_turnsToRepair = TurnsToRepairOriginal;

        UpdateBuildingModel();
        UpdateAmountOfTurnsNeededToDoSMTH();
    }

    private void OnDestroy()
    {
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed -= TryTurnOnBuilding;
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed -= UpdateAmountOfTurnsNeededToDoSMTH;
    }

    private void UpdateAmountOfTurnsNeededToDoSMTH()
    {
        if(CurrentState!= State.Repairing)
            TurnsToRepair = UpdateAmountOfTurnsNeededToDoSMTH(TurnsToRepairOriginal);
    }

    protected virtual void TryTurnOnBuilding()
    {
        if (CurrentState == State.Repairing)
        {
            TurnsToRepair--;

            if (TurnsToRepair == 0)
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
            ControllersManager.Instance.resourceController.ModifyResource(ResourceController.ResourceType.RawMaterials, -BuildingMaterialsToRepair);

            BuildingIsSelactable = false;

            SetGreyMaterials();

            CurrentState = State.Repairing;
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

    private void SaveOriginalMaterials()
    {
        _originalMaterials.Clear();
        var renderers = GetComponentsInChildren<MeshRenderer>();

        foreach (var renderer in renderers)
        {
            _originalMaterials.Add(renderer.materials);
        }
    }

    protected void SetGreyMaterials()
    {
        var renderers = GetComponentsInChildren<MeshRenderer>();

        foreach (var renderer in renderers)
        {
            var greyMaterials = new Material[renderer.materials.Length];
            for (int i = 0; i < renderer.materials.Length; i++)
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
