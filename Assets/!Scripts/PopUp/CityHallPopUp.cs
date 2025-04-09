using TMPro;
using UnityEngine;
using UniRx;
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
    
    private CityHallBuilding _cityHallBuilding;
    private bool LastMilitaryHelpSent;
    public override void Init()
    {
        base.Init();

        _cityHallBuilding = BuildingController.GetCityHallBuilding();

        PeopleUnitsController.OnUnitCreatedByPeopleUnitController
            .Subscribe(_ => UpdateCreateUnitGOButtonState())
            .AddTo(this);

        EventController.OnQuestTriggered
            .Subscribe(popupEvent =>
            {
                if (popupEvent.buildingType == _cityHallBuilding.Type.ToString())
                {
                    EnableQuest(
                        popupEvent.buildingType, popupEvent.questText, popupEvent.deadlineInDays, popupEvent.unitSize,
                        popupEvent.turnsToWork, popupEvent.turnsToRest, popupEvent.materialsToGet,
                        popupEvent.materialsToLose,
                        popupEvent.stabilityToGet, popupEvent.stabilityToLose, popupEvent.relationshipWithGovToGet,
                        popupEvent.relationshipWithGovToLose);
                }
            })
            .AddTo(this);

        EventController.OnNewActionPointsAdded
            .Subscribe(_ => EnableNewActionPointsQuest())
            .AddTo(this);

        _cityHallBuilding.OnLastMilitaryHelpSent
            .Subscribe(_ =>
            {
                LastMilitaryHelpSent = true;
            })
            .AddTo(this);
        
        LastMilitaryHelpSent = false;
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
            _cityHallBuilding.ActionPointsStartedCreating(amountOfReadyUnitsToCreateNewActionPoints);
            enableNewActionPointsQuestGO.SetActive(false);
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
            _cityHallBuilding.NewUnitStartedCreating();
            SetButtonState(createNewUnitBtnParent,false);
        }
    }

    private bool CanCreateNewUnit()
    {
        return HasEnoughResources(ResourceModel.ResourceType.ReadyMaterials,
               _cityHallBuilding.CityHallBuildingConfig.ReadyMaterialsToCreateNewPeopleUnit) &&
               CanUseActionPoint();
    }

    protected override void UpdateAllText()
    {
        if (LastMilitaryHelpSent)
        {
            militaryTimerText.text = $"Вся военная техника отправлена," +
                                     $" дождитесь спасения ";
        }
        else
        {
            militaryTimerText.text = _cityHallBuilding.IsMaterialsSent
                ? "Военная помощь отправлена, ожидайте указаний"
                : $"До конца отправки воен помощи {_cityHallBuilding.DaysLeftToSendArmyMaterials} дн.";
        }
        
        relationWithGovernmentText.text = $"Отношение с правительством {GetRelationText(_cityHallBuilding.RelationWithGovernment)}" +
                                          $" {_cityHallBuilding.RelationWithGovernment}";
        
        helpFromGovTimerText.text = $"Помощь от гос-ва прибудет через {_cityHallBuilding.DaysLeftToReceiveGovHelp} дн.";

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

    // METHOD TO ENABLE CREATE NEW UNITS BUTTON OR DEACTIVATE IT WHEN NO MORE UNITS TO CREATE
    private void UpdateCreateUnitGOButtonState()
    {
        if (PeopleUnitsController.NotCreatedUnits.Count > 0)
        {
            SetButtonState(createNewUnitBtnParent,true);
        }
        else
        {
            createPeopleText.gameObject.SetActive(false);
        }
    }
    
    public override void LoadFromSaveData(PopUpSaveData data)
    {
        var save = data as QuestPopUpSaveData;
        if (save == null) return;
        
        IsBtnActive = save.isBtnActive;
        SetButtonState(createNewUnitBtnParent,IsBtnActive);
        UpdateAllText();
    }
}
