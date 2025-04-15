using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class QuestPopUp : EnoughPopUp, ISaveablePopUp
{
    [SerializeField] private GameObject questObjectPrefab;
    [SerializeField] private Transform questGroupTransform;

    private readonly Dictionary<GameObject, QuestData> _quests = new();
    public bool IsBtnActive;

    private struct QuestData
    {
        public string QuestName;
        public int DeadlineInDays;
        public int UnitSize;
        public int TurnsToWork;
        public int TurnsToRest;
        public int MaterialsToGet;
        public int MaterialsToLose;
        public int StabilityToGet;
        public int StabilityToLose;
        public int RelationshipWithGovToGet;
        public int RelationshipWithGovToLose;
        public string BuildingType;

        public QuestData(
            string questName, int deadlineInDays, int unitSize,
            int turnsToWork, int turnsToRest,
            int materialsToGet, int materialsToLose,
            int stabilityToGet, int stabilityToLose,
            int relationshipWithGovToGet, int relationshipWithGovToLose,
            string buildingType)
        {
            QuestName = questName;
            DeadlineInDays = deadlineInDays;
            UnitSize = unitSize;
            TurnsToWork = turnsToWork;
            TurnsToRest = turnsToRest;
            MaterialsToGet = materialsToGet;
            MaterialsToLose = materialsToLose;
            StabilityToGet = stabilityToGet;
            StabilityToLose = stabilityToLose;
            RelationshipWithGovToGet = relationshipWithGovToGet;
            RelationshipWithGovToLose = relationshipWithGovToLose;
            BuildingType = buildingType;
        }
    }


    public override void Init()
    {
        base.Init();
        TimeController.OnNextDayEvent
            .Subscribe(_ => OnNextDay())
            .AddTo(this);
    }

    private void OnNextDay()
    {
        foreach (var quest in _quests.Keys.ToList())
        {
            var data = _quests[quest];
            data.DeadlineInDays--;

            if (data.DeadlineInDays <= 0)
            {
                LoseQuest(quest, data.StabilityToLose, data.RelationshipWithGovToLose);
            }
            else
            {
                _quests[quest] = data;
                quest.GetComponent<TextMeshProUGUI>().text = $"{data.QuestName}  {data.DeadlineInDays} дн.";
            }
        }
    }

    protected void EnableQuest(
        string buildingType, string questName, int deadlineInDays, int unitSize,
        int turnsToWork, int turnsToRest,
        int materialsToGet, int materialsToLose,
        int stabilityToGet, int stabilityToLose,
        int relationshipWithGovToGet, int relationshipWithGovToLose)
    {
        var quest = Instantiate(questObjectPrefab, questGroupTransform);
        if (!quest) return;

        quest.SetActive(true);
        quest.GetComponent<TextMeshProUGUI>().text = $"{questName}  {deadlineInDays} дн.";

        var completeButton = quest.GetComponentInChildren<Button>();
        if (completeButton)
        {
            completeButton.onClick.RemoveAllListeners();
            completeButton.onClick.AddListener(() =>
            {
                if (!HasEnoughPeople(unitSize) || !CanUseActionPoint()) return;

                if (ProcessResources(buildingType, materialsToGet, materialsToLose))
                {
                    PeopleUnitsController.AssignUnitsToTask(unitSize, turnsToWork, turnsToRest);
                    CompleteQuest(quest, stabilityToGet, relationshipWithGovToGet);
                }
            });
        }

        _quests[quest] = new QuestData(
    questName, deadlineInDays, unitSize,
    turnsToWork, turnsToRest,
    materialsToGet, materialsToLose,
    stabilityToGet, stabilityToLose,
    relationshipWithGovToGet, relationshipWithGovToLose,
    buildingType);
    }

    private void CompleteQuest(GameObject quest, int stabilityToGet, int relationshipWithGovToGet)
    {
        ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Stability, stabilityToGet));
        BuildingsController.GetCityHallBuilding().ModifyRelationWithGov(relationshipWithGovToGet);

        quest.SetActive(false);
        _quests.Remove(quest);
    }

    private void LoseQuest(GameObject quest, int stabilityToLose, int relationshipWithGovToLose)
    {
        ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Stability, -stabilityToLose));
        BuildingsController.GetCityHallBuilding().ModifyRelationWithGov(-relationshipWithGovToLose);

        quest.SetActive(false);
        _quests.Remove(quest);
    }

    private bool ProcessResources(string buildingType, int materialsToGet, int materialsToLose)
    {
        var resourceType = GetResourceTypeByBuilding(buildingType);
        if (resourceType == null) return false;

        var type = resourceType.Value;
        if (!HasEnoughResources(type, materialsToLose)) return false;
        if (!HasEnoughSpaceForResources(type, materialsToGet)) return false;

        ResourceViewModel.ModifyResourceCommand.Execute((type, materialsToGet));
        ResourceViewModel.ModifyResourceCommand.Execute((type, -materialsToLose));
        return true;
    }

    private ResourceModel.ResourceType? GetResourceTypeByBuilding(string buildingType)
    {
        return buildingType switch
        {
            "FoodTrucks" => ResourceModel.ResourceType.Provision,
            "Hospital" => ResourceModel.ResourceType.Medicine,
            "CityHall" => ResourceModel.ResourceType.ReadyMaterials,
            _ => null
        };
    }

    protected void SetButtonState(GameObject btnsParent, bool activeState)
    {
        IsBtnActive = activeState;
        btnsParent.transform.GetChild(0).gameObject.SetActive(activeState);
        btnsParent.transform.GetChild(1).gameObject.SetActive(!activeState);
    }

    protected virtual void UpdateAllText()
    {
        // Обновление текста
    }

    public new int PopUpID => base.PopUpId;

    public virtual PopUpSaveData SaveData()
    {
        var saveData = new QuestPopUpSaveData
        {
            popUpID = PopUpID,
            isBtnActive = IsBtnActive
        };

        foreach (var quest in _quests.Values)
        {
            saveData.savedQuests.Add(new QuestSaveData
            {
                QuestName = quest.QuestName,
                DeadlineInDays = quest.DeadlineInDays,
                UnitSize = quest.UnitSize,
                TurnsToWork = quest.TurnsToWork,
                TurnsToRest = quest.TurnsToRest,
                MaterialsToGet = quest.MaterialsToGet,
                MaterialsToLose = quest.MaterialsToLose,
                StabilityToGet = quest.StabilityToGet,
                StabilityToLose = quest.StabilityToLose,
                RelationshipWithGovToGet = quest.RelationshipWithGovToGet,
                RelationshipWithGovToLose = quest.RelationshipWithGovToLose,
                BuildingType = quest.BuildingType
            });
        }
        return saveData;
    }

    public virtual void LoadData(PopUpSaveData data)
    {
        foreach (var quest in _quests.Keys.ToList())
        {
            Destroy(quest);
        }
        _quests.Clear();

        var save = data as QuestPopUpSaveData;
        if (save == null) return;

        IsBtnActive = save.isBtnActive;
        UpdateAllText();

        foreach (var questSave in save.savedQuests)
        {
            EnableQuest(
                questSave.BuildingType,
                questSave.QuestName,
                questSave.DeadlineInDays,
                questSave.UnitSize,
                questSave.TurnsToWork,
                questSave.TurnsToRest,
                questSave.MaterialsToGet,
                questSave.MaterialsToLose,
                questSave.StabilityToGet,
                questSave.StabilityToLose,
                questSave.RelationshipWithGovToGet,
                questSave.RelationshipWithGovToLose
            );
        }

    }
}
