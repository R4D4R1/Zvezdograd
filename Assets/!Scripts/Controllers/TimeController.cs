using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Threading.Tasks;
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

    [Range(1,3),SerializeField] private int increaseMaxAPValue;
    [Range(1,3),SerializeField] private int increaseAddAPValue;

    private int _localIncreaseMaxAPValue = 0;
    private int _localIncreaseAddAPValue = 0;
    
    public Button NextTurnButton => nextTurnBtn;

    private readonly DateTime _startDate = new(1942, 8, 30);

    private ReactiveProperty<DateTime> _currentDate;
    private ReactiveProperty<PeriodOfDay> _currentPeriod;
    private ReactiveProperty<int> _actionPoints;
    
    public DateTime CurrentDate => _currentDate.Value;
    public PeriodOfDay CurrentPeriod => _currentPeriod.Value;
    
    private PopUpsController _popUpsController;
    private TimeControllerConfig _timeControllerConfig;
    private TutorialController _tutorialController;
    private BuildingController _buildingController;

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
    
    [Inject]
    public void Construct(PopUpsController popUpsController, TimeControllerConfig timeControllerConfig,
        TutorialController tutorialController, BuildingController buildingController)
    {
        _popUpsController = popUpsController;
        _timeControllerConfig = timeControllerConfig;
        _tutorialController = tutorialController;
        _buildingController = buildingController;
    }

    public override void Init()
    {
        base.Init();
        _currentDate = new ReactiveProperty<DateTime>(_startDate);
        _currentPeriod = new ReactiveProperty<PeriodOfDay>(PeriodOfDay.Morning);
        _actionPoints = new ReactiveProperty<int>(_timeControllerConfig.ActionPointsMaxValue);

        _currentDate.Subscribe(_ => UpdateText()).AddTo(this);
        _currentPeriod.Subscribe(_ => UpdateText()).AddTo(this);
        _currentPeriod.Subscribe(_ => UpdateLighting()).AddTo(this);

        _actionPoints.Subscribe(value =>
            actionPointsText.text =
                $"ОД  {value} / {_timeControllerConfig.ActionPointsMaxValue + _localIncreaseMaxAPValue}"
        ).AddTo(this);

        nextTurnBtn.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(0.5))
            .Subscribe(_ => EndTurnButtonClicked().Forget())
            .AddTo(this);

        _popUpsController.EventPopUp.OnEventPopUpHide
            .Subscribe(_ => EnableNextTurnLogic())
            .AddTo(this);

        _buildingController.GetCityHallBuilding().OnNewActionPointsCreated
            .Subscribe(_ => NewActionPointsCreated())
            .AddTo(this);
        
        _tutorialController.OnTutorialStarted
            .Subscribe(_ => DisableNextTurnLogic())
            .AddTo(this);
        
        UpdateLighting();
        UpdateText();
    }

    private void OnEnable()
    {
        _tutorialController.OnTutorialEnd.AddListener(EnableNextTurnLogic);
    }

    private void OnDisable()
    {
        _tutorialController.OnTutorialEnd.RemoveAllListeners();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && nextTurnBtn.interactable)
        {
            EndTurnButtonClicked().Forget();
        }
    }

    public void EnableNextTurnLogic()
    {
        nextTurnBtn.interactable = true;
    }
    
    public void DisableNextTurnLogic()
    {
        nextTurnBtn.interactable = false;
    }

    private void NewActionPointsCreated()
    {
        _localIncreaseMaxAPValue = increaseMaxAPValue;
        _localIncreaseAddAPValue = increaseAddAPValue;
    }
    
    public bool OnActionPointUsed()
    {
        if (_actionPoints.Value > 0)
        {
            _actionPoints.Value--;
            return true;
        }
        return false;
    }

    private void UpdateTime()
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
                break;
        }
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
        periodText.text = _currentPeriod.Value.ToString();
    }

    private async UniTaskVoid EndTurnButtonClicked()
    {
        OnNextTurnBtnClickStarted.OnNext(Unit.Default);
        nextTurnBtn.interactable = false;
        foreach (var script in btnScripts) script.enabled = false;

        await blackoutImage.DOFade(1, _timeControllerConfig.NextTurnFadeTime / 2).AsyncWaitForCompletion();
        
        UpdateTime();
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
        _actionPoints.Value = Mathf.Clamp(
            _actionPoints.Value + _timeControllerConfig.ActionPointsAddValueInTheNextDay+_localIncreaseAddAPValue,
            0, _timeControllerConfig.ActionPointsMaxValue + _localIncreaseAddAPValue
        );
    }
}
