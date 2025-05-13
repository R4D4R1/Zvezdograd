using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewSelectableBuildingConfig", menuName = "BuildingConfigs/SelectableBuildingConfig")]
public class SelectableBuildingConfig : ScriptableObject
{
    [FormerlySerializedAs("_buildingNameText")] [SerializeField] private string buildingLabel;
    [FormerlySerializedAs("_descriptionText")] [SerializeField, TextArea(2,5)] private string buildingDescription;

    public string BuildingLabel => buildingLabel;
    public string BuildingDescription => buildingDescription;
}
