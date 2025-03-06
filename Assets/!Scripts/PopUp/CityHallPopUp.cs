using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CityHallPopUp : QuestPopUp
{
    [SerializeField] private TextMeshProUGUI _relationWithGovermentText;
    [SerializeField] private TextMeshProUGUI _militaryTimerText;
    [SerializeField] private TextMeshProUGUI _helpFromGovTimerText;
    [SerializeField] private TextMeshProUGUI _createPeopleText;

    private CityHallBuilding _building;
    private TimeController _timeController;

    private void OnEnable()
    {
        ControllersManager.Instance.buildingController.GetCityHallBuilding().OnNewUnitCreated += UpdateCreateUnitGO;
    }

    private void OnDisable()
    {
        ControllersManager.Instance.buildingController.GetCityHallBuilding().OnNewUnitCreated -= UpdateCreateUnitGO;
    }

    protected override void Start()
    {
        base.Start();

        _building = ControllersManager.Instance.buildingController.GetCityHallBuilding();
        _timeController = ControllersManager.Instance.timeController;
        _timeController.OnNextDayEvent += OnNextDayEvent;

        SetButtonState(true);
        _errorText.enabled = false;
        _isDestroyable = false;

        UpdateAllText();
        UpdateCreatePeopleText();
    }

    public void ShowCityHallPopUp()
    {
        UpdateAllText();
        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            IsActive = true;
            SetAlpha(1);
        });
    }

    private void OnNextDayEvent()
    {
        if (_building.DayPassed())
        {
            UpdateAllText();
        }
    }

    public void CreateNewUnit()
    {
        if (EnoughReadyMaterialToCreate())
        {
            ControllersManager.Instance.buildingController.GetCityHallBuilding().NewUnitStartedCreating();

            SetButtonState(false);
        }
        else
        {
            ShowErrorMessage();
        }
    }

    //private bool CanCreateNewUnit()
    //{
    //    return CheckForEnoughPeople(ControllersManager.Instance.buildingController.GetCityHallBuilding().PeopleToCreateNewPeopleUnit) &&
    //           EnoughReadyMaterialToCreate();
    //}

    public bool EnoughReadyMaterialToCreate()
    {
        return ChechIfEnoughResourcesByType(ResourceController.ResourceType.ReadyMaterials,
            ControllersManager.Instance.buildingController.GetCityHallBuilding().ReadyMaterialsToCreateNewPeopleUnit);
    }

    private void ShowErrorMessage()
    {
        if (!EnoughReadyMaterialToCreate())
        {
            _errorText.text = "НЕ ДОСТАТОЧНО МАТЕРИАЛОВ";
        }
        else if (!CheckForEnoughPeople(ControllersManager.Instance.buildingController.GetFoodTruckBuilding().PeopleToGiveProvision))
        {
            _errorText.text = "НЕ ДОСТАТОЧНО ЛЮДЕЙ";
        }
        _errorText.enabled = true;
    }

    private void UpdateAllText()
    {
        _relationWithGovermentText.text = $"Отношение {_building.RelationWithGoverment}";
        _militaryTimerText.text = _building.IsMaterialsSent
            ? "Военная помощь отправлена, ожидайте указаний"
            : $"Крайний срок отправки воен. помощи {_building.DaysLeftToSendArmyMaterials} дн.";

        _helpFromGovTimerText.text = $"Помощь прибудет через {_building.DaysLeftToRecieveGovHelp} дн.";
    }

    private void UpdateCreatePeopleText()
    {
        _createPeopleText.text = $"Организовать новое подразделение - доступно " +
            $"{ControllersManager.Instance.peopleUnitsController.NotCreatedUnits.Count}";
    }

    public void UpdateCreateUnitGO()
    {
        if (ControllersManager.Instance.peopleUnitsController.NotCreatedUnits.Count > 0)
        {
            UpdateCreatePeopleText();
            SetButtonState(true);
        }
        else
        {
            Destroy(_createPeopleText.gameObject);
        }

    }
}
