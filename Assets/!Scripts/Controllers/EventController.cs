using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;
using Zenject;
using System.IO;
using DG.Tweening;
using UniRx;

public class EventController : MonoInit
{
    [Header("Camera Animation Points")]
    [SerializeField] private Transform cameraPointA;
    [SerializeField] private Transform cameraPointB;
    [SerializeField,Range(1,5)] private float gameOverAnimationDuration;
    [SerializeField] private bool useLocalFileIfAvailable = true;

    private bool _isGameOver;
    private Dictionary<string, PopupEvent> _specificEvents;

    private PopUpsController _popUpsController;
    private TimeController _timeController;
    private MainGameController _mainGameController;
    private Camera _camera;

    [HideInInspector]
    public bool IsSnowing = false;

    public readonly Subject<Unit> OnSnowStarted = new();
    public readonly Subject<Unit> OnNewActionPointsAdded = new();
    public readonly Subject<Unit> OnProvisionLost = new();
    public readonly Subject<PopupEvent> OnQuestTriggered = new();
    public readonly Subject<Unit> OnGameOverStarted = new();

    private const string FileName = "specificEvents.json";
    private const string EventFileUrl = "https://drive.google.com/uc?export=download&id=1m2oEvzu-1W5dA53_i66Zm7CjzfS1PHDz";

    [Inject]
    public void Construct(PopUpsController popUpsController, TimeController timeController,
        MainGameController mainGameController,Camera mainCamera)
    {
        _popUpsController = popUpsController;
        _timeController = timeController;
        _mainGameController = mainGameController;
        _camera = mainCamera;
    }

    public override UniTask Init()
    {
        base.Init();
        _timeController.OnNextTurnBtnClickBetween
            .Subscribe(_ => OnPeriodChanged())
            .AddTo(this);

        LoadEvents().Forget();

        OnPeriodChanged();

        return UniTask.CompletedTask;
    }

    private async UniTaskVoid LoadEvents()
    {
        _specificEvents = new Dictionary<string, PopupEvent>();
        var persistentPath = Path.Combine(Application.persistentDataPath, FileName);
        string jsonContent;

        bool hasInternet = Application.internetReachability != NetworkReachability.NotReachable;

        // Check if we should prefer local file even if there's internet
        if (useLocalFileIfAvailable || !hasInternet)
        {
            Debug.LogWarning("Используем локальный файл...");
            jsonContent = TryLoadLocalFile(persistentPath);
            if (jsonContent == null) return;
        }
        else
        {
            using var www = UnityEngine.Networking.UnityWebRequest.Get(EventFileUrl);
            await www.SendWebRequest();

            if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                jsonContent = www.downloadHandler.text;
                File.WriteAllText(persistentPath, jsonContent);
            }
            else
            {
                Debug.LogWarning($"Ошибка при скачивании: {www.error}");
                jsonContent = TryLoadLocalFile(persistentPath);
                if (jsonContent == null) return;
            }
        }

        var specificEventsData = JsonConvert.DeserializeObject<PopupEventData>(jsonContent);
        foreach (var e in specificEventsData.events)
        {
            var key = e.date + e.period;
            _specificEvents[key] = e;
        }
    }

    private string TryLoadLocalFile(string path)
    {
        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }

        Debug.LogError("Нет интернета и нет локального файла!");
        return null;
    }

    private async void OnPeriodChanged()
    {
        await UniTask.Delay(1);

        switch (_mainGameController.GameOverState)
        {
            case MainGameController.GameOverStateEnum.WinBySendingArmyMaterials:
                StartGameOver("ПОБЕДА",
                    "Вы смогли вовремя отправить все материалы и дождаться помощи от правительства. " +
                    "Дружеская техника проезжает по нашим дорогам и движется на передовую чтобы спасти нашу страну.");
                return;

            case MainGameController.GameOverStateEnum.StabilityLose:
                StartGameOver("ВАС СВЕРГЛИ",
                    "Вы не смогли сработаться с собственными согражданами и ни утратили веру в вас. " +
                    "Народ принял решение, что лучшим наказанием для вас будет расстрел у берегу озера");
                return;
            
            case MainGameController.GameOverStateEnum.NoTimeLeftLose:
                StartGameOver("ДРУЖЕСТВЕННЫЕ ВОЙСКА НЕ УСПЕЛИ",
                    "Прошло слишком много времени. Союзные силы не могут вечно держать оборону без достаточной поддержки государства. " +
                    "Сегодня было сообщено что оставшиеся силы сдались в плен и теперь проход к Звездограду открыт. " +
                    "По прибытии вражеских войск город был отдан без сенного сопротивления. " +
                    "Третий рейх основал Рейхскомиссариат Штерненштад.");
                return;
        }

        if (_isGameOver) return;

        var eventKey = _timeController.CurrentDate.ToString("yyyy-MM-dd") + _timeController.CurrentPeriod;
        if (_specificEvents.TryGetValue(eventKey, out var popupEvent))
        {
            TriggerSubEvents(popupEvent);
            if (!string.IsNullOrEmpty(popupEvent.buildingType))
            {
                OnQuestTriggered.OnNext(popupEvent);
            }

            _popUpsController.EventPopUp.ShowEventPopUp(popupEvent.title, popupEvent.mainText, popupEvent.buttonText);
        }
    }

    private void TriggerSubEvents(PopupEvent popupEvent)
    {
        if (string.IsNullOrEmpty(popupEvent.subEvent)) return;

        switch (popupEvent.subEvent)
        {
            case "Snow":
                IsSnowing = true;
                OnSnowStarted.OnNext(Unit.Default);
                break;
            case "NewActionPoints":
                OnNewActionPointsAdded.OnNext(Unit.Default);
                break;
            case "FoodGotLost":
                OnProvisionLost.OnNext(Unit.Default);
                break;
        }
    }

    private async void StartGameOver(string title, string mainText)
    {
        OnGameOverStarted.OnNext(Unit.Default);
        
        _isGameOver = true;
        _camera.transform.position = cameraPointA.position;
        _camera.transform.rotation = cameraPointA.rotation;
        
        await _camera.transform.DOMove(cameraPointB.position, gameOverAnimationDuration).
            SetEase(Ease.InOutSine).AsyncWaitForCompletion();
        
        _popUpsController.EventPopUp.ShowEventPopUp(title, mainText, "ГЛАВНОЕ МЕНЮ");
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
