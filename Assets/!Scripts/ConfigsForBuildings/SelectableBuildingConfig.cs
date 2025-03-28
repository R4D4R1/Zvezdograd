using UnityEngine;

[CreateAssetMenu(fileName = "NewSelectableBuildingConfig", menuName = "BuildingConfigs/SelectableBuildingConfig")]
public class SelectableBuildingConfig : ScriptableObject
{
    [SerializeField] private string _buildingNameText;
    [SerializeField] private string _descriptionText;

    public string BuildingNameText => _buildingNameText;
    public string DescriptionText => _descriptionText;
}
