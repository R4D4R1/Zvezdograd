using UniRx;

public class ResourceViewModel
{
    public ResourceModel Model { get; private set; }

    public IReadOnlyReactiveProperty<int> Provision => Model.Provision;
    public IReadOnlyReactiveProperty<int> Medicine => Model.Medicine;
    public IReadOnlyReactiveProperty<int> RawMaterials => Model.RawMaterials;
    public IReadOnlyReactiveProperty<int> ReadyMaterials => Model.ReadyMaterials;
    public IReadOnlyReactiveProperty<int> Stability => Model.Stability;

    public ResourceViewModel(ResourceModel model)
    {
        Model = model;
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

    public void ModifyResource(ResourceModel.ResourceType type, int value)
    {
        Model.ModifyResource(type, value);
    }
}
