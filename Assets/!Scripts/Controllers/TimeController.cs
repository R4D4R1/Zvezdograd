using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Zenject;
using UniRx;
using UnityEngine.Serialization;

public class TimeController : MonoBehaviour
{
    [SerializeField] private Light morningLight;
    [SerializeField] private Light dayLight;
    [SerializeField] private Light eveningLight;

    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI periodText;
    [SerializeField] private Image blackoutImage;

    [FormerlySerializedAs("_actionPointsText")] [SerializeField] private TextMeshProUGUI actionPointsText;
    [FormerlySerializedAs("_actionPointsValue")] [Range(1f, 10f), SerializeField] private int actionPointsValue;
    [FormerlySerializedAs("_actionPointsMaxValue")] [Range(1f, 10f), SerializeField] private int actionPointsMaxValue;
    [FormerlySerializedAs("_actionPointsAddValueInTheNextDay")] [Range(1f, 5f), SerializeField] private int actionPointsAddValueInTheNextDay;

    [FormerlySerializedAs("_daysBetweenBombingRegularBuildings")] [Range(1f, 5f), SerializeField] private int daysBetweenBombingRegularBuildings;
    [FormerlySerializedAs("_daysBetweenBombingSpecialBuildings")] [Range(1f, 5f), SerializeField] private int daysBetweenBombingSpecialBuildings;
    [FormerlySerializedAs("_nextTurnFadeTime")] [Range(1.02f, 2f), SerializeField] private float nextTurnFadeTime;

    [FormerlySerializedAs("_nextTurnBtn")] [SerializeField] private Button nextTurnBtn;
    [FormerlySerializedAs("_btnScripts")] [SerializeField] private MonoBehaviour[] btnScripts;

    public Button NextTurnButton => nextTurnBtn;

    private readonly DateTime _startDate = new DateTime(1942, 8, 30);
    private int _daysWithoutBombing;

    private ReactiveProperty<DateTime> _currentDate;
    private ReactiveProperty<PeriodOfDay> _currentPeriod;
    public DateTime CurrentDate => _currentDate.Value;
    public PeriodOfDay CurrentPeriod => _currentPeriod.Value;


    private ControllersManager _controllersManager;
    private ResourceViewModel _resourceViewModel;
    private EventPopUp _eventPopUp;

    public readonly Subject<Unit> OnNextDayEvent = new();
    public readonly Subject<Unit> OnNextTurnBtnClickBetween = new();
    public readonly Subject<Unit> OnNextTurnBtnClickEnded = new();

    public enum PeriodOfDay
    {
        Утро,
        День,
        Вечер,
    }

    [Inject]
    public void Construct(ControllersManager controllersManager, ResourceViewModel resourceViewModel,EventPopUp eventPopUp)
    {
        _controllersManager = controllersManager;
        _resourceViewModel = resourceViewModel;
        _eventPopUp = eventPopUp;
    }

    public void Init()
    {
        _daysWithoutBombing = 0;
        _currentDate = new ReactiveProperty<DateTime>(_startDate);
        _currentPeriod = new ReactiveProperty<PeriodOfDay>(PeriodOfDay.Утро);

        _currentDate.Subscribe(_ => UpdateText()).AddTo(this);
        _currentPeriod.Subscribe(_ => UpdateLighting()).AddTo(this);

        nextTurnBtn.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(0.5)) //anti spam
            .Subscribe(_ => EndTurnButtonClicked().Forget())
            .AddTo(this);

        UpdateLighting();
        UpdateText();

        UpdateActionPointsText();
    }

    public void SetDateAndPeriod(DateTime newDate, PeriodOfDay newPeriod)
    {
        _currentDate.Value = newDate;
        _currentPeriod.Value = newPeriod;
    }

    public bool OnActionPointUsed()
    {
        if (actionPointsValue > 0)
        {
            actionPointsValue--;
            UpdateActionPointsText();
            return true;
        }
        else
        {
            Debug.Log("NO ACTION POINTS");
            return false;
        }
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
                _daysWithoutBombing++;

                if (_daysWithoutBombing == daysBetweenBombingRegularBuildings)
                {
                    _controllersManager.BuildingController.BombRegularBuilding();
                    _daysWithoutBombing = 0;
                    OnNextDayEvent.OnNext(Unit.Default);
                }
                break;
        }

        UpdateText();
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

    private void UpdateActionPointsText()
    {
        actionPointsText.text = $"ОД  {actionPointsValue.ToString()} / {actionPointsMaxValue.ToString()} ";
    }

    private async UniTaskVoid EndTurnButtonClicked()
    {
        nextTurnBtn.interactable = false;
        foreach (var script in btnScripts) script.enabled = false;

        await blackoutImage.DOFade(1, nextTurnFadeTime / 2).AsyncWaitForCompletion();
        UpdateTime();

        //await UniTask.Delay(100);

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
        actionPointsValue += actionPointsAddValueInTheNextDay;
        actionPointsValue = Math.Clamp(actionPointsValue, 0, actionPointsMaxValue);

        UpdateActionPointsText();
    }
}
