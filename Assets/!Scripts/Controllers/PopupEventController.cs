using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Events;

public class PopupEventController : MonoBehaviour
{
    [SerializeField] private TextAsset _specificEventsJson;
    [SerializeField] private UnityEvent _onSnowStarted;

    private bool _isGameOver = false;

    private Dictionary<string, PopupEvent> _specificEvents;

    public event Action OnGameOver;

    private void OnEnable()
    {
        ControllersManager.Instance.buildingController.GetCityHallBuilding().OnGameWon += OnGameWonEventShow;
        ControllersManager.Instance.resourceController.OnStabilityZero += OnGameOverEventShow;
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += OnPeriodChanged;
    }

    private void OnDisable()
    {
        ControllersManager.Instance.buildingController.GetCityHallBuilding().OnGameWon -= OnGameWonEventShow;
        ControllersManager.Instance.resourceController.OnStabilityZero -= OnGameOverEventShow;
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed -= OnPeriodChanged;
    }

    private void Start()
    {
        LoadEvents();
    }

    private void LoadEvents()
    {
        _specificEvents = new Dictionary<string, PopupEvent>();
        var specificEventsData = JsonConvert.DeserializeObject<PopupEventData>(_specificEventsJson.text);
        foreach (var e in specificEventsData.events)
        {
            string eventKey = e.date + e.period;
            _specificEvents[eventKey] = e;
        }
    }

    private void OnPeriodChanged()
    {
        if (_isGameOver)
        {
            OnGameOver?.Invoke();
            return;
        }

        DateTime currentDate = ControllersManager.Instance.timeController.CurrentDate;
        string currentPeriod = ControllersManager.Instance.timeController.CurrentPeriod.ToString();

        string eventKey = currentDate.ToString("yyyy-MM-dd") + currentPeriod;

        if (_specificEvents.TryGetValue(eventKey, out PopupEvent popupEvent))
        {
            if (!string.IsNullOrEmpty(popupEvent.weatherType))
            {
                if (popupEvent.weatherType == "����")
                {
                    _onSnowStarted?.Invoke();
                }
            }
            if (!string.IsNullOrEmpty(popupEvent.buildingType))
            {
                // ���������, �������� �� ��� ������ ����� �� ����������
                if (popupEvent.buildingType == "���" || popupEvent.buildingType == "�����������" || popupEvent.buildingType == "�����")
                {
                    if (popupEvent.buildingType == "���")
                    {
                        ControllersManager.Instance.popUpsController.FoodTrucksPopUp.EnableQuest(QuestPopUp.QuestType.Provision, popupEvent.questText, popupEvent.unitSize, popupEvent.deadlineInDays,
                            popupEvent.turnsToWork, popupEvent.turnsToRest, popupEvent.materialsToGet, popupEvent.materialsToLose,
                            popupEvent.stabilityToGet, popupEvent.stabilityToLose, popupEvent.relationshipWithGovToGet, popupEvent.relationshipWithGovToLose);
                    }
                    else if (popupEvent.buildingType == "�����������")
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
    private void OnGameWonEventShow()
    {
        _isGameOver = true;
        EventPopUp.Instance.ShowEventPopUp(
            "������",
            "����� ������ � ��������� ����� �� ��� �� ��������� �������� ������, �� ����� �����...",
            "�� �������");
    }

    private void OnGameOverEventShow()
    {
        _isGameOver = true;
        EventPopUp.Instance.ShowEventPopUp(
            "��� �������",
            "�� �� ������ �������� ��� ����� � ������������ � ����, ��-�� ���� � ���� ����������� ���������������",
            "�����");

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

    public string weatherType;

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
