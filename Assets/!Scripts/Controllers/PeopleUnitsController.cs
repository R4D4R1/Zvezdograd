using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Zenject;
using UniRx;

public class PeopleUnitsController : MonoInit
{
    public List<PeopleUnit> _allUnits;
    private PeopleUnitsControllerConfig _config;

    public List<PeopleUnit> ReadyUnits { get; } = new();
    public Queue<PeopleUnit> InjuredUnits { get; } = new();
    private List<PeopleUnit> CreatedUnits { get; set; } = new();
    public Queue<PeopleUnit> NotCreatedUnits { get; } = new();
    private List<float> InitialPositions { get; } = new();

    private TimeController _timeController;
    private BuildingsController _buildingsController;

    public readonly Subject<Unit> OnUnitCreatedByPeopleUnitController = new();
    public readonly Subject<Unit> OnUnitInjuredByPeopleUnitController = new();
    public readonly Subject<Unit> OnUnitHealedByPeopleUnitController = new();

    [Inject]
    public void Construct(PeopleUnitsControllerConfig config, BuildingsController buildingsController, TimeController timeController)
    {
        _config = config;
        _buildingsController = buildingsController;
        _timeController = timeController;
    }

    public override UniTask Init()
    {
        base.Init();

        _timeController.OnNextTurnBtnClickBetween
            .Subscribe(_ => NextTurn())
            .AddTo(this);

        _buildingsController.GetCityHallBuilding().OnCityHallUnitCreated
            .Subscribe(_ => CreateUnit())
            .AddTo(this);

        _buildingsController.GetHospitalBuilding().OnHospitalUnitHealed
            .Subscribe(_ => HealInuredUnit())
            .AddTo(this);

        _buildingsController.OnBuildingBombed
            .Subscribe(_ => TryInjureRandomReadyUnit())
            .AddTo(this);
        
        _buildingsController.GetCityHallBuilding().OnNewActionPointsStartedCreating
            .Subscribe(RemoveReadyUnitsForNewActionPoints)
            .AddTo(this);

        PeopleUnit anyUnit = FindFirstObjectByType<PeopleUnit>();

        if (!anyUnit)
        {
            Debug.LogError("Не найден ни один PeopleUnit!");
            return UniTask.CompletedTask;
        }

        var parent = anyUnit.transform.parent;

        _allUnits = new List<PeopleUnit>();
        foreach (Transform child in parent)
        {
            PeopleUnit unit = child.GetComponent<PeopleUnit>();
            if (unit)
            {
                _allUnits.Add(unit);
            }
        }

        for (int unitNum = 0; unitNum < _allUnits.Count; unitNum++)
        {
            InitialPositions.Add(_allUnits[unitNum].transform.localPosition.x);

            if (unitNum < _config.StartPeopleUnitAmount)
            {
                CreatedUnits.Add(_allUnits[unitNum]);
                SetReadyState(CreatedUnits[unitNum]);
            }
            else
            {
                _allUnits[unitNum].SetState(PeopleUnit.UnitState.NotCreated, 0, 0);
                NotCreatedUnits.Enqueue(_allUnits[unitNum]);
            }
        }

        UpdateReadyUnits();
        return UniTask.CompletedTask;
    }

    private void NextTurn()
    {
        foreach (var unit in _allUnits)
        {
            unit.UpdateUnitState();
        }

        UpdateReadyUnits();
    }

    public void AssignUnitsToTask(int requiredUnits, int busyTurns, int restingTurns)
    {
        if (AreUnitsReady(requiredUnits))
        {
            var assignedUnits = 0;

            foreach (var unit in ReadyUnits.Where(_ => assignedUnits < requiredUnits))
            {
                unit.SetState(PeopleUnit.UnitState.Busy, busyTurns, restingTurns);
                assignedUnits++;
            }

            UpdateReadyUnits();
        }
        else
        {
            Debug.Log("Not enough units");
        }
    }

    private void TryInjureRandomReadyUnit()
    {
        var randomValue = Random.Range(0, 100);
        if (randomValue <= _config.ChanceOfInjuringRandomReadyUnit)
        {
            if (ReadyUnits.Count > 0)
            {
                var randomIndex = Random.Range(0, ReadyUnits.Count);
                var randomUnit = ReadyUnits[randomIndex];

                randomUnit.SetState(PeopleUnit.UnitState.Injured, 0, 0);
                InjuredUnits.Enqueue(randomUnit);

                OnUnitInjuredByPeopleUnitController.OnNext(Unit.Default);

                UpdateReadyUnits();
            }
        }
    }

    private void CreateUnit()
    {
        var unit = NotCreatedUnits.Dequeue();

        unit.gameObject.SetActive(true);
        SetReadyState(unit);

        CreatedUnits.Add(unit);
        OnUnitCreatedByPeopleUnitController.OnNext(Unit.Default);
        UpdateReadyUnits();
    }

    private void HealInuredUnit()
    {
        var unit = InjuredUnits.Dequeue();
        SetReadyState(unit);

        OnUnitHealedByPeopleUnitController.OnNext(Unit.Default);
        UpdateReadyUnits();
    }

    private bool AreUnitsReady(int units) => units <= ReadyUnits.Count;

    public void UpdateReadyUnits()
    {
        ReadyUnits.Clear();

        foreach (var unit in _allUnits.Where(unit => unit.GetCurrentState() == PeopleUnit.UnitState.Ready))
        {
            ReadyUnits.Add(unit);
        }

        AnimateUnitPositions();
    }

    private void AnimateUnitPositions()
    {
        CreatedUnits = CreatedUnits
            .Select((unit, index) => (unit, index))
            .OrderBy(x => x.unit.BusyTurns + x.unit.RestingTurns)
            .ThenBy(x => x.index)
            .Select(x => x.unit)
            .ToList();

        for (var i = 0; i < CreatedUnits.Count; i++)
        {
            CreatedUnits[i].transform.DOLocalMoveX(InitialPositions[i], _config.DurationOfAnimationOfTransitionOfUnits);
        }
    }

    private void RemoveReadyUnitsForNewActionPoints(int amountOfReadyUnitsToCreateNewActionPoints)
    {
        for (int i = 0; i < amountOfReadyUnitsToCreateNewActionPoints; i++)
        {
            var randomIndex = Random.Range(0, ReadyUnits.Count);
            var unit = ReadyUnits[randomIndex];

            unit.SetState(PeopleUnit.UnitState.NotCreated, 0, 0);
            CreatedUnits.Remove(unit);
            UpdateReadyUnits();
        }
    }

    private void SetReadyState(PeopleUnit unit)
    {
        unit.SetState(PeopleUnit.UnitState.Ready, 0, 0);
    }
}
