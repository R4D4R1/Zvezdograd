using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class TimeController : MonoBehaviour
{
    [SerializeField] private Light morningLight;
    [SerializeField] private Light dayLight;
    [SerializeField] private Light eveningLight;

    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text periodText;

    [SerializeField] private Image blackoutImage;

    [Range(1f, 5f)]
    [SerializeField] private int _daysBetweenBombingRegularBuildings;

    [Range(1f, 5f)]
    [SerializeField] private int _daysBetweenBombingSpecialBuildings;

    [SerializeField] private Button _nextTurnBtn;
    [SerializeField] private MonoBehaviour[] _btnScripts;

    public event Action OnNextTurnBtnPressed;
    public event Action OnNextDayEvent;

    public enum PeriodOfDay
    {
        Утро,
        День,
        Вечер
    }

    private DateTime _startDate = new DateTime(1942, 8, 30);
    public DateTime CurrentDate { get; private set; }
    public PeriodOfDay CurrentPeriod { get; private set; }
    private int _daysWithoutBombing;
    private bool _startedTransition = false;


    private void Start()
    {
        _daysWithoutBombing = 0;
        CurrentDate = _startDate;
        CurrentPeriod = PeriodOfDay.Утро;
        UpdateLighting();
        UpdateText();
    }
    public void SetDateAndPeriod(DateTime newDate, PeriodOfDay newPeriod)
    {
        CurrentDate = newDate;
        CurrentPeriod = newPeriod;
        UpdateLighting();
        UpdateText();  // Update the UI after setting new values
    }

    private void UpdateTime()
    {
        switch (CurrentPeriod)
        {
            case PeriodOfDay.Утро:
                CurrentPeriod = PeriodOfDay.День;
                break;

            case PeriodOfDay.День:
                CurrentPeriod = PeriodOfDay.Вечер;
                break;

            case PeriodOfDay.Вечер:
                CurrentPeriod = PeriodOfDay.Утро;

                CurrentDate = CurrentDate.AddDays(1);

                _daysWithoutBombing++;
                if (_daysWithoutBombing == _daysBetweenBombingRegularBuildings)
                {
                    ControllersManager.Instance.buildingController.BombRegularBuilding();
                    _daysWithoutBombing = 0;
                    OnNextDayEvent.Invoke();
                }

                break;
        }
        UpdateLighting();
        UpdateText();
    }

    private void UpdateLighting()
    {
        morningLight.enabled = (CurrentPeriod == PeriodOfDay.Утро);
        dayLight.enabled = (CurrentPeriod == PeriodOfDay.День);
        eveningLight.enabled = (CurrentPeriod == PeriodOfDay.Вечер);
    }

    private void UpdateText()
    {
        dayText.text = CurrentDate.ToString("d MMMM");
        periodText.text = CurrentPeriod.ToString();
    }

    public void EndTurnButtonClicked()
    {
        ControllersManager.Instance.selectionController.enabled = false;
        _nextTurnBtn.interactable = false;

        foreach (var script in _btnScripts)
        {
            script.enabled = false;
        }


        blackoutImage.DOFade(1, 0.5f).OnComplete(async() =>
        {
            UpdateTime();
            await UniTask.Delay(100);
            OnNextTurnBtnPressed.Invoke();

            blackoutImage.DOFade(0, 0.5f).OnComplete(() =>
            {
                // Activate Next Turn Btn
                _nextTurnBtn.interactable = true;
                _startedTransition = false;
                foreach (var script in _btnScripts)
                {
                    script.enabled = true;
                }

                if(!EventPopUp.Instance.IsActive)
                    ControllersManager.Instance.selectionController.enabled = true;
            });
        });
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (_startedTransition == false)
            {
                Bootstrapper.Instance?.SoundController?.PlayButtonPressSound();

                _startedTransition = true;
                EndTurnButtonClicked();
            }
        }
    }
}
