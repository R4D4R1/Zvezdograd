using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;
using Zenject;
using System.IO;
using System.Threading.Tasks;
using UniRx;

public class EventController : MonoInit
{
    private bool _isGameOver;

    private Dictionary<string, PopupEvent> _specificEvents;

    private PopUpsController _popUpsController;
    private TimeController _timeController;
    private MainGameController _mainGameController;

    public readonly Subject<Unit> OnSnowStarted = new();
    public readonly Subject<PopupEvent> OnQuestTriggered = new();

    [Inject]
    public void Construct(PopUpsController popUpsController,TimeController timeController,
        MainGameController mainGameController)
    {
        _popUpsController = popUpsController;
        _timeController = timeController;
        _mainGameController = mainGameController;
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

        var jsonPath = Path.Combine(Application.streamingAssetsPath, "specificEvents.json");

        if (!File.Exists(jsonPath))
        {
            Debug.LogError($"NO FILE IN PATH : {jsonPath}");
            return;
        }

        var jsonContent = File.ReadAllText(jsonPath);
        var specificEventsData = JsonConvert.DeserializeObject<PopupEventData>(jsonContent);

        foreach (var e in specificEventsData.events)
        {
            var eventKey = e.date + e.period;
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
            OnStabilityGameLoseEventShow();
        }

        if (_isGameOver)
        {
            return;
        }

        var currentDate = _timeController.CurrentDate;
        var currentPeriod = _timeController.CurrentPeriod.ToString();

        var eventKey = currentDate.ToString("yyyy-MM-dd") + currentPeriod;

        if (_specificEvents.TryGetValue(eventKey, out var popupEvent))
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
                OnQuestTriggered.OnNext(popupEvent);
            }

            // Show the popup
            _popUpsController.EventPopUp.ShowEventPopUp(
                popupEvent.title,
                popupEvent.mainText,
                popupEvent.buttonText);
        }
    }

    private void OnGameWonEventShow()
    {
        _isGameOver = true;
        _popUpsController.EventPopUp.ShowEventPopUp(
            "ПОБЕДА",
            "ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА" +
            " ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ",
            "ГЛАВНОЕ МЕНЮ");
    }

    private void OnStabilityGameLoseEventShow()
    {
        _isGameOver = true;
        _popUpsController.EventPopUp.ShowEventPopUp(
            "ВЫ ПРОИГРАЛИ",
            "ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА" +
            " ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ПРИМЕР ТЕКСТА ",
            "ГЛАВНОЕ МЕНЮ");
    }
    
    private void OnWarMaterialGameLoseEventShow()
    {
        _isGameOver = true;
        _popUpsController.EventPopUp.ShowEventPopUp(
            "ДРУЖЕСТВЕННЫЕ ВОЙСКА НЕ УСПЕЛИ",
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
