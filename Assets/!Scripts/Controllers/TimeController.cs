using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Zenject;

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
        ����,
        ����,
        �����
    }

    private DateTime _startDate = new DateTime(1942, 8, 30);
    public DateTime CurrentDate { get; private set; }
    public PeriodOfDay CurrentPeriod { get; private set; }
    private int _daysWithoutBombing;
    private bool _startedTransition = false;

    protected ControllersManager _controllersManager;
    protected ResourceViewModel _resourceViewModel;

    [Inject]
    public void Construct(ControllersManager controllersManager, ResourceViewModel resourceViewModel)
    {
        _controllersManager = controllersManager;
        _resourceViewModel = resourceViewModel;
    }
    private void Start()
    {
        _daysWithoutBombing = 0;
        CurrentDate = _startDate;
        CurrentPeriod = PeriodOfDay.����;
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
                    _controllersManager.BuildingController.BombRegularBuilding();
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
        _controllersManager.SelectionController.enabled = false;
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

                Debug.Log(EventPopUp.Instance.IsActive);

                if(!EventPopUp.Instance.IsActive)
                    _controllersManager.SelectionController.enabled = true;
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
