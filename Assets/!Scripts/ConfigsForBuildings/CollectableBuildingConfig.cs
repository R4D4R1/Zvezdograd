using UnityEngine;

[CreateAssetMenu(fileName = "CollectableBuildingConfig", menuName = "BuildingConfigs/CollectableBuildingConfig")]
public class CollectableBuildingConfig : ScriptableObject
{
    [Header("Resources")]
    [SerializeField] private int rawMaterialsLeft;
    [SerializeField] private int rawMaterialsGet;
    [SerializeField] private int peopleToCollect;

    [Header("Turns Settings")]
    [SerializeField] private int turnsToCollectOriginal;
    [SerializeField] private int turnsToRest;

    [Header("Materials")]
    [SerializeField] private Material greyMaterial;

    public int RawMaterialsLeft => rawMaterialsLeft;
    public int RawMaterialsGet => rawMaterialsGet;
    public int PeopleToCollect => peopleToCollect;
    public int TurnsToCollectOriginal => turnsToCollectOriginal;
    public int TurnsToRest => turnsToRest;
    public Material GreyMaterial => greyMaterial;
}
