using TMPro;
using UnityEngine;
using UniRx;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CityHallPopUp : QuestPopUp
{
    [Header("CITYHALL POPUP SETTINGS")]
    [SerializeField] private TextMeshProUGUI relationWithGovernmentText;
    [SerializeField] private TextMeshProUGUI militaryTimerText;
    [SerializeField] private TextMeshProUGUI helpFromGovTimerText;
    [SerializeField] private TextMeshProUGUI createPeopleText;
    [SerializeField] private GameObject createNewUnitBtnParent;
    [SerializeField] private GameObject enableNewActionPointsQuestGO;
    [Range(1,3),SerializeField] private int amountOfReadyUnitsToCreateNewActionPoints;
    
    [SerializeField] private Color badRelationColor;
    [SerializeField] private Color averageRelationColor;
    [SerializeField] private Color goodRelationColor;
    
    private CityHallBuilding _building;

    public override void Init()
    {
        base.Init();

        _building = BuildingController.GetCityHallBuilding();

        PeopleUnitsController.OnUnitCreatedByPeopleUnitController
            .Subscribe(_ => UpdateCreateUnitGOButtonState())
            .AddTo(this);
        
        EventController.OnQuestTriggered
            .Subscribe(popupEvent =>
            {
                if (popupEvent.buildingType == _building.Type.ToString())
                {
                    EnableQuest(
                        popupEvent.buildingType, popupEvent.questText, popupEvent.deadlineInDays, popupEvent.unitSize,
                        popupEvent.turnsToWork, popupEvent.turnsToRest, popupEvent.materialsToGet, popupEvent.materialsToLose,
                        popupEvent.stabilityToGet, popupEvent.stabilityToLose, popupEvent.relationshipWithGovToGet, popupEvent.relationshipWithGovToLose);
                }
            })
            .AddTo(this);

        EventController.OnNewActionPoints
            .Subscribe(popupEvent =>
            {
                EnableNewActionPointsQuest();
            })
            .AddTo(this);
        
        SetButtonState(createNewUnitBtnParent,true);
    }

    private void EnableNewActionPointsQuest()
    {
        enableNewActionPointsQuestGO.SetActive(true);
        enableNewActionPointsQuestGO.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(CreateNewActionPoints);
    }

    private void CreateNewActionPoints()
    {
        if (CanCreateNewActionPoints())
        {
            _building.ActionPointsStartedCreating(amountOfReadyUnitsToCreateNewActionPoints);
            enableNewActionPointsQuestGO.SetActive(false);
            //Destroy(enableNewActionPointsQuestGO);
        }
    }
    private bool CanCreateNewActionPoints()
    {
        return HasEnoughPeople(amountOfReadyUnitsToCreateNewActionPoints) &&
               CanUseActionPoint();
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
               HasEnoughPeople(BuildingController.GetFoodTruckBuilding().PeopleToGiveProvision) &&
               CanUseActionPoint();
    }

    private void UpdateAllText()
    {
        relationWithGovernmentText.text = $"Отношение с правительством {GetRelationText(_building.RelationWithGovernment)} {_building.RelationWithGovernment}";

        militaryTimerText.text = _building.IsMaterialsSent
            ? "Военная помощь отправлена, ожидайте указаний"
            : $"До конца отправки воен помощи {_building.DaysLeftToSendArmyMaterials} дн.";

        helpFromGovTimerText.text = $"Помощь от гос-ва прибудет через {_building.DaysLeftToReceiveGovHelp} дн.";

        createPeopleText.text = $"Организовать новое подразделение, осталось {PeopleUnitsController.NotCreatedUnits.Count}";
    }

    private string GetRelationText(int relation)
    {
        string relationText;
        string colorHex;

        switch (relation)
        {
            case >= 0 and <= 3:
                relationText = "Плохое";
                colorHex = ColorUtility.ToHtmlStringRGB(badRelationColor);
                break;
            case >= 4 and <= 8:
                relationText = "Хорошее";
                colorHex = ColorUtility.ToHtmlStringRGB(averageRelationColor);
                break;
            default:
                relationText = "Отличное";
                colorHex = ColorUtility.ToHtmlStringRGB(goodRelationColor);
                break;
        }

        return $"<color=#{colorHex}>{relationText}</color>";
    }

    private void UpdateCreateUnitGOButtonState()
    {
        if (PeopleUnitsController.NotCreatedUnits.Count > 0)
        {
            SetButtonState(createNewUnitBtnParent,true);
        }
        else
        {
            Destroy(createPeopleText.gameObject);
        }
    }
}
