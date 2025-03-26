using UnityEngine;

[CreateAssetMenu(fileName = "BuildingControllerConfig", menuName = "Configs/BuildingControllerConfig")]
public class BuildingControllerConfig : ScriptableObject
{
    [SerializeField, Range(0f, 100f)] private float chanceOfBombingBuilding = 50f;
    [SerializeField, Range(0f, 100f)] private float chanceOfBombingSpecialBuilding = 25f;

    public float ChanceOfBombingBuilding => chanceOfBombingBuilding;
    public float ChanceOfBombingSpecialBuilding => chanceOfBombingSpecialBuilding;
}