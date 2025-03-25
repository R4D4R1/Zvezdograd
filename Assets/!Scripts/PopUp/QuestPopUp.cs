using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class QuestPopUp : EnoughPopUp
{
    [SerializeField] private List<GameObject> questObjects;

    private Dictionary<GameObject, (int deadline, int stabilityToGet, int stabilityToLose, int relationshipWithGovToGet, int relationshipWithGovToLose)> questDeadlines = new Dictionary<GameObject, (int, int, int, int, int)>();

    public enum QuestType
    {
        Provision,
        Medicine,
        CityBuilding
    }

    public override void Init()
    {
        TimeController.OnNextDayEvent
            .Subscribe(_ => OnNextDay())
            .AddTo(this);
    }

    public void EnableQuest(QuestType questType, string questName, int deadlineInDays, int unitSize, int turnsToWork, int turnsToRest,
    int materialsToGet, int materialsToLose, int stabilityToGet, int stabilityToLose,
    int relationshipWithGovToGet, int relationshipWithGovToLose)
    {
        foreach (var quest in questObjects.Where(quest => !quest.activeSelf))
        {
            quest.SetActive(true);
            quest.GetComponent<TextMeshProUGUI>().text = questName + "  " + deadlineInDays + " DS.";

            var completeButton = quest.GetComponentInChildren<Button>();
            if (completeButton)
            {
                completeButton.onClick.RemoveAllListeners();
                completeButton.onClick.AddListener(() =>
                {
                    if (!HasEnoughPeople(unitSize))
                    {
                        errorText.text = "NOT ENOUGH PEOPLE!";
                        return;
                    }

                    if (!CanUseActionPoint())
                        return;

                    bool canComplete = false;
                    switch (questType)
                    {
                        case QuestType.Provision:
                            canComplete = CheckResourceAvailability(ResourceViewModel.Provision.Value, ResourceViewModel.Model.MaxProvision,
                                materialsToGet, materialsToLose, "FOOD");
                            if (canComplete)
                            {
                                ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Provision,
                                    materialsToGet));
                                ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Provision,
                                    -materialsToLose));
                            }

                            break;

                        case QuestType.Medicine:
                            canComplete = CheckResourceAvailability(ResourceViewModel.Medicine.Value, ResourceViewModel.Model.MaxMedicine,
                                materialsToGet, materialsToLose, "MEDICINE");
                            if (canComplete)
                            {
                                ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Medicine, materialsToGet));
                                ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Medicine, -materialsToLose));
                            }
                            break;

                        case QuestType.CityBuilding:
                            canComplete = CheckResourceAvailability(ResourceViewModel.ReadyMaterials.Value, ResourceViewModel.Model.MaxReadyMaterials,
                                materialsToGet, materialsToLose, "BUILDING MATERIALS");
                            if (canComplete)
                            {
                                ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.ReadyMaterials, materialsToGet));
                                ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.ReadyMaterials, -materialsToLose));
                            }
                            break;
                    }

                    if (canComplete)
                    {
                        PeopleUnitsController.AssignUnitsToTask(unitSize, turnsToWork, turnsToRest);
                        CompleteQuest(quest, stabilityToGet, relationshipWithGovToGet);
                    }
                });
            }

            questDeadlines[quest] = (deadlineInDays, stabilityToGet, stabilityToLose, relationshipWithGovToGet, relationshipWithGovToLose);

            return;
        }
    }

    private void OnNextDay()
    {
        if (questDeadlines.Count > 0)
        {
            foreach (var quest in questDeadlines.Keys.ToList())
            {
                var data = questDeadlines[quest];
                questDeadlines[quest] = (data.deadline - 1, data.stabilityToGet, data.stabilityToLose, data.relationshipWithGovToGet, data.relationshipWithGovToLose);

                if (questDeadlines[quest].deadline <= 0)
                {
                    LoseQuest(quest, data.stabilityToLose, data.relationshipWithGovToLose);
                }
            }
        }
    }

    private void CompleteQuest(GameObject quest, int stabilityToGet, int relationshipWithGovToGet)
    {
        ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Stability, stabilityToGet));
        
        BuildingController.GetCityHallBuilding().ModifyRelationWithGov(relationshipWithGovToGet);

        quest.SetActive(false);
        questDeadlines.Remove(quest);
    }

    private void LoseQuest(GameObject quest, int stabilityToLose, int relationshipWithGovToLose)
    {
        ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Stability, -stabilityToLose));
        BuildingController.GetCityHallBuilding().ModifyRelationWithGov(-relationshipWithGovToLose);

        quest.SetActive(false);
        questDeadlines.Remove(quest);
    }
    private bool CheckResourceAvailability(int currentAmount, int maxAmount, int materialsToGet, int materialsToLose, string resourceName)
    {
        if (currentAmount < materialsToLose)
        {
            errorText.enabled = true;
            errorText.text = $"NOT ENOUGH {resourceName}!";
            return false;
        }
        if (currentAmount + materialsToGet > maxAmount)
        {
            errorText.enabled = true;
            errorText.text = $"NOT ENOUGH SPACE FOR {resourceName}!";
            return false;
        }
        return true;
    }

    protected void SetButtonState(GameObject btnsParent, bool isActive)
    {
        btnsParent.transform.GetChild(0).gameObject.SetActive(isActive);
        btnsParent.transform.GetChild(1).gameObject.SetActive(!isActive);
    }
}
