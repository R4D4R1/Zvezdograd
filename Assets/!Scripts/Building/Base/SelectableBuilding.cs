using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Zenject;

public class SelectableBuilding : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("DEFAULT SETTINGS")]
    [FormerlySerializedAs("_selectableBuildingConfig")]
    [SerializeField] private SelectableBuildingConfig selectableBuildingConfig;

    [HideInInspector] public bool BuildingIsSelectable = true;

    public readonly Subject<SelectableBuilding> OnPointerEnterSubject = new();
    public readonly Subject<SelectableBuilding> OnPointerExitSubject = new();
    public readonly Subject<SelectableBuilding> OnPointerClickSubject = new();

    public string BuildingLabel { get; private set; }
    public string BuildingDescription { get; private set; }
    public int BuildingId { get; private set; }

    protected TimeController _timeController;
    protected ResourceViewModel _resourceViewModel;
    protected BuildingsController _buildingsController;
    protected PeopleUnitsController _peopleUnitsController;
    protected MainGameController _mainGameController;
    protected PopUpsController _popUpsController;

    [Inject]
    public void Construct(
        TimeController timeController,
        ResourceViewModel resourceViewModel,
        BuildingsController buildingsController,
        PeopleUnitsController peopleUnitsController,
        MainGameController mainGameController,
        PopUpsController popUpsController)
    {
        _timeController = timeController;
        _resourceViewModel = resourceViewModel;
        _buildingsController = buildingsController;
        _peopleUnitsController = peopleUnitsController;
        _mainGameController = mainGameController;
        _popUpsController = popUpsController;
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
    }

    private void GenerateOrLoadBuildingId()
    {
        string uniqueKey = $"Building_{gameObject.name}";

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnterSubject.OnNext(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExitSubject.OnNext(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnPointerClickSubject.OnNext(this);
    }
}
