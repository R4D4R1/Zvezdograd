using UnityEngine;

[CreateAssetMenu(fileName = "BuildingConfig", menuName = "BuildingConfigs/RepairableBuildingConfig")]
public class RepairableBuildingConfig : ScriptableObject
{
    [Header("Building Info")]
    [SerializeField] private string _damagedBuildingNameText;
    [SerializeField] private string _damagedDescriptionText;

    [Header("Repair Requirements")]
    [SerializeField, Range(1f, 5f)] private int _buildingMaterialsToRepair;
    [SerializeField, Range(1f, 5f)] private int _peopleToRepair;

    [Header("Repair Time")]
    [SerializeField, Range(2f, 5f)] private int _turnsToRepairOriginal;

    [SerializeField, Range(1f, 5f)] private int _turnsToRestFromRepair;

    [Header("Building Type")]
    [SerializeField] private RepairableBuilding.BuildingType _buildingType;
    [SerializeField] private RepairableBuilding.State _state;

    public string DamagedBuildingNameText => _damagedBuildingNameText;
    public string DamagedDescriptionText => _damagedDescriptionText;
    public int BuildingMaterialsToRepair => _buildingMaterialsToRepair;
    public int PeopleToRepair => _peopleToRepair;
    public int TurnsToRepairOriginal => _turnsToRepairOriginal;
    public int TurnsToRestFromRepair => _turnsToRestFromRepair;
    public RepairableBuilding.BuildingType BuildingType => _buildingType;
    public RepairableBuilding.State State => _state;
}
