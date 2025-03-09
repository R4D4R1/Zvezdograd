using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestPopUp : EnoughPopUp
{
    [SerializeField] private List<GameObject> questObjects;
    [SerializeField] protected GameObject activeBtn;
    [SerializeField] protected GameObject inactiveBtn;

    private Dictionary<GameObject, (int deadline, int stabilityToGet, int stabilityToLose, int relationshipWithGovToGet, int relationshipWithGovToLose)> questDeadlines = new Dictionary<GameObject, (int, int, int, int, int)>();

    public enum QuestType
    {
        Provision,
        Medicine,
        CityBuilding
    }

    protected virtual void Start()
    {
        ControllersManager.Instance.timeController.OnNextDayEvent += OnNextDay;
    }

    private void OnDisable()
    {
        ControllersManager.Instance.timeController.OnNextDayEvent -= OnNextDay;
    }

    public void EnableQuest(QuestType questType, string questName, int deadlineInDays, int unitSize, int turnsToWork, int turnsToRest,
    int materialsToGet, int materialsToLose, int stabilityToGet, int stabilityToLose,
    int relationshipWithGovToGet, int relationshipWithGovToLose)
    {
        foreach (var quest in questObjects)
        {
            if (!quest.activeSelf)
            {
                quest.SetActive(true);
                quest.GetComponent<TextMeshProUGUI>().text = questName + "  " + deadlineInDays + " дн.";

                Button completeButton = quest.GetComponentInChildren<Button>();
                if (completeButton != null)
                {
                    completeButton.onClick.RemoveAllListeners();
                    completeButton.onClick.AddListener(() =>
                    {
                        if (!CheckForEnoughPeople(unitSize))
                        {
                            _errorText.text = "Недостаточно людей!";
                            return;
                        }

                        ResourceController resourceController = ControllersManager.Instance.resourceController;

                        bool canComplete = false;
                        switch (questType)
                        {
                            case QuestType.Provision:
                                canComplete = CheckResourceAvailability(resourceController.GetProvision(), resourceController.GetMaxProvision(),
                                    materialsToGet, materialsToLose, "еды");
                                if (canComplete)
                                {
                                    resourceController.ModifyResource(ResourceController.ResourceType.Provision, materialsToGet);
                                    resourceController.ModifyResource(ResourceController.ResourceType.Provision, -materialsToLose);
                                }
                                break;

                            case QuestType.Medicine:
                                canComplete = CheckResourceAvailability(resourceController.GetMedicine(), resourceController.GetMaxMedicine(),
                                    materialsToGet, materialsToLose, "медикаментов");
                                if (canComplete)
                                {
                                    resourceController.ModifyResource(ResourceController.ResourceType.Medicine, materialsToGet);
                                    resourceController.ModifyResource(ResourceController.ResourceType.Medicine, -materialsToLose);
                                }
                                break;

                            case QuestType.CityBuilding:
                                canComplete = CheckResourceAvailability(resourceController.GetReadyMaterials(), resourceController.GetMaxReadyMaterials(),
                                    materialsToGet, materialsToLose, "стройматериалов");
                                if (canComplete)
                                {
                                    resourceController.ModifyResource(ResourceController.ResourceType.ReadyMaterials, materialsToGet);
                                    resourceController.ModifyResource(ResourceController.ResourceType.ReadyMaterials, -materialsToLose);
                                }
                                break;
                        }

                        if (canComplete)
                        {
                            ControllersManager.Instance.peopleUnitsController.AssignUnitsToTask(unitSize, turnsToWork, turnsToRest);
                            CompleteQuest(quest, stabilityToGet, relationshipWithGovToGet);
                        }
                    });
                }

                questDeadlines[quest] = (deadlineInDays, stabilityToGet, stabilityToLose, relationshipWithGovToGet, relationshipWithGovToLose);

                //Debug.Log($"Задание {questName} активировано: {quest.name} с дедлайном {deadlineInDays} дней");
                return;
            }
        }

        //Debug.LogWarning("Нет доступных заданий для активации.");
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
                    //Debug.Log($"Квест {quest.name} не выполнен в срок и завершён.");
                    LoseQuest(quest, data.stabilityToLose, data.relationshipWithGovToLose);
                }
            }
        }
    }

    private void CompleteQuest(GameObject quest, int stabilityToGet, int relationshipWithGovToGet)
    {
        ControllersManager.Instance.resourceController.ModifyResource(ResourceController.ResourceType.Stability, stabilityToGet);
        ControllersManager.Instance.buildingController.GetCityHallBuilding().ModifyRelationWithGov(relationshipWithGovToGet);

        quest.SetActive(false);
        questDeadlines.Remove(quest);
        //Debug.Log($"Задание {quest.name} завершено и отключено.");
    }

    private void LoseQuest(GameObject quest, int stabilityToLose, int relationshipWithGovToLose)
    {
        ControllersManager.Instance.resourceController.ModifyResource(ResourceController.ResourceType.Stability, -stabilityToLose);
        ControllersManager.Instance.buildingController.GetCityHallBuilding().ModifyRelationWithGov(-relationshipWithGovToLose);

        quest.SetActive(false);
        questDeadlines.Remove(quest);
        //Debug.Log($"Задание {quest.name} завершено и отключено.");
    }

    /// <summary>
    /// Проверяет, можно ли выполнить квест с текущими ресурсами.
    /// </summary>
    private bool CheckResourceAvailability(int currentAmount, int maxAmount, int materialsToGet, int materialsToLose, string resourceName)
    {
        if (currentAmount < materialsToLose)
        {
            _errorText.enabled = true;
            _errorText.text = $"Недостаточно {resourceName}!";
            return false;
        }
        if (currentAmount + materialsToGet > maxAmount)
        {
            _errorText.enabled = true;
            _errorText.text = $"Нет места для {resourceName}!";
            return false;
        }
        return true;
    }

    protected void SetButtonState(bool isActive)
    {
        activeBtn.SetActive(isActive);
        inactiveBtn.SetActive(!isActive);
    }
}
