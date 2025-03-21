using UnityEngine;

[CreateAssetMenu(fileName = "CollectableBuildingConfig", menuName = "BuildingConfigs/CollectableBuildingConfig")]
public class CollectableBuildingConfig : ScriptableObject
{
    [Header("Resources")]
    [Range(1, 10), SerializeField] private int rawMaterialsLeft;
    [Range(1, 10), SerializeField] private int rawMaterialsGet;
    [Range(1, 10), SerializeField] private int peopleToCollect;

    [Header("Turns Settings")]
    [SerializeField] private int turnsToCollectOriginal;
    [SerializeField] private int turnsToRest;

    public int RawMaterialsLeft => rawMaterialsLeft;
    public int RawMaterialsGet => rawMaterialsGet;
    public int PeopleToCollect => peopleToCollect;
    public int TurnsToCollectOriginal => turnsToCollectOriginal;
    public int TurnsToRest => turnsToRest;
}
