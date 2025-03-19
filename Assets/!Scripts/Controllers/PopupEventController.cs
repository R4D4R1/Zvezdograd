using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;
using Zenject;
using System.IO;
using UniRx;

public class PopupEventController : MonoBehaviour
{
    private bool _isGameOver = false;

    private Dictionary<string, PopupEvent> _specificEvents;

    protected ControllersManager _controllersManager;
    protected ResourceViewModel _resourceViewModel;
    protected EventPopUp _eventPopUp;

    public event Action OnSnowStartedEvent;

    [Inject]
    public void Construct(ControllersManager controllersManager, ResourceViewModel resourceViewModel,EventPopUp eventPopUp)
    {
        _controllersManager = controllersManager;
        _resourceViewModel = resourceViewModel;
        _eventPopUp = eventPopUp;
    }

    public void Init()
    {
        _controllersManager.TimeController.OnNextTurnBtnClickBetween
            .Subscribe(_ => OnPeriodChanged())
            .AddTo(this);

        LoadEvents();
    }

    private void LoadEvents()
    {
        _specificEvents = new Dictionary<string, PopupEvent>();

        string jsonPath = Path.Combine(Application.streamingAssetsPath, "specificEvents.json");

        if (!File.Exists(jsonPath))
        {
            Debug.LogError($"���� �� ������ �� ����: {jsonPath}");
            return;
        }

        string jsonContent = File.ReadAllText(jsonPath);
        var specificEventsData = JsonConvert.DeserializeObject<PopupEventData>(jsonContent);

        foreach (var e in specificEventsData.events)
        {
            string eventKey = e.date + e.period;
            _specificEvents[eventKey] = e;
        }
    }

    private async void OnPeriodChanged()
    {
        // Delay to check WIN/LOSE condition
        await UniTask.Delay(1);

        if (_controllersManager.MainGameController.GameOverState == MainGameController.GameOverStateEnum.Win)
        {
            OnGameWonEventShow();
        }
        else if (_controllersManager.MainGameController.GameOverState == MainGameController.GameOverStateEnum.Lose)
        {
            OnGameLoseEventShow();
        }

        if (_isGameOver)
        {
            return;
        }

        DateTime currentDate = _controllersManager.TimeController.CurrentDate;
        string currentPeriod = _controllersManager.TimeController.CurrentPeriod.ToString();

        string eventKey = currentDate.ToString("yyyy-MM-dd") + currentPeriod;

        if (_specificEvents.TryGetValue(eventKey, out PopupEvent popupEvent))
        {
            if (!string.IsNullOrEmpty(popupEvent.weatherType))
            {
                if (popupEvent.weatherType == "����")
                {
                    OnSnowStartedEvent?.Invoke();
                }
            }

            // Handle building type and popup events
            if (!string.IsNullOrEmpty(popupEvent.buildingType))
            {
                // Check for specific building types (Food, Medicine, City Hall, etc.)
                HandleBuildingTypePopup(popupEvent);
            }

            // Show the popup
            _eventPopUp.ShowEventPopUp(
                popupEvent.title,
                popupEvent.mainText,
                popupEvent.buttonText);
        }
    }

    private void HandleBuildingTypePopup(PopupEvent popupEvent)
    {
        if (popupEvent.buildingType == "���")
        {
            _controllersManager.PopUpsController.FoodTrucksPopUp.EnableQuest(QuestPopUp.QuestType.Provision, popupEvent.questText, popupEvent.unitSize, popupEvent.deadlineInDays,
                popupEvent.turnsToWork, popupEvent.turnsToRest, popupEvent.materialsToGet, popupEvent.materialsToLose,
                popupEvent.stabilityToGet, popupEvent.stabilityToLose, popupEvent.relationshipWithGovToGet, popupEvent.relationshipWithGovToLose);
        }
        else if (popupEvent.buildingType == "�����������")
        {
            _controllersManager.PopUpsController.HospitalPopUp.EnableQuest(QuestPopUp.QuestType.Medicine, popupEvent.questText, popupEvent.unitSize, popupEvent.deadlineInDays,
                popupEvent.turnsToWork, popupEvent.turnsToRest, popupEvent.materialsToGet, popupEvent.materialsToLose,
                popupEvent.stabilityToGet, popupEvent.stabilityToLose, popupEvent.relationshipWithGovToGet, popupEvent.relationshipWithGovToLose);
        }
        else if (popupEvent.buildingType == "�����")
        {
            _controllersManager.PopUpsController.CityHallPopUp.EnableQuest(QuestPopUp.QuestType.CityBuilding, popupEvent.questText, popupEvent.unitSize, popupEvent.deadlineInDays,
                popupEvent.turnsToWork, popupEvent.turnsToRest, popupEvent.materialsToGet, popupEvent.materialsToLose,
                popupEvent.stabilityToGet, popupEvent.stabilityToLose, popupEvent.relationshipWithGovToGet, popupEvent.relationshipWithGovToLose);
        }
        else
        {
            Debug.LogWarning($"Unknown Building Type: {popupEvent.buildingType}");
        }
    }

    private void OnGameWonEventShow()
    {
        _isGameOver = true;
        _eventPopUp.ShowEventPopUp(
            "������",
            "����� ������ � ��������� ����� �� ��� �� ��������� �������� ������, �� ����� �����...",
            "�� �������");
    }

    private void OnGameLoseEventShow()
    {
        _isGameOver = true;
        _eventPopUp.ShowEventPopUp(
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
