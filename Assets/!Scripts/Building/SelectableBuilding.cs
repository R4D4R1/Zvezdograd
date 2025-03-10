using UnityEngine;

public class SelectableBuilding : MonoBehaviour
{
    public int BuildingId { get; private set; }

    [field: SerializeField] public string BuildingNameText { get; protected set; }
    [field: SerializeField] public string DescriptionText { get; protected set; }
    [field: SerializeField] public bool BuildingIsSelectable { get; protected set; } = true;

    private void Start()
    {
        GenerateOrLoadBuildingId();
    }

    private void GenerateOrLoadBuildingId()
    {
        string uniqueKey = $"Building_{gameObject.GetInstanceID()}";

        if (PlayerPrefs.HasKey(uniqueKey))
        {
            BuildingId = PlayerPrefs.GetInt(uniqueKey);
        }
        else
        {
            BuildingId = Random.Range(100000, 999999);
            PlayerPrefs.SetInt(uniqueKey, BuildingId);
            PlayerPrefs.Save();
        }

        Debug.Log(BuildingNameText + " " + BuildingId);
    }
}
