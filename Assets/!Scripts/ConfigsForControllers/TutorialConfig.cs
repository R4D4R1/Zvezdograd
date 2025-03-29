using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TutorialControllerConfig", menuName = "Configs/TutorialControllerConfig")]
public class TutorialControllerConfig : ScriptableObject
{
    [Header("Описания зданий")]
    [SerializeField, TextArea(2, 4)] private string collectableBuildingDescription;
    [SerializeField, TextArea(2, 4)] private string intactBuildingDescription;
    [SerializeField, TextArea(2, 4)] private string damagedBuildingDescription;
    [SerializeField, TextArea(2, 4)] private string factoryBuildingDescription;
    [SerializeField, TextArea(2, 4)] private string cityHallBuildingDescription;
    [SerializeField, TextArea(2, 4)] private string hospitalBuildingDescription;
    [SerializeField, TextArea(2, 4)] private string foodTruckBuildingDescription;
    
    [Header("Названия попапов")]
    [SerializeField] private string clockLabel;
    [SerializeField] private string provisionLabel;
    [SerializeField] private string medsLabel;
    [SerializeField] private string rawMaterialsLabel;
    [SerializeField] private string readyMaterialsLabel;
    [SerializeField] private string unitsLabel;
    [SerializeField] private string stabilityLabel;
    [SerializeField] private string nextTurnLabel;
    
    [Header("Описания специальных попапов")]
    [SerializeField, TextArea(2, 3)] private string clockDescription;
    [SerializeField, TextArea(2, 3)] private string provisionDescription;
    [SerializeField, TextArea(2, 3)] private string medsDescription;
    [SerializeField, TextArea(2, 3)] private string rawMaterialsDescription;
    [SerializeField, TextArea(2, 3)] private string readyMaterialsDescription;
    [SerializeField, TextArea(2, 3)] private string unitsDescription;
    [SerializeField, TextArea(2, 3)] private string stabilityDescription;
    [SerializeField, TextArea(2, 3)] private string nextTurnDescription;
    
    public string CollectableBuildingDescription => collectableBuildingDescription;
    public string IntactBuildingDescription => intactBuildingDescription;
    public string DamagedBuildingDescription => damagedBuildingDescription;
    public string FactoryBuildingDescription => factoryBuildingDescription;
    public string CityHallBuildingDescription => cityHallBuildingDescription;
    public string HospitalBuildingDescription => hospitalBuildingDescription;
    public string FoodTruckBuildingDescription => foodTruckBuildingDescription;
    
    
    public string ClockLabel => clockLabel;
    public string ProvisionLabel => provisionLabel;
    public string MedsLabel => medsLabel;
    public string RawMaterialsLabel => rawMaterialsLabel;
    public string ReadyMaterialsLabel => readyMaterialsLabel;
    public string UnitsLabel => unitsLabel;
    public string StabilityLabel => stabilityLabel;
    public string NextTurnLabel => nextTurnLabel;
    
    
    public string ClockDescription => clockDescription;
    public string ProvisionDescription => provisionDescription;
    public string MedsDescription => medsDescription;
    public string RawMaterialsDescription => rawMaterialsDescription;
    public string ReadyMaterialsDescription => readyMaterialsDescription;
    public string UnitsDescription => unitsDescription;
    public string StabilityDescription => stabilityDescription;
    public string NextTurnDescription => nextTurnDescription;
}