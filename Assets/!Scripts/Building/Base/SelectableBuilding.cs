using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class SelectableBuilding : MonoBehaviour
{
    [FormerlySerializedAs("_selectableBuildingConfig")]
    [Header("DEFAULT SETTINGS")]
    [SerializeField] private SelectableBuildingConfig selectableBuildingConfig;

    [HideInInspector] public bool buildingIsSelectable = true;
    
    public string BuildingLabel { get; private set; }
    public string BuildingDescription { get; private set; }
    
    private int BuildingId { get; set; }

    protected TimeController TimeController;
    protected ResourceViewModel ResourceViewModel;
    protected BuildingController BuildingController;
    protected PeopleUnitsController PeopleUnitsController;
    protected MainGameController MainGameController;
    protected PopUpsController PopUpsController;

    [Inject]
    public void Construct(TimeController timeController,ResourceViewModel resourceViewModel,
        BuildingController buildingController, PeopleUnitsController peopleUnitsController,
        MainGameController mainGameController,PopUpsController popUpsController)
    {
        TimeController = timeController;
        ResourceViewModel = resourceViewModel;
        BuildingController = buildingController;
        PeopleUnitsController = peopleUnitsController;
        MainGameController = mainGameController;
        PopUpsController = popUpsController;
    }
    
    
    public virtual void Init()
    {
        if (selectableBuildingConfig)
        {
            BuildingLabel = selectableBuildingConfig.BuildingLabel;
            BuildingDescription = selectableBuildingConfig.BuildingDescription;
        }

        buildingIsSelectable = true;

        GenerateOrLoadBuildingId();

        //Debug.Log($"{name} Init");
    }

    private void GenerateOrLoadBuildingId()
    {
        string uniqueKey = $"Building_{gameObject.GetInstanceID()}";

        if (PlayerPrefs.HasKey(uniqueKey))
        {
            BuildingId = PlayerPrefs.GetInt(uniqueKey);
        }
        else
        {
            BuildingId = Random.Range(100000, 999999);
            PlayerPrefs.SetInt(uniqueKey, BuildingId);
            PlayerPrefs.Save();
        }
    }
}
