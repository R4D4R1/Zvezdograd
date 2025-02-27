using UnityEngine;

public class SelectableBuilding : MonoBehaviour
{
    public int BuildingId { get; private set; }
    [field: SerializeField] public string BuildingNameText { get; protected set; }

    [field: SerializeField] public string DescriptionText { get; protected set; }

    [field: SerializeField] public bool BuildingIsSelactable { get; protected set; } = true;

    protected virtual void Awake()
    {
        BuildingId = GenerateUniqueId();
    }

    private int GenerateUniqueId()
    {
        return gameObject.GetInstanceID();
    }
}
