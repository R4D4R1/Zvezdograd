using System;
using UnityEngine;

public class RepairableBuilding : MonoBehaviour
{
    public enum State
    {
        Intact,
        Damaged
    }

    [SerializeField] private State state;

    [field: SerializeField] public string BuildingNameText { get; protected set; }
    [field: SerializeField] public string DescriptionText { get; protected set; }
    [field: SerializeField] public string DamagedBuildingNameText { get; private set; }
    [field: SerializeField] public string DamagedDescriptionText { get; private set; }

    private GameObject intactBuildingModel;
    private GameObject damagedBuildingModel;
    private string originalBuildingNameText;
    private string originalDescriptionText;

    public event Action OnStateChanged;

    public State CurrentState
    {
        get => state;
        set
        {
            if (state != value)
            {
                state = value;
                UpdateBuildingModel();
                UpdateBuildingText();
                OnStateChanged?.Invoke();
            }
        }
    }

    private void Awake()
    {
        FindBuildingModels();
        originalBuildingNameText = BuildingNameText;
        originalDescriptionText = DescriptionText;
        UpdateBuildingModel();
        UpdateBuildingText();
    }

    private void Start()
    {
        // Ensure the building model is set correctly at the start
        UpdateBuildingModel();
    }

    public void RepairBuilding()
    {
        if (state == State.Damaged)
        {
            CurrentState = State.Intact;
            Debug.Log("Building repaired and is now intact.");
        }
    }

    public void BombBuilding()
    {
        if (state == State.Intact)
        {
            CurrentState = State.Damaged;
            Debug.Log(gameObject.name + " BOMBED!");
        }
    }

    private void FindBuildingModels()
    {
        // Find the intact and damaged building models based on the components
        IntactBuilding intactComponent = GetComponentInChildren<IntactBuilding>();
        DamagedBuilding damagedComponent = GetComponentInChildren<DamagedBuilding>();

        if (intactComponent != null)
        {
            intactBuildingModel = intactComponent.gameObject;
        }
        else
        {
            Debug.LogError("IntactBuilding component not found on any child object.");
        }

        if (damagedComponent != null)
        {
            damagedBuildingModel = damagedComponent.gameObject;
        }
        else
        {
            Debug.LogError("DamagedBuilding component not found on any child object.");
        }
    }

    private void UpdateBuildingModel()
    {
        if (intactBuildingModel != null && damagedBuildingModel != null)
        {
            intactBuildingModel.SetActive(state == State.Intact);
            damagedBuildingModel.SetActive(state == State.Damaged);
        }
    }

    private void UpdateBuildingText()
    {
        if (state == State.Damaged)
        {
            BuildingNameText = DamagedBuildingNameText;
            DescriptionText = DamagedDescriptionText;
        }
        else if (state == State.Intact)
        {
            BuildingNameText = originalBuildingNameText;
            DescriptionText = originalDescriptionText;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event to prevent memory leaks
        OnStateChanged -= UpdateBuildingText;
    }
}
