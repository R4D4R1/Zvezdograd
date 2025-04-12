using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Zenject;
using UniRx;
using UnityEngine.Serialization;

public class TimeController : MonoInit
{
    [SerializeField] private Light morningLight;
    [SerializeField] private Light noonLight;
    [SerializeField] private Light eveningLight;

    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI periodText;
    [SerializeField] private Image blackoutImage;
    [SerializeField] private TextMeshProUGUI actionPointsText;

    [SerializeField] private Button nextTurnBtn;
    [FormerlySerializedAs("_btnScripts")] [SerializeField] private MonoBehaviour[] btnScripts;

    [Range(1, 3), SerializeField] private int increaseMaxAPValue;
    [Range(1, 3), SerializeField] private int increaseAddAPValue;

    [FormerlySerializedAs("_localIncreaseMaxAPValue")] public int LocalIncreaseMaxAPValue;
    [FormerlySerializedAs("_localIncreaseAddAPValue")] public int LocalIncreaseAddAPValue;

    private readonly DateTime _startDate = new(1942, 10, 30);
    public List<DelayedAction> DelayedActions = new();

    private ReactiveProperty<DateTime> _currentDate;
    private ReactiveProperty<PeriodOfDay> _currentPeriod;
    private ReactiveProperty<int> _currentActionPoints;

    private PopUpsController _popUpsController;
    private TimeControllerConfig _timeControllerConfig;
    private TutorialController _tutorialController;
    private BuildingsController _buildingsController;

    public Button NextTurnButton => nextTurnBtn;
    public DateTime CurrentDate => _currentDate.Value;
    public PeriodOfDay CurrentPeriod => _currentPeriod.Value;
    public int CurrentActionPoints => _currentActionPoints.Value;
    
    public readonly Subject<Unit> OnNextDayEvent = new();
    public readonly Subject<Unit> OnNextTurnBtnClickStarted = new();
    public readonly Subject<Unit> OnNextTurnBtnClickBetween = new();
    public readonly Subject<Unit> OnNextTurnBtnClickEnded = new();

    public enum PeriodOfDay
    {
        Morning,
        Noon,
        Evening,
    }

    public class DelayedAction
    {
        public int DaysRemaining;
        public readonly Action Action;

        public DelayedAction(int days, Action action)
        {
            DaysRemaining = days;
            Action = action;
        }
    }

    [Inject]
    public void Construct(PopUpsController popUpsController, TimeControllerConfig timeControllerConfig,
        TutorialController tutorialController, BuildingsController buildingsController)
    {
        _popUpsController = popUpsController;
        _timeControllerConfig = timeControllerConfig;
        _tutorialController = tutorialController;
        _buildingsController = buildingsController;
    }

    public override UniTask Init()
    {
        base.Init();

        _currentDate = new ReactiveProperty<DateTime>(_startDate);
        _currentPeriod = new ReactiveProperty<PeriodOfDay>(PeriodOfDay.Morning);
        _currentActionPoints = new ReactiveProperty<int>(_timeControllerConfig.ActionPointsMaxValue);

        _currentDate.Subscribe(_ => UpdateTime()).AddTo(this);
        _currentPeriod.Subscribe(_ => UpdateTime()).AddTo(this);

        _currentActionPoints.Subscribe(value =>
            actionPointsText.text =
                $"ОД  {value} / {_timeControllerConfig.ActionPointsMaxValue + LocalIncreaseMaxAPValue}"
        ).AddTo(this);

        nextTurnBtn.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(0.5))
            .Subscribe(_ => EndTurnButtonClicked().Forget())
            .AddTo(this);

        _popUpsController.EventPopUp.OnEventPopUpHide
            .Subscribe(_ => EnableNextTurnLogic())
            .AddTo(this);

        _buildingsController.GetCityHallBuilding().OnNewActionPointsCreated
            .Subscribe(_ => NewActionPointsCreated())
            .AddTo(this);

        _tutorialController.OnTutorialStarted
            .Subscribe(_ => DisableNextTurnLogic())
            .AddTo(this);

        LocalIncreaseMaxAPValue = 0;
        LocalIncreaseAddAPValue = 0;

