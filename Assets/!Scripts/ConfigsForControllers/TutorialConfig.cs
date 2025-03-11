using UnityEngine;

[CreateAssetMenu(fileName = "TutorialConfig", menuName = "Configs/TutorialConfig")]
public class TutorialConfig : ScriptableObject
{
    [SerializeField] private CollectableBuilding collectableBuilding;
    [SerializeField] private RepairableBuilding intactBuilding;
    [SerializeField] private RepairableBuilding damagedBuilding;
    [SerializeField] private FactoryBuilding factoryBuilding;

    [SerializeField] private GameObject specialPopUpPrefab;
    [SerializeField] private Transform popUpParent;

    [Header("Текст для подсказок зданий")]
    [SerializeField] private string collectableBuildingDescription;
    [SerializeField] private string intactBuildingDescription;
    [SerializeField] private string damagedBuildingDescription;
    [SerializeField] private string factoryBuildingDescription;
    [SerializeField] private string cityHallBuildingDescription;
    [SerializeField] private string hospitalBuildingDescription;
    [SerializeField] private string foodTruckBuildingDescription;

    public CollectableBuilding CollectableBuilding => collectableBuilding;
    public RepairableBuilding IntactBuilding => intactBuilding;
    public RepairableBuilding DamagedBuilding => damagedBuilding;
    public FactoryBuilding FactoryBuilding => factoryBuilding;

    public GameObject SpecialPopUpPrefab => specialPopUpPrefab;
    public Transform PopUpParent => popUpParent;

    public string CollectableBuildingDescription => collectableBuildingDescription;
    public string IntactBuildingDescription => intactBuildingDescription;
    public string DamagedBuildingDescription => damagedBuildingDescription;
    public string FactoryBuildingDescription => factoryBuildingDescription;
    public string CityHallBuildingDescription => cityHallBuildingDescription;
    public string HospitalBuildingDescription => hospitalBuildingDescription;
    public string FoodTruckBuildingDescription => foodTruckBuildingDescription;
}
