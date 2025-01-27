using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestPopUp : EnoughPopUp
{
    [SerializeField] private List<GameObject> questObjects;

    public void EnableQuest(string questName,int unitSize, int turnsToWork, int turnsToRest,
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

                    completeButton.onClick.AddListener(() => DisableQuest(quest));
                }

                Debug.Log($"������� {questName} ������������: {quest.name}");
                return;
            }
        }

        Debug.LogWarning($"��� ��������� ������� ��� ���������.");
    }

    private void DisableQuest(GameObject quest)
    {
        quest.SetActive(false);
        Debug.Log($"������� {quest.name} ��������� � ���������.");
    }
}
