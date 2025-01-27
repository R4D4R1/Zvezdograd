using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestPopUp : EnoughPopUp
{
    [SerializeField] private List<GameObject> questObjects;

    public enum QuestType
    {
        Food,
        Medicine,
        CityBuilding
    }

    public void EnableQuest(QuestType questType,string questName,int unitSize, int turnsToWork, int turnsToRest,
        int materialsToGet, int materialsToLose, int stabilityToGet, int stabilityToLose,
        int relationshipWithGovToGet, int relationshipWithGovToLose)
    {
        foreach (var quest in questObjects)
        {
            if (!quest.activeSelf)
            {
                quest.SetActive(true);

                quest.GetComponent<TextMeshProUGUI>().text = questName;

                Button completeButton = quest.GetComponentInChildren<Button>();
                if (completeButton != null)
                {
                    completeButton.onClick.AddListener(()=>
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

                Debug.Log($"Задание {questName} активировано: {quest.name}");
                return;
            }
        }

        Debug.LogWarning($"Нет доступных заданий для активации.");
    }

    private void DisableQuest(GameObject quest)
    {
        quest.SetActive(false);
        Debug.Log($"Задание {quest.name} завершено и отключено.");
    }
}
