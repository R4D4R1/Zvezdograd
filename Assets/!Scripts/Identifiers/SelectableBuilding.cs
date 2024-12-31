using UnityEngine;

public class SelectableBuilding : MonoBehaviour
{
    [field: SerializeField] public string BuildingNameText { get; protected set; }
    [field: SerializeField] public string DescriptionText { get; protected set; }
}
