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
    [field: SerializeField] public int PeopleToRepair { get; private set; }
    [field: SerializeField] public int TurnsToRepair { get; private set; }
    [field: SerializeField] public int BuildingMaterialsToRepair { get; private set; }

    [SerializeField] private State state;
    [SerializeField] private BuildingType buildingType;

    public event Action OnStateChanged;

    private void Awake()
    {
        
    }

    public void InitBuilding()
    {
        FindBuildingModels();
        UpdateBuildingModel();
    }

    public void RepairBuilding()
    {
        if (state == State.Damaged)
        {
            CurrentState = State.Intact;
            //Debug.Log("Building repaired and is now intact.");

            //Check for materials

            if (ControllersManager.Instance.resourceController.GetBuildingMaterials() >= BuildingMaterialsToRepair)
            {
                ControllersManager.Instance.peopleUnitsController.AssignUnitsToTask(PeopleToRepair, TurnsToRepair);
                ControllersManager.Instance.resourceController.AddOrRemoveBuildingMaterials(-BuildingMaterialsToRepair);
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