using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class QuestPopUp : EnoughPopUp
{
    [SerializeField] private List<GameObject> questObjects;

    private Dictionary<GameObject, int> questDeadlines = new Dictionary<GameObject, int>();

    public enum QuestType
    {
        Food,
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
                    completeButton.onClick.RemoveAllListeners(); // Убираем старые подписки на событие
                    completeButton.onClick.AddListener(() =>
                    {
                        ControllersManager.Instance.peopleUnitsController.AssignUnitsToTask(unitSize, turnsToWork, turnsToRest);

                        ControllersManager.Instance.resourceController.AddOrRemoveStability(stabilityToGet);
                        ControllersManager.Instance.resourceController.AddOrRemoveStability(-stabilityToLose);
                        ControllersManager.Instance.buildingController.GetCityHallBuilding().AddRelationWithGov(relationshipWithGovToGet);
                        ControllersManager.Instance.buildingController.GetCityHallBuilding().AddRelationWithGov(-relationshipWithGovToLose);

                        if (questType == QuestType.Food)
                        {
                            ControllersManager.Instance.resourceController.AddOrRemoveProvision(materialsToGet);
                            ControllersManager.Instance.resourceController.AddOrRemoveProvision(-materialsToLose);
                        }
                        if (questType == QuestType.Medicine)
                        {
                            ControllersManager.Instance.resourceController.AddOrRemoveProvision(materialsToGet);
                            ControllersManager.Instance.resourceController.AddOrRemoveProvision(-materialsToLose);
                        }
                        if (questType == QuestType.CityBuilding)
                        {
                            ControllersManager.Instance.resourceController.AddOrRemoveReadyMaterials(materialsToGet);
                            ControllersManager.Instance.resourceController.AddOrRemoveReadyMaterials(-materialsToLose);
                        }

                        DisableQuest(quest);
                    });
                }

                // Добавляем квест в словарь с отслеживанием дней
                questDeadlines[quest] = deadlineInDays;

                Debug.Log($"Задание {questName} активировано: {quest.name} с дедлайном {deadlineInDays} дней");
                return;
            }
        }

        Debug.LogWarning($"Нет доступных заданий для активации.");
    }

    private void OnNextDay()
    {
        if (questDeadlines.Count > 0)
        {
            Debug.Log("NEW DAY FOR DEADLINE");

            foreach (var quest in questDeadlines.Keys.ToList()) // Создаём копию ключей
            {
                questDeadlines[quest]--; // Уменьшаем дедлайн на 1 день

                if (questDeadlines[quest] <= 0)
                {
                    Debug.Log($"Квест {quest.name} не выполнен в срок и завершён.");
                    DisableQuest(quest); // Сразу отключаем квест
                }
            }
        }
    }

    private void DisableQuest(GameObject quest)
    {
        quest.SetActive(false);
        questDeadlines.Remove(quest);
        Debug.Log($"Задание {quest.name} завершено и отключено.");
    }
}
