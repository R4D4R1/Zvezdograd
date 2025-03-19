using UnityEngine;
using Zenject;

public class SelectableBuilding : MonoBehaviour
{
    [Header("DEFAULT SETTINGS")]
    [SerializeField] private SelectableBuildingConfig _selectableBuildingConfig;

    [HideInInspector] public bool BuildingIsSelectable = true;

    public int BuildingId { get; private set; }

    protected ControllersManager _controllersManager;
    protected ResourceViewModel _resourceViewModel;

    [Inject]
    public void Construct(ControllersManager controllersManager, ResourceViewModel resourceViewModel)
    {
        _controllersManager = controllersManager;
        _resourceViewModel = resourceViewModel;
    }

    public virtual void Init()
    {
        if (_selectableBuildingConfig != null)
        {
            BuildingNameText = _selectableBuildingConfig.BuildingNameText;
            DescriptionText = _selectableBuildingConfig.DescriptionText;
        }

        BuildingIsSelectable = true;

        GenerateOrLoadBuildingId();

        //Debug.Log($"{gameObject.name} - Initialized successfully");
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

    public string BuildingNameText { get; protected set; }
    public string DescriptionText { get; protected set; }

    //public bool BuildingIsSelectable
    //{
    //    get => _selectableBuildingConfig != null && _selectableBuildingConfig.BuildingIsSelectable;
    //    set
    //    {
    //        if (_selectableBuildingConfig != null)
    //        {
    //            _selectableBuildingConfig.BuildingIsSelectable = value;
    //        }
    //    }
    //}

}
