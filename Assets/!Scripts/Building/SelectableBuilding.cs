using UnityEngine;
using Zenject;

public class SelectableBuilding : MonoBehaviour
{
    [field: SerializeField] public string BuildingNameText { get; protected set; }
    [field: SerializeField] public string DescriptionText { get; protected set; }
    [field: SerializeField] public bool BuildingIsSelectable { get; protected set; } = true;
    public int BuildingId { get; private set; }

    protected ControllersManager _controllersManager;
    protected ResourceViewModel _resourceViewModel;

    [Inject]
    public void Construct(ControllersManager controllersManager, ResourceViewModel resourceViewModel)
    {
        _controllersManager = controllersManager;
        _resourceViewModel = resourceViewModel;
    }

    protected virtual void Start()
    {
        GenerateOrLoadBuildingId();
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

        //Debug.Log(BuildingNameText + " " + BuildingId);
    }
}
