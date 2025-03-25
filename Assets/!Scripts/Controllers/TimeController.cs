using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Zenject;
using UniRx;
using UnityEngine.Serialization;

public class TimeController : MonoInit
{
    [SerializeField] private Light morningLight;
    [SerializeField] private Light dayLight;
    [SerializeField] private Light eveningLight;

    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI periodText;
    [SerializeField] private Image blackoutImage;

    [SerializeField] private TextMeshProUGUI actionPointsText;
    [Range(1, 10), SerializeField] private int actionPointsMaxValue;
    [Range(1, 5), SerializeField] private int actionPointsAddValueInTheNextDay;

    [Range(1, 5), SerializeField] private int daysBetweenBombingRegularBuildings;
    [Range(1, 5), SerializeField] private int daysBetweenBombingSpecialBuildings;
    [Range(1.02f, 2f), SerializeField] private float nextTurnFadeTime;

    [SerializeField] private Button nextTurnBtn;
    [FormerlySerializedAs("_btnScripts")] [SerializeField] private MonoBehaviour[] btnScripts;

    public Button NextTurnButton => nextTurnBtn;

    private readonly DateTime _startDate = new(1942, 8, 30);

    private ReactiveProperty<DateTime> _currentDate;
    private ReactiveProperty<PeriodOfDay> _currentPeriod;
    private ReactiveProperty<int> _actionPoints;
    
    public DateTime CurrentDate => _currentDate.Value;
    public PeriodOfDay CurrentPeriod => _currentPeriod.Value;
    
    private EventPopUp _eventPopUp;

    public readonly Subject<Unit> OnNextDayEvent = new();
    public readonly Subject<Unit> OnNextTurnBtnClickBetween = new();
    public readonly Subject<Unit> OnNextTurnBtnClickEnded = new();
    public readonly Subject<Unit> OnBuildingBombed  = new();

    public enum PeriodOfDay
    {
        Утро,
        День,
        Вечер,
    }

    [Inject]
    public void Construct(ResourceViewModel resourceViewModel, EventPopUp eventPopUp)
    {
        _eventPopUp = eventPopUp;
    }

    public override void Init()
    {
        _currentDate = new ReactiveProperty<DateTime>(_startDate);
        _currentPeriod = new ReactiveProperty<PeriodOfDay>(PeriodOfDay.Утро);
        _actionPoints = new ReactiveProperty<int>(actionPointsMaxValue);

        _currentDate.Subscribe(_ => UpdateText()).AddTo(this);
        
        _currentPeriod.Subscribe(_ => UpdateText()).AddTo(this);
        _currentPeriod.Subscribe(_ => UpdateLighting()).AddTo(this);
        
        _actionPoints.Subscribe(value => 
            actionPointsText.text = $"ОД  {value} / {actionPointsMaxValue}"
        ).AddTo(this);

        nextTurnBtn.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(0.5)) // Anti-spam
            .Subscribe(_ => EndTurnButtonClicked().Forget())
            .AddTo(this);

        UpdateLighting();
        UpdateText();
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
            case PeriodOfDay.Утро:
                _currentPeriod.Value = PeriodOfDay.День;
                break;
            case PeriodOfDay.День:
                _currentPeriod.Value = PeriodOfDay.Вечер;
                break;
            case PeriodOfDay.Вечер:
                _currentPeriod.SetValueAndForceNotify(PeriodOfDay.Утро);
                _currentDate.Value = _currentDate.Value.AddDays(1);

                OnNextDayEvent.OnNext(Unit.Default);
                break;
        }
    }


    private void UpdateLighting()
    {
        morningLight.enabled = _currentPeriod.Value == PeriodOfDay.Утро;
        dayLight.enabled = _currentPeriod.Value == PeriodOfDay.День;
        eveningLight.enabled = _currentPeriod.Value == PeriodOfDay.Вечер;
    }

    private void UpdateText()
    {
        dayText.text = _currentDate.Value.ToString("d MMMM");
        periodText.text = _currentPeriod.Value.ToString();
    }

    private async UniTaskVoid EndTurnButtonClicked()
    {
        nextTurnBtn.interactable = false;
        foreach (var script in btnScripts) script.enabled = false;

        await blackoutImage.DOFade(1, nextTurnFadeTime / 2).AsyncWaitForCompletion();
        
        UpdateTime();
        OnNextTurnBtnClickBetween.OnNext(Unit.Default);
        AddActionPoints();

        await blackoutImage.DOFade(0, nextTurnFadeTime / 2).AsyncWaitForCompletion();

        nextTurnBtn.interactable = true;
        foreach (var script in btnScripts) script.enabled = true;
        if (!_eventPopUp.IsActive)
        {
            OnNextTurnBtnClickEnded.OnNext(Unit.Default);
        }
    }

    private void AddActionPoints()
    {
        _actionPoints.Value = Mathf.Clamp(
            _actionPoints.Value + actionPointsAddValueInTheNextDay, 0, actionPointsMaxValue
        );
    }
}
