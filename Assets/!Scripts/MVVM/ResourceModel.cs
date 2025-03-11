using UniRx;

public class ResourceModel
{
    public ReactiveProperty<int> Provision { get; } = new(0);
    public ReactiveProperty<int> Medicine { get; } = new(0);
    public ReactiveProperty<int> RawMaterials { get; } = new(0);
    public ReactiveProperty<int> ReadyMaterials { get; } = new(0);
    public ReactiveProperty<int> Stability { get; } = new(100);

    public readonly int MaxProvision = 10;
    public readonly int MaxMedicine = 10;
    public readonly int MaxRawMaterials = 10;
    public readonly int MaxReadyMaterials = 10;
    public readonly int MaxStability = 100;

    public ResourceModel(int provision = 0, int medicine = 0, int rawMaterials = 0, int readyMaterials = 0, int stability = 100)
    {
        Provision = new ReactiveProperty<int>(provision);
        Medicine = new ReactiveProperty<int>(medicine);
        RawMaterials = new ReactiveProperty<int>(rawMaterials);
        ReadyMaterials = new ReactiveProperty<int>(readyMaterials);
        Stability = new ReactiveProperty<int>(stability);
    }

    public void ModifyResource(ResourceType type, int value)
    {
        switch (type)
        {
            case ResourceType.Provision:
                Provision.Value = UnityEngine.Mathf.Clamp(Provision.Value + value, 0, MaxProvision);
                break;
            case ResourceType.Medicine:
                Medicine.Value = UnityEngine.Mathf.Clamp(Medicine.Value + value, 0, MaxMedicine);
                break;
            case ResourceType.RawMaterials:
                RawMaterials.Value = UnityEngine.Mathf.Clamp(RawMaterials.Value + value, 0, MaxRawMaterials);
                break;
            case ResourceType.ReadyMaterials:
                ReadyMaterials.Value = UnityEngine.Mathf.Clamp(ReadyMaterials.Value + value, 0, MaxReadyMaterials);
                break;
            case ResourceType.Stability:
                Stability.Value = UnityEngine.Mathf.Clamp(Stability.Value + value, 0, MaxStability);
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
