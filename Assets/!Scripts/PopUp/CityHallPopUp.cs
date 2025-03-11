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
        _controllersManager.PeopleUnitsController.OnUnitCreatedByPeopleUnitController += UpdateCreateUnitGO;
    }

    private void OnDisable()
    {
        _controllersManager.PeopleUnitsController.OnUnitCreatedByPeopleUnitController -= UpdateCreateUnitGO;
    }

    protected override void Start()
    {
        base.Start();

        _building = _controllersManager.BuildingController.GetCityHallBuilding();
        _timeController = _controllersManager.TimeController;
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
        transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
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
            _controllersManager.BuildingController.GetCityHallBuilding().NewUnitStartedCreating();

            SetButtonState(false);
        }
        else
        {
            ShowErrorMessage();
        }
    }

    public bool EnoughReadyMaterialToCreate()
    {
        return ChechIfEnoughResourcesByType(ResourceModel.ResourceType.ReadyMaterials,
            _controllersManager.BuildingController.GetCityHallBuilding().ReadyMaterialsToCreateNewPeopleUnit);
    }

    private void ShowErrorMessage()
    {
        if (!EnoughReadyMaterialToCreate())
        {
            _errorText.text = "�� ���������� ����������";
        }
        else if (!CheckForEnoughPeople(_controllersManager.BuildingController.GetFoodTruckBuilding().PeopleToGiveProvision))
        {
            _errorText.text = "�� ���������� �����";
        }
        _errorText.enabled = true;
    }

    private void UpdateAllText()
    {
        _relationWithGovermentText.text = $"��������� {_building.RelationWithGoverment}";
        _militaryTimerText.text = _building.IsMaterialsSent
            ? "������� ������ ����������, �������� ��������"
            : $"������� ���� �������� ����. ������ {_building.DaysLeftToSendArmyMaterials} ��.";

        _helpFromGovTimerText.text = $"������ �������� ����� {_building.DaysLeftToRecieveGovHelp} ��.";
    }

    private void UpdateCreatePeopleText()
    {
        _createPeopleText.text = $"������������ ����� ������������� - �������� " +
            $"{_controllersManager.PeopleUnitsController.NotCreatedUnits.Count}";
    }

    public void UpdateCreateUnitGO()
    {
        if (_controllersManager.PeopleUnitsController.NotCreatedUnits.Count > 0)
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
