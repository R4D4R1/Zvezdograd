using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;
using Zenject;
using System.IO;
using UniRx;

public class PopupEventController : MonoInit
{
    private bool _isGameOver;

    private Dictionary<string, PopupEvent> _specificEvents;

    private EventPopUp _eventPopUp;
    private TimeController _timeController;
    private MainGameController _mainGameController;
    private PopUpsController _popUpsController;

    public readonly Subject<Unit> OnSnowStarted = new();

    [Inject]
    public void Construct(EventPopUp eventPopUp, TimeController timeController,
        MainGameController mainGameController,PopUpsController popUpsController)
    {
        _eventPopUp = eventPopUp;
        _timeController = timeController;
        _mainGameController = mainGameController;
        _popUpsController = popUpsController;
    }

    public override void Init()
    {
        base.Init();
        _timeController.OnNextTurnBtnClickBetween
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
            Debug.LogError($"NO FILE IN PATH : {jsonPath}");
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

        if (_mainGameController.GameOverState == MainGameController.GameOverStateEnum.Win)
        {
            OnGameWonEventShow();
        }
        else if (_mainGameController.GameOverState == MainGameController.GameOverStateEnum.Lose)
        {
            OnGameLoseEventShow();
        }

        if (_isGameOver)
        {
            return;
        }

        DateTime currentDate = _timeController.CurrentDate;
        string currentPeriod = _timeController.CurrentPeriod.ToString();

        string eventKey = currentDate.ToString("yyyy-MM-dd") + currentPeriod;

        if (_specificEvents.TryGetValue(eventKey, out PopupEvent popupEvent))
        {
            if (!string.IsNullOrEmpty(popupEvent.weatherType))
            {
                if (popupEvent.weatherType == "снег")
                {
                    OnSnowStarted.OnNext(Unit.Default);
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
        if (popupEvent.buildingType == "Еда")
        {
            _popUpsController.FoodTrucksPopUp.EnableQuest(QuestPopUp.QuestType.Provision, popupEvent.questText, popupEvent.unitSize, popupEvent.deadlineInDays,
                popupEvent.turnsToWork, popupEvent.turnsToRest, popupEvent.materialsToGet, popupEvent.materialsToLose,
                popupEvent.stabilityToGet, popupEvent.stabilityToLose, popupEvent.relationshipWithGovToGet, popupEvent.relationshipWithGovToLose);
        }
        else if (popupEvent.buildingType == "Медикаменты")
        {
            _popUpsController.HospitalPopUp.EnableQuest(QuestPopUp.QuestType.Medicine, popupEvent.questText, popupEvent.unitSize, popupEvent.deadlineInDays,
                popupEvent.turnsToWork, popupEvent.turnsToRest, popupEvent.materialsToGet, popupEvent.materialsToLose,
                popupEvent.stabilityToGet, popupEvent.stabilityToLose, popupEvent.relationshipWithGovToGet, popupEvent.relationshipWithGovToLose);
        }
        else if (popupEvent.buildingType == "Стройматериалы")
        {
            _popUpsController.CityHallPopUp.EnableQuest(QuestPopUp.QuestType.CityBuilding, popupEvent.questText, popupEvent.unitSize, popupEvent.deadlineInDays,
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
            "ПОБЕДА",
            "ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА" +
            " ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ",
            "ГЛАВНОЕ МЕНЮ");
    }

    private void OnGameLoseEventShow()
    {
        _isGameOver = true;
        _eventPopUp.ShowEventPopUp(
            "ВЫ ПРОИГРАЛИ",
            "ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА" +
            " ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ",
            "ГЛАВНОЕ МЕНЮ");
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
