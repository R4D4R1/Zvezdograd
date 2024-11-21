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

    [SerializeField] private Image blackoutImage; // ������ ����������� ��� ���������� ������

    [Range(1f, 5f)]
    [SerializeField] private int _daysBetweenBombingRegularBuildings;

    [Range(1f, 5f)]
    [SerializeField] private int _daysBetweenBombingSpecialBuildings;

    private enum PeriodOfDay
    {
        ����,
        ����,
        �����
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
        _currentPeriod = PeriodOfDay.����;
        UpdateLighting();
        UpdateText();
    }

    private void UpdateTime()
    {
        switch (_currentPeriod)
        {
            case PeriodOfDay.����:
                _currentPeriod = PeriodOfDay.����;
                break;

            case PeriodOfDay.����:
                _currentPeriod = PeriodOfDay.�����;
                break;

            case PeriodOfDay.�����:
                _currentPeriod = PeriodOfDay.����;

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
        morningLight.enabled = (_currentPeriod == PeriodOfDay.����);
        dayLight.enabled = (_currentPeriod == PeriodOfDay.����);
        eveningLight.enabled = (_currentPeriod == PeriodOfDay.�����);
    }

    private void UpdateText()
    {
        dayText.text = CurrentDate.ToString("d MMMM");
        periodText.text = _currentPeriod.ToString();
    }

    public void OnEndTurnButtonClicked()
    {
        // ��������� ����� ����� ������ ������� ���
        blackoutImage.DOFade(1, 1.0f).OnComplete(() =>
        {
            UpdateTime(); // ������ ������ ��� ����� ����������
            blackoutImage.DOFade(0, 1.0f); // ������� ����������
        });
    }
}
