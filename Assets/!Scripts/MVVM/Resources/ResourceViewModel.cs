using UniRx;

public class ResourceViewModel
{
    public IReadOnlyReactiveProperty<int> Provision => Model.Provision;
    public IReadOnlyReactiveProperty<int> Medicine => Model.Medicine;
    public IReadOnlyReactiveProperty<int> RawMaterials => Model.RawMaterials;
    public IReadOnlyReactiveProperty<int> ReadyMaterials => Model.ReadyMaterials;
    public IReadOnlyReactiveProperty<int> Stability => Model.Stability;

    public ResourceModel Model { get; private set; }

    public ReactiveCommand<(ResourceModel.ResourceType, int)> ModifyResourceCommand { get; } = new();

    public ResourceViewModel(ResourceModel model)
    {
        Model = model;
        ModifyResourceCommand.Subscribe(x => Model.ModifyResource(x.Item1, x.Item2));
    }

    public int GetResourceValue(ResourceModel.ResourceType resourceType)
    {
        return resourceType switch
        {
            ResourceModel.ResourceType.Provision => Provision.Value,
            ResourceModel.ResourceType.Medicine => Medicine.Value,
            ResourceModel.ResourceType.RawMaterials => RawMaterials.Value,
            ResourceModel.ResourceType.ReadyMaterials => ReadyMaterials.Value,
            _ => 0
        };
    }
    
    public int GetMaxResourceValue(ResourceModel.ResourceType resourceType)
    {
        return resourceType switch
        {
            ResourceModel.ResourceType.Provision => Model.MaxProvision,
            ResourceModel.ResourceType.Medicine => Model.MaxMedicine,
            ResourceModel.ResourceType.RawMaterials => Model.MaxRawMaterials,
            ResourceModel.ResourceType.ReadyMaterials => Model.MaxReadyMaterials,
            _ => 0
        };
    }

    private void ModifyResource(ResourceModel.ResourceType type, int value)
    {
        Model.ModifyResource(type, value);
    }
}
