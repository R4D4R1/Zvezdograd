using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;

public class PopupEventController : MonoBehaviour
{
    [SerializeField] private TextAsset _specificEventsJson;
    [SerializeField] private UnityEvent _onSnowStarted;

    private bool _isGameOver = false;

    private Dictionary<string, PopupEvent> _specificEvents;

    private void OnEnable()
    {
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += OnPeriodChanged;
    }

    private void OnDisable()
    {
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

    private async void OnPeriodChanged()
    {
        // Delay to ckeck WIN/LOSE condition
        await UniTask.Delay(1);

        Debug.Log("New Period");
        var controllerManager = ControllersManager.Instance;

        if(controllerManager.mainGameController.GameOverState == MainGameController.GameOverStateEnum.Win)
        {
            OnGameWonEventShow();
        }
        else if(controllerManager.mainGameController.GameOverState == MainGameController.GameOverStateEnum.Lose)
        {
            OnGameLoseEventShow();
        }

        if (_isGameOver)
        {
            return;
        }

        DateTime currentDate = ControllersManager.Instance.timeController.CurrentDate;
        string currentPeriod = ControllersManager.Instance.timeController.CurrentPeriod.ToString();

        string eventKey = currentDate.ToString("yyyy-MM-dd") + currentPeriod;

        if (_specificEvents.TryGetValue(eventKey, out PopupEvent popupEvent))
        {
            if (!string.IsNullOrEmpty(popupEvent.weatherType))
            {
                if (popupEvent.weatherType == "Снег")
                {
                    _onSnowStarted?.Invoke();
                }
            }
            if (!string.IsNullOrEmpty(popupEvent.buildingType))
            {
                // Проверяем, является ли тип здания одним из допустимых
                if (popupEvent.buildingType == "Еда" || popupEvent.buildingType == "Медикаменты" || popupEvent.buildingType == "Совет")
                {
                    if (popupEvent.buildingType == "Еда")
                    {
                        ControllersManager.Instance.popUpsController.FoodTrucksPopUp.EnableQuest(QuestPopUp.QuestType.Provision, popupEvent.questText, popupEvent.unitSize, popupEvent.deadlineInDays,
                            popupEvent.turnsToWork, popupEvent.turnsToRest, popupEvent.materialsToGet, popupEvent.materialsToLose,
                            popupEvent.stabilityToGet, popupEvent.stabilityToLose, popupEvent.relationshipWithGovToGet, popupEvent.relationshipWithGovToLose);
                    }
                    else if (popupEvent.buildingType == "Медикаменты")
                    {
                        ControllersManager.Instance.popUpsController.HospitalPopUp.EnableQuest(QuestPopUp.QuestType.Medicine, popupEvent.questText, popupEvent.unitSize, popupEvent.deadlineInDays,
                            popupEvent.turnsToWork, popupEvent.turnsToRest, popupEvent.materialsToGet, popupEvent.materialsToLose,
                            popupEvent.stabilityToGet, popupEvent.stabilityToLose, popupEvent.relationshipWithGovToGet, popupEvent.relationshipWithGovToLose);
                    }
                    else if (popupEvent.buildingType == "Совет")
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
            "ПОБЕДА",
            "После долгих и бессонных ночей мы все же дождались прибытия помощи, но какой ценой...",
            "МЫ СПАСЕНЫ");
    }

    private void OnGameLoseEventShow()
    {
        _isGameOver = true;
        EventPopUp.Instance.ShowEventPopUp(
            "ВАС СВЕРГЛИ",
            "Вы не смогли удержать наш город в стабильности и мире, из-за чего и были расстреляны незамедлительно",
            "КОНЕЦ");

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
