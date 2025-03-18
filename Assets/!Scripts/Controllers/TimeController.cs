using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Zenject;
using UniRx;

public class TimeController : MonoBehaviour
{
    [SerializeField] private Light morningLight;
    [SerializeField] private Light dayLight;
    [SerializeField] private Light eveningLight;

    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI periodText;
    [SerializeField] private Image blackoutImage;

    [SerializeField] private TextMeshProUGUI _actionPointsText;
    [Range(1f, 10f), SerializeField] private int _actionPointsValue;
    [Range(1f, 10f), SerializeField] private int _actionPointsMaxValue;
    [Range(1f, 5f), SerializeField] private int _actionPointsAddValueInTheNextDay;

    [Range(1f, 5f), SerializeField] private int _daysBetweenBombingRegularBuildings;
    [Range(1f, 5f), SerializeField] private int _daysBetweenBombingSpecialBuildings;
    [Range(1.02f, 2f), SerializeField] private float _nextTurnFadeTime;

    [SerializeField] private Button _nextTurnBtn;
    [SerializeField] private MonoBehaviour[] _btnScripts;

    public Button NextTurnButton => _nextTurnBtn;

    private readonly DateTime _startDate = new DateTime(1942, 8, 30);
    private int _daysWithoutBombing;

    private ReactiveProperty<DateTime> _currentDate;
    private ReactiveProperty<PeriodOfDay> _currentPeriod;
    public DateTime CurrentDate => _currentDate.Value;
    public PeriodOfDay CurrentPeriod => _currentPeriod.Value;


    private ControllersManager _controllersManager;
    private ResourceViewModel _resourceViewModel;
    private EventPopUp _eventPopUp;

    public readonly Subject<Unit> OnNextTurnBtnPressed = new();
    public readonly Subject<Unit> OnNextDayEvent = new();
    public enum PeriodOfDay
    {
        Утро,
        День,
        Вечер
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

        // Подписки на обновления
        _currentDate.Subscribe(_ => UpdateText()).AddTo(this);
        _currentPeriod.Subscribe(_ => UpdateLighting()).AddTo(this);

        // Подписка на клик кнопки
        _nextTurnBtn.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(0.5)) // Антиспам
            .Subscribe(_ => EndTurnButtonClicked().Forget())
            .AddTo(this);

        UpdateLighting();
        UpdateText();

        UpdateActionPointsText();

        Debug.Log($"{name} - Initialized successfully");
    }

    public void SetDateAndPeriod(DateTime newDate, PeriodOfDay newPeriod)
    {
        _currentDate.Value = newDate;
        _currentPeriod.Value = newPeriod;
    }

    public bool OnActionPointUsed()
    {
        if (_actionPointsValue > 0)
        {
            _actionPointsValue--;
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

                if (_daysWithoutBombing == _daysBetweenBombingRegularBuildings)
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
        _actionPointsText.text = $"ОЧКИ ДЕЙСТВИЯ  {_actionPointsValue.ToString()}/{_actionPointsMaxValue.ToString()} ";
    }

    private async UniTaskVoid EndTurnButtonClicked()
    {
        _nextTurnBtn.interactable = false;
        foreach (var script in _btnScripts) script.enabled = false;

        await blackoutImage.DOFade(1, _nextTurnFadeTime / 2).AsyncWaitForCompletion();
        UpdateTime();

        await UniTask.Delay(100);

        OnNextTurnBtnPressed.OnNext(Unit.Default);
        AddActionPoints();

        await blackoutImage.DOFade(0, _nextTurnFadeTime / 2).AsyncWaitForCompletion();

        _nextTurnBtn.interactable = true;
        foreach (var script in _btnScripts) script.enabled = true;

        if (!_eventPopUp.IsActive)
        {
            _controllersManager.SelectionController.enabled = true;
        }
    }

    private void AddActionPoints()
    {
        _actionPointsValue += _actionPointsAddValueInTheNextDay;
        _actionPointsValue = Math.Clamp(_actionPointsValue, 0, _actionPointsMaxValue);

        UpdateActionPointsText();
    }
}
