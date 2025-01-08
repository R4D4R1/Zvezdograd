using System;
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
        get => state;
        set
        {
            if (state != value)
            {
                state = value;
                UpdateBuildingModel();
                OnStateChanged?.Invoke();
            }
        }
    }

    public BuildingType Type => buildingType; // Public getter for buildingType

    private GameObject _intactBuildingModel;
    private GameObject _damagedBuildingModel;

    [field: SerializeField] public string DamagedBuildingNameText { get; private set; }
    [field: SerializeField] public string DamagedDescriptionText { get; private set; }
    [field: SerializeField] public int BuildingMaterialsToRepair { get; private set; }
    [field: SerializeField] public int PeopleToRepair { get; private set; }
    [field: SerializeField] public int TurnsToRepair { get; private set; }
    [field: SerializeField] public int TurnsToRestFromRepair { get; private set; }
    
    [SerializeField] protected State state;
    [SerializeField] protected BuildingType buildingType;
    [SerializeField] protected Material originalMaterial;
    [SerializeField] protected Material greyMaterial;

    public event Action OnStateChanged;

    protected int _turnsToWork = 0;


    public void InitBuilding()
    {
        FindBuildingModels();
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += TryTurnOnBuilding;
        _turnsToWork = TurnsToRepair;

        UpdateBuildingModel();
    }


    private void TryTurnOnBuilding()
    {
        _turnsToWork--;
        if (_turnsToWork == 0)
        {
            BuildingIsActive = true;
        }

        if (BuildingIsActive)
        {
            var meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.material = originalMaterial;
            }
            else
            {
                Debug.LogWarning("MeshRenderer not found on the first child.");
            }
        }
    }

    public void RepairBuilding()
    {
        if (state == State.Damaged)
        {
            CurrentState = State.Intact;

            ControllersManager.Instance.peopleUnitsController.AssignUnitsToTask(PeopleToRepair, TurnsToRepair, TurnsToRestFromRepair);
            ControllersManager.Instance.resourceController.AddOrRemoveReadyMaterials(-BuildingMaterialsToRepair);

            _turnsToWork = TurnsToRepair;

            BuildingIsActive = false;

            var meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.material = greyMaterial;
            }
            else
            {
                Debug.LogWarning("MeshRenderer not found on the first child.");
            }
        }
    }

    public void BombBuilding()
    {
        if (state == State.Intact)
        {
            CurrentState = State.Damaged;
            //Debug.Log(gameObject.name + " BOMBED!");
        }
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
            _intactBuildingModel.SetActive(state == State.Intact);
            _damagedBuildingModel.SetActive(state == State.Damaged);
        }
    }
}