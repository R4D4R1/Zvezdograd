using UnityEngine;

public class Building : MonoBehaviour
{
    [field: SerializeField] public string BuildingNameText { get; protected set; }
    [field: SerializeField] public string DescriptionText { get; protected set; }
}
