using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class PopupEventController : MonoBehaviour
{
    [SerializeField] private TextAsset _specificEventsJson;
    [SerializeField] private TextAsset _randomEventsJson;

    [Range(0,100)]
    [SerializeField] private int _randomEventChance;

    private Dictionary<string, PopupEvent> specificEvents;
    private List<PopupEvent> randomEvents;

    private DateTime randomEventStartDate = new DateTime(1941, 9, 1);

    private HashSet<DateTime> usedRandomEventDays;

    private void Start()
    {
        LoadEvents();
        usedRandomEventDays = new HashSet<DateTime>();
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += OnDateChanged;
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

        randomEvents = new List<PopupEvent>();
        var randomEventsData = JsonConvert.DeserializeObject<PopupEventData>(_randomEventsJson.text);
        randomEvents.AddRange(randomEventsData.events);
    }

    private void OnDateChanged()
    {
        DateTime currentDate = ControllersManager.Instance.timeController.CurrentDate;
        string currentPeriod = ControllersManager.Instance.timeController.CurrentPeriod.ToString();

        string eventKey = currentDate.ToString("yyyy-MM-dd") + currentPeriod;

        if (specificEvents.TryGetValue(eventKey, out PopupEvent popupEvent))
        {
            ShowPopup(popupEvent.title, popupEvent.mainText, popupEvent.buttonText);
        }
        else
        {
            TryShowRandomEvent(currentDate, currentPeriod);
        }
    }

    private void TryShowRandomEvent(DateTime currentDate, string currentPeriod)
    {
        if (currentDate >= randomEventStartDate && !usedRandomEventDays.Contains(currentDate))
        {

            if (UnityEngine.Random.Range(0, 100) < _randomEventChance)
            {

                int randomIndex = UnityEngine.Random.Range(0, randomEvents.Count);
                var randomEvent = randomEvents[randomIndex];

                ShowPopup(randomEvent.title, randomEvent.mainText, randomEvent.buttonText);
                usedRandomEventDays.Add(currentDate);

            }
            else
            {

            }
        }
    }

    private void ShowPopup(string title, string mainText, string buttonText)
    {
        EventPopUp.Instance.ShowEventPopUp(title, mainText, buttonText);
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
