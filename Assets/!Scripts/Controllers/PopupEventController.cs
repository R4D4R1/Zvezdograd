using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class PopupEventController : MonoBehaviour
{
    [SerializeField] private TextAsset _specificEventsJson;

    [Range(0,100)]
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
            EventPopUp.Instance.ShowEventPopUp("ВАС СВЕРГЛИ", "Вы не смогли удержать наш город в стабильности и мире из-за чего и были растреляны незамедлитедьно", "КОНЕЦ");
        }
        else
        {
            if (specificEvents.TryGetValue(eventKey, out PopupEvent popupEvent))
            {
                EventPopUp.Instance.ShowEventPopUp(popupEvent.title, popupEvent.mainText, popupEvent.buttonText);
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
}

[Serializable]
public class PopupEventData
{
    public List<PopupEvent> events;
}
