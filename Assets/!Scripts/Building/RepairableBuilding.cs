using System;
using System.Collections.Generic;
using UnityEngine;

public class RepairableBuilding : BuildingDependingOnStability
{
    [SerializeField] private RepairableBuildingConfig _repairabelConfig;

    private List<Material[]> _originalMaterials = new List<Material[]>();
    public int TurnsToRepair { get;protected set; }

    public string DamagedBuildingNameText => _repairabelConfig.DamagedBuildingNameText;
    public string DamagedDescriptionText => _repairabelConfig.DamagedDescriptionText;
    public int BuildingMaterialsToRepair => _repairabelConfig.BuildingMaterialsToRepair;
    public int PeopleToRepair => _repairabelConfig.PeopleToRepair;
    public int TurnsToRepairOriginal => _repairabelConfig.TurnsToRepairOriginal;
    public int TurnsToRestFromRepair => _repairabelConfig.TurnsToRestFromRepair;
    public State CurrentState;
    public BuildingType Type => _repairabelConfig.BuildingType;
    public Material GreyMaterial => _repairabelConfig.GreyMaterial;

    //public event Action OnStateChanged;

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

    public void InitBuilding()
    {
        CurrentState = _repairabelConfig.State;
        FindBuildingModels();
        SaveOriginalMaterials();
        _controllersManager.TimeController.OnNextTurnBtnPressed += TryTurnOnBuilding;
        _controllersManager.TimeController.OnNextTurnBtnPressed += UpdateAmountOfTurnsNeededToDoSMTH;

        UpdateBuildingModel();
        UpdateAmountOfTurnsNeededToDoSMTH();
    }

    private void OnDestroy()
    {
        _controllersManager.TimeController.OnNextTurnBtnPressed -= TryTurnOnBuilding;
        _controllersManager.TimeController.OnNextTurnBtnPressed -= UpdateAmountOfTurnsNeededToDoSMTH;
    }

    private void UpdateAmountOfTurnsNeededToDoSMTH()
    {
        if (CurrentState != State.Repairing)
            TurnsToRepair = UpdateAmountOfTurnsNeededToDoSMTH(TurnsToRepairOriginal);
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
            _controllersManager.PeopleUnitsController.AssignUnitsToTask(PeopleToRepair, TurnsToRepair, TurnsToRestFromRepair);
            _resourceViewModel.ModifyResource(ResourceModel.ResourceType.ReadyMaterials, -BuildingMaterialsToRepair);

            BuildingIsSelectable = false;
            SetGreyMaterials();
            CurrentState = State.Repairing;
        }
    }

    public void BombBuilding()
    {
        if (CurrentState == State.Intact)
        {
            Debug.Log(gameObject.name);
            BuildingIsSelectable = true;
            CurrentState = State.Damaged;
            UpdateBuildingModel();
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
                greyMaterials[i] = GreyMaterial;
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