        UpdateTime();
        return UniTask.CompletedTask;
    }

    private void OnEnable() => _tutorialController.OnTutorialEnd.AddListener(EnableNextTurnLogic);

    private void OnDisable() => _tutorialController.OnTutorialEnd.RemoveAllListeners();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && nextTurnBtn.interactable)
        {
            EndTurnButtonClicked().Forget();
        }
    }

    public void EnableNextTurnLogic() => nextTurnBtn.interactable = true;
    
    public void DisableNextTurnLogic() => nextTurnBtn.interactable = false;

    private void NewActionPointsCreated()
    {
        LocalIncreaseMaxAPValue += increaseMaxAPValue;
        LocalIncreaseAddAPValue += increaseAddAPValue;
    }

    public bool OnActionPointUsed()
    {
        if (_currentActionPoints.Value > 0)
        {
            _currentActionPoints.Value--;
            return true;
        }
        return false;
    }

    private void ChangePeriodToNext()
    {
        switch (_currentPeriod.Value)
        {
            case PeriodOfDay.Morning:
                _currentPeriod.Value = PeriodOfDay.Noon;
                break;
            case PeriodOfDay.Noon:
                _currentPeriod.Value = PeriodOfDay.Evening;
                break;
            case PeriodOfDay.Evening:
                _currentPeriod.SetValueAndForceNotify(PeriodOfDay.Morning);
                _currentDate.Value = _currentDate.Value.AddDays(1);
                OnNextDayEvent.OnNext(Unit.Default);
                ProcessDelayedActions();
                break;
        }
    }

    private void UpdateTime()
    {
        UpdateLighting();
        UpdateText();
    }

    private void UpdateLighting()
    {
        morningLight.enabled = _currentPeriod.Value == PeriodOfDay.Morning;
        noonLight.enabled = _currentPeriod.Value == PeriodOfDay.Noon;
        eveningLight.enabled = _currentPeriod.Value == PeriodOfDay.Evening;
    }

    private void UpdateText()
    {
        dayText.text = _currentDate.Value.ToString("d MMMM");

        periodText.text = _currentPeriod.Value switch
        {
            PeriodOfDay.Morning => "Утро",
            PeriodOfDay.Noon => "Полдень",
            PeriodOfDay.Evening => "Вечер",
            _ => periodText.text
        };
    }

    private async UniTaskVoid EndTurnButtonClicked()
    {
        OnNextTurnBtnClickStarted.OnNext(Unit.Default);
        nextTurnBtn.interactable = false;

        foreach (var script in btnScripts) script.enabled = false;

        await blackoutImage.DOFade(1, _timeControllerConfig.NextTurnFadeTime / 2).AsyncWaitForCompletion();
        
        ChangePeriodToNext();
        OnNextTurnBtnClickBetween.OnNext(Unit.Default);
        AddActionPoints();

        await blackoutImage.DOFade(0, _timeControllerConfig.NextTurnFadeTime / 2).AsyncWaitForCompletion();

        foreach (var script in btnScripts) script.enabled = true;

        if (!_popUpsController.EventPopUp.IsActive)
        {
            EnableNextTurnLogic();
            OnNextTurnBtnClickEnded.OnNext(Unit.Default);
        }
    }

    private void AddActionPoints()
    {
        _currentActionPoints.Value = Mathf.Clamp(
            _currentActionPoints.Value + _timeControllerConfig.ActionPointsAddValueInTheNextDay + LocalIncreaseAddAPValue,
            0, _timeControllerConfig.ActionPointsMaxValue + LocalIncreaseAddAPValue
        );
    }

    private void ProcessDelayedActions()
    {
        for (int i = DelayedActions.Count - 1; i >= 0; i--)
        {
            DelayedActions[i].DaysRemaining--;

            if (DelayedActions[i].DaysRemaining <= 0)
            {
                DelayedActions[i].Action?.Invoke();
                DelayedActions.RemoveAt(i);
            }
        }
    }

    // Method to wait in ingame days and then execute an action
    public void WaitDaysAndExecute(int days, Action action)
    {
        if (days <= 0)
        {
            Debug.LogError($"DAYS CANNOT BE LESS THAN 1!");
            return;
        }

        DelayedActions.Add(new DelayedAction(days, action));
    }
    
    public void SetDateAndPeriod(DateTime newDate, PeriodOfDay newPeriod)
    {
        _currentDate.Value = newDate;
        _currentPeriod.Value = newPeriod;
    }
    
    public void SetActionPoints(int value)
    {
        _currentActionPoints.Value = value;
    }
}
