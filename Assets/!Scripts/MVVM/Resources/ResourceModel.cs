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

    public readonly int MaxProvision = 10;
    public readonly int MaxMedicine = 10;
    public readonly int MaxRawMaterials = 10;
    public readonly int MaxReadyMaterials = 10;
    public readonly int MaxStability = 100;

    private readonly ControllersManager _controllersManager;

    [Inject]
    public ResourceModel(ControllersManager controllersManager, ResourcesConfig resourceConfig)
    {
        Debug.Log("Test");
        _controllersManager = controllersManager;

        Provision = new ReactiveProperty<int>(resourceConfig.InitialProvision);
        Medicine = new ReactiveProperty<int>(resourceConfig.InitialMedicine);
        RawMaterials = new ReactiveProperty<int>(resourceConfig.InitialRawMaterials);
        ReadyMaterials = new ReactiveProperty<int>(resourceConfig.InitialReadyMaterials);
        Stability = new ReactiveProperty<int>(resourceConfig.InitialStability);
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
                    _controllersManager.MainGameController.GameLost();
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

