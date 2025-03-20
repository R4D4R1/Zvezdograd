using DG.Tweening;
using TMPro;
using UnityEngine;
using UniRx;

public class CityHallPopUp : QuestPopUp
{
    [SerializeField] private TextMeshProUGUI _relationWithGovermentText;
    [SerializeField] private TextMeshProUGUI _militaryTimerText;
    [SerializeField] private TextMeshProUGUI _helpFromGovTimerText;
    [SerializeField] private TextMeshProUGUI _createPeopleText;

    private CityHallBuilding _building;
    private TimeController _timeController;

    public override void Init()
    {
        base.Init();

        _building = _controllersManager.BuildingController.GetCityHallBuilding();
        _timeController = _controllersManager.TimeController;

        _controllersManager.PeopleUnitsController.OnUnitCreatedByPeopleUnitController
            .Subscribe(_ => UpdateCreateUnitGO())
            .AddTo(this);

        _controllersManager.TimeController.OnNextDayEvent
            .Subscribe(_ => UpdateCreateUnitGO())
            .AddTo(this);

        SetButtonState(true);

    }

    public void ShowCityHallPopUp()
    {
        UpdateAllText();
        ShowPopUp();
    }

    public void CreateNewUnit()
    {
        if (CanCreateNewUnit())
        {
            _building.NewUnitStartedCreating();
            SetButtonState(false);
        }
    }

    private bool CanCreateNewUnit()
    {
        return HasEnoughResources(ResourceModel.ResourceType.ReadyMaterials,
               _building.ReadyMaterialsToCreateNewPeopleUnit) &&
               HasEnoughPeople(_controllersManager.BuildingController.GetFoodTruckBuilding().PeopleToGiveProvision) &&
               CanUseActionPoint();
    }

    private void UpdateAllText()
    {
        _relationWithGovermentText.text = $"Отношение {_building.RelationWithGoverment}";
        _militaryTimerText.text = _building.IsMaterialsSent
            ? "Военная помощь отправлена, ожидайте указаний"
            : $"Крайний срок отправки воен. помощи {_building.DaysLeftToSendArmyMaterials} дн.";

        _helpFromGovTimerText.text = $"Помощь прибудет через {_building.DaysLeftToRecieveGovHelp} дн.";

        _createPeopleText.text = $"Организовать новое подразделение - доступно " +
                                 $"{_controllersManager.PeopleUnitsController.NotCreatedUnits.Count}";
    }

    public void UpdateCreateUnitGO()
    {
        if (_controllersManager.PeopleUnitsController.NotCreatedUnits.Count > 0)
        {
            SetButtonState(true);
        }
        else
        {
            Destroy(_createPeopleText.gameObject);
        }
    }
}
