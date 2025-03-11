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

    public void ModifyResource(ResourceModel.ResourceType type, int value)
    {
        Model.ModifyResource(type, value);
    }
}
