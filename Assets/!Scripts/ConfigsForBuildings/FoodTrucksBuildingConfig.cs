using UnityEngine;

[CreateAssetMenu(fileName = "FoodTrucksBuildingConfig", menuName = "BuildingConfigs/FoodTrucksBuildingConfig")]
public class FoodTrucksBuildingConfig : ScriptableObject
{
    [Header("Food Trucks Settings")]
    [SerializeField] private int _peopleToGiveProvision;
    [SerializeField] private int _turnsToGiveProvisionOriginal;
    [SerializeField] private int _turnsToRestFromProvisionJob;
    [SerializeField] private int _foodToGive;
    [SerializeField] private int _stabilityAddValue;
    [SerializeField] private int _stabilityRemoveValue;

    public int PeopleToGiveProvision => _peopleToGiveProvision;
    public int TurnsToGiveProvisionOriginal => _turnsToGiveProvisionOriginal;
    public int TurnsToRestFromProvisionJob => _turnsToRestFromProvisionJob;
    public int FoodToGive => _foodToGive;
    public int StabilityAddValue => _stabilityAddValue;
    public int StabilityRemoveValue => _stabilityRemoveValue;
}
