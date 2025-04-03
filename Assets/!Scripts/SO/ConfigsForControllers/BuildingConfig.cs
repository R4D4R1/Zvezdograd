using UnityEngine;

[CreateAssetMenu(fileName = "BuildingControllerConfig", menuName = "Configs/BuildingControllerConfig")]
public class BuildingControllerConfig : ScriptableObject
{
    [SerializeField, Range(0f, 100f)] private float chanceOfBombingBuilding;
    [SerializeField, Range(0f, 100f)] private float chanceOfBombingSpecialBuilding ;
    [SerializeField, Range(1f, 10f)] private int stabilityRemoveValueForRegularBombedBuilding;
    [SerializeField, Range(1f, 10f)] private int stabilityRemoveValueForSpecialBombedBuilding;
    
    public float ChanceOfBombingBuilding => chanceOfBombingBuilding;
    public float ChanceOfBombingSpecialBuilding => chanceOfBombingSpecialBuilding;
    public int StabilityRemoveValueForRegularBombedBuilding => stabilityRemoveValueForRegularBombedBuilding;
    public int StabilityRemoveValueForSpecialBombedBuilding => stabilityRemoveValueForSpecialBombedBuilding;
}