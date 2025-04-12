using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class SelectableBuilding : MonoBehaviour
{
    [Header("DEFAULT SETTINGS")]
    [FormerlySerializedAs("_selectableBuildingConfig")]
    [SerializeField] private SelectableBuildingConfig selectableBuildingConfig;

    [FormerlySerializedAs("buildingIsSelectable")] [HideInInspector] 
    public bool BuildingIsSelectable = true;

    public string BuildingLabel { get; private set; }
    public string BuildingDescription { get; private set; }
    public int BuildingId { get; private set; }

    protected TimeController TimeController;
    protected ResourceViewModel ResourceViewModel;
    protected BuildingsController BuildingsController;
    protected PeopleUnitsController PeopleUnitsController;
    protected MainGameController MainGameController;
    protected PopUpsController PopUpsController;

    [Inject]
    public void Construct(
        TimeController timeController,
        ResourceViewModel resourceViewModel,
        BuildingsController buildingsController,
        PeopleUnitsController peopleUnitsController,
        MainGameController mainGameController,
        PopUpsController popUpsController)
    {
        TimeController = timeController;
        ResourceViewModel = resourceViewModel;
        BuildingsController = buildingsController;
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

        BuildingIsSelectable = true;

        GenerateOrLoadBuildingId();

        // Debug.Log($"{name} Init");
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
