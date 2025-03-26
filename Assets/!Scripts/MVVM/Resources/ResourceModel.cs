using UniRx;
using UnityEngine;
using Zenject;

public class ResourceModel
{
    public ReactiveProperty<int> Provision { get; }
    public ReactiveProperty<int> Medicine { get; }
    public ReactiveProperty<int> RawMaterials { get; }
    public ReactiveProperty<int> ReadyMaterials { get; }
    public ReactiveProperty<int> Stability { get; }
    
    public readonly int MaxProvision;
    public readonly int MaxMedicine;
    public readonly int MaxRawMaterials;
    public readonly int MaxReadyMaterials;
    public readonly int MaxStability;

    private readonly MainGameController _mainGameController;

    [Inject]
    public ResourceModel(MainGameController mainGameController, ResourcesConfig resourceConfig)
    {
        _mainGameController = mainGameController;

        // SetResourceValue(ResourceType.Provision, resourceConfig.Provision);
        // SetResourceValue(ResourceType.Medicine, resourceConfig.Medicine);
        // SetResourceValue(ResourceType.RawMaterials, resourceConfig.RawMaterials);
        // SetResourceValue(ResourceType.ReadyMaterials, resourceConfig.ReadyMaterials);
        // SetResourceValue(ResourceType.Stability, resourceConfig.Stability);
        
        Provision = new ReactiveProperty<int>(resourceConfig.Provision);
        Medicine = new ReactiveProperty<int>(resourceConfig.Medicine);
        RawMaterials = new ReactiveProperty<int>(resourceConfig.RawMaterials);
        ReadyMaterials = new ReactiveProperty<int>(resourceConfig.ReadyMaterials);
        Stability = new ReactiveProperty<int>(resourceConfig.Stability);

        // Use config for max values
        MaxProvision = resourceConfig.MaxProvision;
        MaxMedicine = resourceConfig.MaxMedicine;
        MaxRawMaterials = resourceConfig.MaxRawMaterials;
        MaxReadyMaterials = resourceConfig.MaxReadyMaterials;
        MaxStability = resourceConfig.MaxStability;
    }
    
    private void SetResourceValue(ResourceType type, int value)
    {
        switch (type)
        {
            case ResourceType.Provision:
                Provision.Value = Mathf.Clamp(value, 0, MaxProvision);
                break;
            case ResourceType.Medicine:
                Medicine.Value = Mathf.Clamp(value, 0, MaxMedicine);
                break;
            case ResourceType.RawMaterials:
                RawMaterials.Value = Mathf.Clamp(value, 0, MaxRawMaterials);
                break;
            case ResourceType.ReadyMaterials:
                ReadyMaterials.Value = Mathf.Clamp(value, 0, MaxReadyMaterials);
                break;
            case ResourceType.Stability:
                Stability.Value = Mathf.Clamp(value, 0, MaxStability);
                if (Stability.Value == 0)
                {
                    _mainGameController.GameLost();
                }
                break;
        }
    }

    public void ModifyResource(ResourceType type, int value)
    {
        switch (type)
        {
            case ResourceType.Provision:
                Provision.Value = Mathf.Clamp(Provision.Value + value, 0, MaxProvision);
                break;
            case ResourceType.Medicine:
                Medicine.Value = Mathf.Clamp(Medicine.Value + value, 0, MaxMedicine);
                break;
            case ResourceType.RawMaterials:
                RawMaterials.Value = Mathf.Clamp(RawMaterials.Value + value, 0, MaxRawMaterials);
                break;
            case ResourceType.ReadyMaterials:
                ReadyMaterials.Value = Mathf.Clamp(ReadyMaterials.Value + value, 0, MaxReadyMaterials);
                break;
            case ResourceType.Stability:
                Stability.Value = Mathf.Clamp(Stability.Value + value, 0, MaxStability);
                if (Stability.Value == 0)
                {
                    _mainGameController.GameLost();
                }
                break;
        }
    }

    public enum ResourceType
    {
        Provision,
        Medicine,
        RawMaterials,
        ReadyMaterials,
        Stability
    }
}

