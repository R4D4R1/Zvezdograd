using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

public class TimeController : MonoBehaviour
{
    public static TimeController Instance;

    [SerializeField] private Light morningLight;
    [SerializeField] private Light dayLight;
    [SerializeField] private Light eveningLight;

    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text periodText;

    [SerializeField] private Image blackoutImage; // Черное изображение для затемнения экрана

    [Range(1f, 5f)]
    [SerializeField] private int _daysBetweenBombingRegularBuildings;

    [Range(1f, 5f)]
    [SerializeField] private int _daysBetweenBombingSpecialBuildings;

    private enum PeriodOfDay
    {
        Утро,
        День,
        Вечер
    }

    private DateTime _startDate = new DateTime(1941, 8, 30);
    private DateTime _endDate = new DateTime(1941, 9, 30);
    public DateTime CurrentDate { get; private set; }
    private PeriodOfDay _currentPeriod;
    private int _daysWithoutBombing;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        _daysWithoutBombing = 0;
        CurrentDate = _startDate;
        _currentPeriod = PeriodOfDay.Утро;
        UpdateLighting();
        UpdateText();
    }

    private void UpdateTime()
    {
        switch (_currentPeriod)
        {
            case PeriodOfDay.Утро:
                _currentPeriod = PeriodOfDay.День;
                break;

            case PeriodOfDay.День:
                _currentPeriod = PeriodOfDay.Вечер;
                break;

            case PeriodOfDay.Вечер:
                _currentPeriod = PeriodOfDay.Утро;

                CurrentDate = CurrentDate.AddDays(1);

                _daysWithoutBombing++;
                if (_daysWithoutBombing == _daysBetweenBombingRegularBuildings)
                {
                    BuildingBombingController.Instance.BombRegularBuilding();
                    _daysWithoutBombing = 0;
                }

                break;
        }
        UpdateLighting();
        UpdateText();
    }

    private void UpdateLighting()
    {
        morningLight.enabled = (_currentPeriod == PeriodOfDay.Утро);
        dayLight.enabled = (_currentPeriod == PeriodOfDay.День);
        eveningLight.enabled = (_currentPeriod == PeriodOfDay.Вечер);
    }

    private void UpdateText()
    {
        dayText.text = CurrentDate.ToString("d MMMM");
        periodText.text = _currentPeriod.ToString();
    }

    public void OnEndTurnButtonClicked()
    {
        // Затемнить экран перед сменой периода дня
        blackoutImage.DOFade(1, 1.0f).OnComplete(() =>
        {
            UpdateTime(); // Меняем период дня после затемнения
            blackoutImage.DOFade(0, 1.0f); // Убираем затемнение
        });
    }
}
