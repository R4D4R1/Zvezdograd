using UnityEngine;

[CreateAssetMenu(fileName = "TutorialControllerConfig", menuName = "Configs/TutorialControllerConfig")]
public class TutorialControllerConfig : ScriptableObject
{
    [Header("Описание зданий")]
    [SerializeField] private string collectableBuildingDescription;
    [SerializeField] private string intactBuildingDescription;
    [SerializeField] private string damagedBuildingDescription;
    [SerializeField] private string factoryBuildingDescription;
    [SerializeField] private string cityHallBuildingDescription;
    [SerializeField] private string hospitalBuildingDescription;
    [SerializeField] private string foodTruckBuildingDescription;
    
    public string CollectableBuildingDescription => collectableBuildingDescription;
    public string IntactBuildingDescription => intactBuildingDescription;
    public string DamagedBuildingDescription => damagedBuildingDescription;
    public string FactoryBuildingDescription => factoryBuildingDescription;
    public string CityHallBuildingDescription => cityHallBuildingDescription;
    public string HospitalBuildingDescription => hospitalBuildingDescription;
    public string FoodTruckBuildingDescription => foodTruckBuildingDescription;
}