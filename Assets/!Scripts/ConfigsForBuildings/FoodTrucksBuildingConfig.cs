using UnityEngine;

[CreateAssetMenu(fileName = "FoodTrucksBuildingConfig", menuName = "BuildingConfigs/FoodTrucksBuildingConfig")]
public class FoodTrucksBuildingConfig : ScriptableObject
{
    [Header("Food Trucks Settings")]
    [Range(1, 10), SerializeField] private int _peopleToGiveProvision;
    [Range(1, 10), SerializeField] private int _turnsToGiveProvisionOriginal;
    [Range(1, 10), SerializeField] private int _turnsToRestFromProvisionJob;
    [Range(1, 10), SerializeField] private int _foodToGive;
    [Range(1, 10), SerializeField] private int _stabilityAddValue;
    [Range(1, 10), SerializeField] private int _stabilityRemoveValue;

    public int PeopleToGiveProvision => _peopleToGiveProvision;
    public int TurnsToGiveProvisionOriginal => _turnsToGiveProvisionOriginal;
    public int TurnsToRestFromProvisionJob => _turnsToRestFromProvisionJob;
    public int FoodToGive => _foodToGive;
    public int StabilityAddValue => _stabilityAddValue;
    public int StabilityRemoveValue => _stabilityRemoveValue;
}
