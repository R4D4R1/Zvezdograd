using TMPro;
using UnityEngine;
using UniRx;
using UnityEngine.Serialization;

public class CityHallPopUp : QuestPopUp
{
    [FormerlySerializedAs("_relationWithGovermentText")] [SerializeField] private TextMeshProUGUI relationWithGovermentText;
    [FormerlySerializedAs("_militaryTimerText")] [SerializeField] private TextMeshProUGUI militaryTimerText;
    [FormerlySerializedAs("_helpFromGovTimerText")] [SerializeField] private TextMeshProUGUI helpFromGovTimerText;
    [FormerlySerializedAs("_createPeopleText")] [SerializeField] private TextMeshProUGUI createPeopleText;
    [SerializeField] private GameObject createNewUnitBtnParent;
    
    private CityHallBuilding _building;

    public override void Init()
    {
        base.Init();

        _building = _controllersManager.BombBuildingController.GetCityHallBuilding();

        _controllersManager.PeopleUnitsController.OnUnitCreatedByPeopleUnitController
            .Subscribe(_ => UpdateCreateUnitGOButtonState())
            .AddTo(this);

        SetButtonState(createNewUnitBtnParent,true);
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
            SetButtonState(createNewUnitBtnParent,false);
        }
    }

    private bool CanCreateNewUnit()
    {
        return HasEnoughResources(ResourceModel.ResourceType.ReadyMaterials,
               _building.ReadyMaterialsToCreateNewPeopleUnit) &&
               HasEnoughPeople(_controllersManager.BombBuildingController.GetFoodTruckBuilding().PeopleToGiveProvision) &&
               CanUseActionPoint();
    }

    private void UpdateAllText()
    {
        relationWithGovermentText.text = $"Отношения {_building.RelationWithGoverment}";
        
        militaryTimerText.text = _building.IsMaterialsSent
            ? "Военная помощь отправлена, ожидайте указаний"
            : $"До конца отправки воен помощи {_building.DaysLeftToSendArmyMaterials} дн.";

        helpFromGovTimerText.text = $"Помощь от гос-ва прибудет через {_building.DaysLeftToRecieveGovHelp} дн.";

        createPeopleText.text = $"Организовать новое подразделение, осталось  " +
                                 $"{_controllersManager.PeopleUnitsController.NotCreatedUnits.Count}";
    }

    private void UpdateCreateUnitGOButtonState()
    {
        if (_controllersManager.PeopleUnitsController.NotCreatedUnits.Count > 0)
        {
            SetButtonState(createNewUnitBtnParent,true);
        }
        else
        {
            Destroy(createPeopleText.gameObject);
        }
    }
}
