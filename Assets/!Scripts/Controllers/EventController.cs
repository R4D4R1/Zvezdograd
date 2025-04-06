using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;
using Zenject;
using System.IO;
using UniRx;

public class EventController : MonoInit
{
    private bool _isGameOver;

    private Dictionary<string, PopupEvent> _specificEvents;

    private PopUpsController _popUpsController;
    private TimeController _timeController;
    private MainGameController _mainGameController;

    public readonly Subject<Unit> OnSnowStarted = new();
    public readonly Subject<Unit> OnNewActionPoints = new();
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

    private async void LoadEvents()
{
    _specificEvents = new Dictionary<string, PopupEvent>();

    const string FILE_NAME = "specificEvents.json";
    const string EVENT_FILE_URL = "https://drive.google.com/uc?export=download&id=1ntL2TWuR70Dpi8gooGGMTca0V9LKadUB";

    var persistentPath = Path.Combine(Application.persistentDataPath, FILE_NAME);

    string jsonContent;

    var isInternetAvailable = Application.internetReachability != NetworkReachability.NotReachable;

    if (isInternetAvailable)
    {
        Debug.Log("Есть интернет, пытаюсь скачать файл...");
        
        using var www = UnityEngine.Networking.UnityWebRequest.Get(EVENT_FILE_URL);
        
        await www.SendWebRequest();

        if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            jsonContent = www.downloadHandler.text;
            File.WriteAllText(persistentPath, jsonContent);
            Debug.Log("Файл успешно скачан и сохранён.");
        }
        else
        {
            Debug.LogWarning($"Ошибка при скачивании: {www.error}");

            if (File.Exists(persistentPath))
            {
                jsonContent = File.ReadAllText(persistentPath);
                Debug.Log("Загружен локальный кэш-файл после ошибки.");
            }
            else
            {
                Debug.LogError("Нет ни скачанного, ни локального файла. Невозможно продолжить.");
                return;
            }
        }
    }
    else
    {
        Debug.LogWarning("Интернет не найден, пробую использовать локальный файл...");

        if (File.Exists(persistentPath))
        {
            jsonContent = File.ReadAllText(persistentPath);
            Debug.Log("Файл загружен из локального хранилища.");
        }
        else
        {
            Debug.LogError("Нет интернета и нет локального файла!");
            return;
        }
    }

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
        else if (_mainGameController.GameOverState == MainGameController.GameOverStateEnum.StabilityLose)
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
            if (!string.IsNullOrEmpty(popupEvent.subEvent))
            {
                Debug.Log(popupEvent.subEvent);

                if (popupEvent.subEvent == "Snow")
                {
                    OnSnowStarted.OnNext(Unit.Default);
                }
                if (popupEvent.subEvent == "NewActionPoints")
                {
                    OnNewActionPoints.OnNext(Unit.Default);
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
            " Вы смогли вовремя отправить все материалы и дождаться помощи от правительства. " +
            "Дружеская техника проезжает по нашим дорогам и движется на передовую чтобы спасти нашу страну. ",
            "ГЛАВНОЕ МЕНЮ");
    }

    private void OnStabilityGameLoseEventShow()
    {
        _isGameOver = true;
        _popUpsController.EventPopUp.ShowEventPopUp(
            "ВАС СВЕРГЛИ",
            " В тяжелые времена встает вопрос решений. Вы не смогли сработаться с собственными согражданами и " +
            "ни утратили веру в вас. Вы повергли граждан в ужасное состояние они погибают от стресса и мысли что вы не на их стороне. " +
            "Народ принял решение, что лучшим наказанием для вас будет расстрел у берегу озера",
            "ГЛАВНОЕ МЕНЮ");
    }
    
    private void OnWarMaterialGameLoseEventShow()
    {
        _isGameOver = true;
        _popUpsController.EventPopUp.ShowEventPopUp(
            "ДРУЖЕСТВЕННЫЕ ВОЙСКА НЕ УСПЕЛИ",
            "Прошло слишком много времени. Союзные силы не могут вечно держать оборону без достаточной поддержки государства. " +
            "Сегодня было сообщено что оставшиеся силы сдались в плен и теперь проход к Звездограду открыт. " +
            "По прибытии вражеских войск город был отдан без сенного сопротивления. Третий рейх основал Рейхскомиссариат Штерненштад ",
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

    public string subEvent;

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
