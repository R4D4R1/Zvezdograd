using UnityEngine;

[CreateAssetMenu(fileName = "FactoryBuildingConfig", menuName = "BuildingConfigs/FactoryBuildingConfig")]
public class FactoryBuildingConfig : ScriptableObject
{
    [Header("Production Settings")]
    [Range(1, 10), SerializeField] private int _peopleToCreateReadyMaterials;
    [Range(1, 10), SerializeField] private int _peopleToCreateArmyMaterials;
    [Range(1, 10), SerializeField] private int _turnsToCreateReadyMaterialsOriginal;
    [Range(1, 10), SerializeField] private int _turnsToCreateArmyMaterialsOriginal;
    [Range(1, 10), SerializeField] private int _turnsToRestFromReadyMaterials;
    [Range(1, 10), SerializeField] private int _turnsToRestFromArmyMaterials;
    [Range(1, 10), SerializeField] private int _rawMaterialsToCreateReadyMaterials;
    [Range(1, 10), SerializeField] private int _rawMaterialsToCreateArmyMaterials;
    [Range(1, 10), SerializeField] private int _readyMaterialsGet;

    public int PeopleToCreateReadyMaterials => _peopleToCreateReadyMaterials;
    public int PeopleToCreateArmyMaterials => _peopleToCreateArmyMaterials;
    public int TurnsToCreateReadyMaterialsOriginal => _turnsToCreateReadyMaterialsOriginal;
    public int TurnsToCreateArmyMaterialsOriginal => _turnsToCreateArmyMaterialsOriginal;
    public int TurnsToRestFromReadyMaterials => _turnsToRestFromReadyMaterials;
    public int TurnsToRestFromArmyMaterials => _turnsToRestFromArmyMaterials;
    public int RawMaterialsToCreateReadyMaterials => _rawMaterialsToCreateReadyMaterials;
    public int ReadyMaterialsGet => _readyMaterialsGet;
    public int RawMaterialsToCreateArmyMaterials => _rawMaterialsToCreateArmyMaterials;
}
