using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class PopupEventController : MonoBehaviour
{
    [SerializeField] private TextAsset _specificEventsJson;

    [Range(0, 100)]
    [SerializeField] private int _randomEventChance;

    private Dictionary<string, PopupEvent> specificEvents;

    private void Start()
    {
        LoadEvents();
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += OnPeriodChanged;
    }

    private void LoadEvents()
    {
        specificEvents = new Dictionary<string, PopupEvent>();
        var specificEventsData = JsonConvert.DeserializeObject<PopupEventData>(_specificEventsJson.text);
        foreach (var e in specificEventsData.events)
        {
            string eventKey = e.date + e.period;
            specificEvents[eventKey] = e;
        }
    }

    private void OnPeriodChanged()
    {
        DateTime currentDate = ControllersManager.Instance.timeController.CurrentDate;
        string currentPeriod = ControllersManager.Instance.timeController.CurrentPeriod.ToString();

        string eventKey = currentDate.ToString("yyyy-MM-dd") + currentPeriod;

        if (ControllersManager.Instance.resourceController.IsStabilityZero)
        {
            // SHOW GAME OVER POP UP
            EventPopUp.Instance.ShowEventPopUp(
                "��� �������",
                "�� �� ������ �������� ��� ����� � ������������ � ����, ��-�� ���� � ���� ����������� ���������������",
                "�����");
        }
        else
        {
            if (specificEvents.TryGetValue(eventKey, out PopupEvent popupEvent))
            {
                if (!string.IsNullOrEmpty(popupEvent.buildingType))
                {
                    // ���������, �������� �� ��� ������ ����� �� ����������
                    if (popupEvent.buildingType == "���" || popupEvent.buildingType == "��������" || popupEvent.buildingType == "�����")
                    {
                        if (popupEvent.buildingType == "���")
                        {
                            ControllersManager.Instance.popUpsController.FoodTrucksPopUp.EnableQuest(QuestPopUp.QuestType.Provision, popupEvent.questText, popupEvent.unitSize, popupEvent.deadlineInDays,
                                popupEvent.turnsToWork, popupEvent.turnsToRest, popupEvent.materialsToGet, popupEvent.materialsToLose,
                                popupEvent.stabilityToGet, popupEvent.stabilityToLose, popupEvent.relationshipWithGovToGet, popupEvent.relationshipWithGovToLose);
                        }
                        else if (popupEvent.buildingType == "��������")
                        {
                            ControllersManager.Instance.popUpsController.HospitalPopUp.EnableQuest(QuestPopUp.QuestType.Medicine, popupEvent.questText, popupEvent.unitSize, popupEvent.deadlineInDays,
                                popupEvent.turnsToWork, popupEvent.turnsToRest, popupEvent.materialsToGet, popupEvent.materialsToLose,
                                popupEvent.stabilityToGet, popupEvent.stabilityToLose, popupEvent.relationshipWithGovToGet, popupEvent.relationshipWithGovToLose);
                        }
                        else if (popupEvent.buildingType == "�����")
                        {
                            ControllersManager.Instance.popUpsController.CityHallPopUp.EnableQuest(QuestPopUp.QuestType.CityBuilding, popupEvent.questText, popupEvent.unitSize, popupEvent.deadlineInDays,
                                popupEvent.turnsToWork, popupEvent.turnsToRest, popupEvent.materialsToGet, popupEvent.materialsToLose,
                                popupEvent.stabilityToGet, popupEvent.stabilityToLose, popupEvent.relationshipWithGovToGet, popupEvent.relationshipWithGovToLose);
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Unknown Building Type: {popupEvent.buildingType}");
                    }
                }

                EventPopUp.Instance.ShowEventPopUp(
                    popupEvent.title,
                    popupEvent.mainText,
                    popupEvent.buttonText);
            }
        }
    }
}

[Serializable]
public class PopupEvent
{
    public string date;
    public string period;
    public string title;
    public string mainText;
    public string buttonText;

    // ���� ������� 
    public string buildingType;

    public string questText;

    public int deadlineInDays;

    public int unitSize;
           
    public int turnsToWork;
    public int turnsToRest;
           
    public int materialsToGet;
    public int materialsToLose;
           
    public int stabilityToGet;
    public int stabilityToLose;
           
    public int relationshipWithGovToGet;
    public int relationshipWithGovToLose;
}

[Serializable]
public class PopupEventData
{
    public List<PopupEvent> events;
}
