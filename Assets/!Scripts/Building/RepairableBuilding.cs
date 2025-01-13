using System;
using System.Collections.Generic;
using UnityEngine;

public class RepairableBuilding : SelectableBuilding
{
    public enum State
    {
        Intact,
        Damaged
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

    public BuildingType Type => _buildingType; // Public getter for buildingType

    [field: SerializeField] public string DamagedBuildingNameText { get; private set; }
    [field: SerializeField] public string DamagedDescriptionText { get; private set; }
    [field: SerializeField] public int BuildingMaterialsToRepair { get; private set; }
    [field: SerializeField] public int PeopleToRepair { get; private set; }
    [field: SerializeField] public int TurnsToRepair { get; private set; }
    [field: SerializeField] public int TurnsToRestFromRepair { get; private set; }

    [SerializeField] protected State _state;
    [SerializeField] protected BuildingType _buildingType;
    [SerializeField] protected Material _greyMaterial;

    private List<Material[]> _originalMaterials = new List<Material[]>(); // Список для хранения оригинальных материалов

    public event Action OnStateChanged;

    protected int _turnsToWork = 0;

    public void InitBuilding()
    {
        FindBuildingModels();
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += TryTurnOnBuilding;
        _turnsToWork = TurnsToRepair;

        UpdateBuildingModel();
    }

    protected virtual void TryTurnOnBuilding()
    {
        if (!BuildingIsActive)
        {
            _turnsToWork--;
            if (_turnsToWork == 0)
            {
                BuildingIsActive = true;
                RestoreOriginalMaterials();
            }
        }
    }

    public void RepairBuilding()
    {
        if (_state == State.Damaged)
        {
            CurrentState = State.Intact;

            ControllersManager.Instance.peopleUnitsController.AssignUnitsToTask(PeopleToRepair, TurnsToRepair, TurnsToRestFromRepair);
            ControllersManager.Instance.resourceController.AddOrRemoveReadyMaterials(-BuildingMaterialsToRepair);

            _turnsToWork = TurnsToRepair;
            BuildingIsActive = false;

            ReplaceMaterialsWithGrey();
        }
    }

    public void BombBuilding()
    {
        if (_state == State.Intact)
        {
            CurrentState = State.Damaged;
        }
    }

    protected void ReplaceMaterialsWithGrey()
    {
        var renderers = GetComponentsInChildren<MeshRenderer>();
        _originalMaterials.Clear(); // Очищаем список на случай повторного вызова

        foreach (var renderer in renderers)
        {
            // Сохраняем оригинальные материалы
            _originalMaterials.Add(renderer.materials);

            // Заменяем материалы на серый
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
                Debug.Log("Return material");
                renderers[i].materials = _originalMaterials[i];
            }
            else
            {
                //Debug.LogWarning("Mismatch between renderers and original materials list.");
            }
        }
        _originalMaterials.Clear(); // Очищаем список после восстановления
    }

    private void FindBuildingModels()
    {
        // Find the intact and damaged building models based on the components
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
            _intactBuildingModel.SetActive(_state == State.Intact);
            _damagedBuildingModel.SetActive(_state == State.Damaged);
        }

        //if (BuildingIsActive)
        //{
        //    RestoreOriginalMaterials();
        //}
        //else
        //{
        //    ReplaceMaterialsWithGrey();
        //}
    }

    private GameObject _intactBuildingModel;
    private GameObject _damagedBuildingModel;
}
