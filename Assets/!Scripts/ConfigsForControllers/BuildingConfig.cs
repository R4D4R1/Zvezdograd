using UnityEngine;

[CreateAssetMenu(fileName = "BuildingConfig", menuName = "Configs/BuildingConfig")]
public class BuildingConfig : ScriptableObject
{
    [Range(0f, 100f), SerializeField]
    private int _specialBuildingBombChance;

    public int SpecialBuildingBombChance => _specialBuildingBombChance;
}