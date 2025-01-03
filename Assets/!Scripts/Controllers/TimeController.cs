using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

public class TimeController : MonoBehaviour
{
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

    [SerializeField] private Button _nextTurnBtn;
    [SerializeField] private MonoBehaviour[] _btnScripts;

    public event Action OnNextTurnBtnPressed;

    public enum PeriodOfDay
    {
        ����,
        ����,
        �����
    }

    private DateTime _startDate = new DateTime(1941, 8, 30);
    private DateTime _endDate = new DateTime(1941, 9, 30);
    public DateTime CurrentDate { get; private set; }
    public PeriodOfDay CurrentPeriod { get; private set; }
    private int _daysWithoutBombing;


    private void Start()
    {
        _daysWithoutBombing = 0;
        CurrentDate = _startDate;
        CurrentPeriod = PeriodOfDay.����;
        UpdateLighting();
        UpdateText();
    }

    private void UpdateTime()
    {
        switch (CurrentPeriod)
        {
            case PeriodOfDay.����:
                CurrentPeriod = PeriodOfDay.����;
                break;

            case PeriodOfDay.����:
                CurrentPeriod = PeriodOfDay.�����;
                break;

            case PeriodOfDay.�����:
                CurrentPeriod = PeriodOfDay.����;

                CurrentDate = CurrentDate.AddDays(1);

                _daysWithoutBombing++;
                if (_daysWithoutBombing == _daysBetweenBombingRegularBuildings)
                {
                    ControllersManager.Instance.buildingBombingController.BombRegularBuilding();
                    _daysWithoutBombing = 0;
                }

                break;
        }
        UpdateLighting();
        UpdateText();
    }

    private void UpdateLighting()
    {
        morningLight.enabled = (CurrentPeriod == PeriodOfDay.����);
        dayLight.enabled = (CurrentPeriod == PeriodOfDay.����);
        eveningLight.enabled = (CurrentPeriod == PeriodOfDay.�����);
    }

    private void UpdateText()
    {
        dayText.text = CurrentDate.ToString("d MMMM");
        periodText.text = CurrentPeriod.ToString();
    }

    public void EndTurnButtonClicked()
    {
        _nextTurnBtn.interactable = false;

        foreach (var script in _btnScripts)
        {
            script.enabled = false;
        }


        blackoutImage.DOFade(1, 1.0f).OnComplete(() =>
        {
            UpdateTime();
            blackoutImage.DOFade(0, 1.0f).OnComplete(() =>
            {
                // Activate Next Turn Btn
                _nextTurnBtn.interactable = true;

                foreach (var script in _btnScripts)
                {
                    script.enabled = true;
                }

                OnNextTurnBtnPressed.Invoke();
            });
        });
    }
}
