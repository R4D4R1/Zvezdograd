using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;
using Zenject;
using UniRx;
using UnityEngine.Serialization;

public class PeopleUnitsController : MonoBehaviour
{
    private List<PeopleUnit> _allUnits;
    [Range(1f, 18f), SerializeField] private int startPeopleUnitAmount;
    [Range(0f, 1f), SerializeField] private float durationOfAnimationOfTransitionOfUnits;
    [Range(0f, 100f)] [SerializeField] private int chanceOfInjuringRandomReadyUnit;

    public List<PeopleUnit> ReadyUnits { get; } = new();
    public Queue<PeopleUnit> InjuredUnits { get; } = new();
    private List<PeopleUnit> CreatedUnits { get; set; } = new();
    public Queue<PeopleUnit> NotCreatedUnits { get; } = new();
    private List<float> InitialPositions { get; } = new();

    private ControllersManager _controllersManager;

    public readonly Subject<Unit> OnUnitCreatedByPeopleUnitController = new();
    public readonly Subject<Unit> OnUnitInjuredByPeopleUnitController = new();
    public readonly Subject<Unit> OnUnitHealedByPeopleUnitController = new();

    [Inject]
    public void Construct(ControllersManager controllersManager)
    {
        _controllersManager = controllersManager;
    }

    public void Init()
    {
        _controllersManager.TimeController.OnNextTurnBtnClickBetween
            .Subscribe(_ => NextTurn())
            .AddTo(this);
        
        _controllersManager.BombBuildingController.GetCityHallBuilding().OnCityHallUnitCreated
            .Subscribe(_ => CreateUnit())
            .AddTo(this);
        
        _controllersManager.BombBuildingController.GetHospitalBuilding().OnHospitaUnitHealed
            .Subscribe(_ => HealInuredUnit())
            .AddTo(this);

        _controllersManager.BombBuildingController.OnBuildingBombed
            .Subscribe(_ => TryInjureRandomReadyUnit())
            .AddTo(this);
        
        PeopleUnit anyUnit = FindFirstObjectByType<PeopleUnit>();

        if (anyUnit == null)
        {
            Debug.LogError("Не найден ни один PeopleUnit!");
            return;
        }

        Transform parent = anyUnit.transform.parent;

        _allUnits = new List<PeopleUnit>();
        foreach (Transform child in parent)
        {
            PeopleUnit unit = child.GetComponent<PeopleUnit>();
            if (unit != null)
            {
                _allUnits.Add(unit);
            }
        }

        for (int unitNum = 0; unitNum < _allUnits.Count; unitNum++)
        {
            InitialPositions.Add(_allUnits[unitNum].transform.localPosition.x);

            if (unitNum < startPeopleUnitAmount)
            {
                CreatedUnits.Add(_allUnits[unitNum]);
                CreatedUnits[unitNum].SetState(PeopleUnit.UnitState.Ready, 0, 0);
            }
            else
            {
                _allUnits[unitNum].SetState(PeopleUnit.UnitState.NotCreated, 0, 0);
                NotCreatedUnits.Enqueue(_allUnits[unitNum]);
            }
        }
        
        UpdateReadyUnits();
    }
    

    private void NextTurn()
    {
        foreach (var unit in _allUnits)
        {
            unit.UpdateUnitState();
        }

        UpdateReadyUnits();
        AnimateUnitPositions();
    }

    // МЕТОД ДЛЯ УСТАНОВКИ НЕОБХОД КОЛ-ВА ЮНИТОВ В СОСТОЯНИЕ РАБОТЫ И ОТДЫХА
    public void AssignUnitsToTask(int requiredUnits, int busyTurns, int restingTurns)
    {
        if (AreUnitsReady(requiredUnits))
        {
            int assignedUnits = 0;

            foreach (var unit in ReadyUnits)
            {
                if (assignedUnits < requiredUnits)
                {
                    unit.SetState(PeopleUnit.UnitState.Busy,busyTurns,restingTurns);
                    
                    // unit.SetBusyForTurns(busyTurns, restingTurns);
                    // unit.DisableUnit();
                    
                    assignedUnits++;
                }
            }

            UpdateReadyUnits();
            AnimateUnitPositions();
        }
        else
        {
            Debug.Log("Not enough units");
        }
    }

    // МЕТОД ДЛЯ РАНЕНИЯ ГОТОВЫХ К РАБОТЕ ЮНИТОВ
    private void TryInjureRandomReadyUnit()
    {
        var randomValue = Random.Range(0, 100);
        if (randomValue <= chanceOfInjuringRandomReadyUnit)
        {
            if (ReadyUnits.Count > 0)
            {
                // Выбираем случайный юнит из списка readyUnits
                int randomIndex = Random.Range(0, ReadyUnits.Count);
                PeopleUnit randomUnit = ReadyUnits[randomIndex];

                // Устанавливаем состояние юнита как Injured

                randomUnit.SetState(PeopleUnit.UnitState.Injured, 0, 0);
                InjuredUnits.Enqueue(randomUnit);

                OnUnitInjuredByPeopleUnitController.OnNext(Unit.Default);

                // Обновляем список готовых юнитов
                UpdateReadyUnits();
                AnimateUnitPositions();

                Debug.Log($"Unit {randomUnit.name} has been injured.");
            }
            else
            {
                Debug.Log("No ready units available to injure.");
            }
        }
    }

    // МЕТОД ДЛЯ СОЗДАНИЯ ЮНИТА
    private void CreateUnit()
    {
        var unit = NotCreatedUnits.Dequeue();

        unit.gameObject.SetActive(true);
        unit.SetState(PeopleUnit.UnitState.Ready, 0, 0);

        CreatedUnits.Add(unit);
        OnUnitCreatedByPeopleUnitController.OnNext(Unit.Default);
        AnimateUnitPositions();
    }
    
    private void HealInuredUnit()
    {
        var unit = InjuredUnits.Dequeue();
        unit.SetState(PeopleUnit.UnitState.Ready, 0, 0);
        
        OnUnitHealedByPeopleUnitController.OnNext(Unit.Default);
        AnimateUnitPositions();
    }

    private bool AreUnitsReady(int units)
    {
        return units <= ReadyUnits.Count;
    }

    // МЕТОД ДЛЯ ДОБАВЛЕНИЯ СВОБОДНЫХ ЮНИТОВ В ЛИСТ ДЛЯ ВЫБОРА ЮНИТА ДЛЯ РАБОТ
    private void UpdateReadyUnits()
    {
        ReadyUnits.Clear();
        foreach (var unit in _allUnits)
        {
            if (unit.GetCurrentState() == PeopleUnit.UnitState.Ready)
            {
                ReadyUnits.Add(unit);
            }
        }
    }
    
    // МЕТОД ДЛЯ АНИМАЦИИ СОЗДАНЫХ ЮНИТОВ
    private void AnimateUnitPositions()
    {
        var indexedUnits = CreatedUnits
            .Select((unit, index) => (unit, index))
            .ToList();

        indexedUnits.Sort((a, b) =>
        {
            int result = (a.unit.BusyTurns + a.unit.RestingTurns).CompareTo(b.unit.BusyTurns + b.unit.RestingTurns);
            return result != 0 ? result : a.index.CompareTo(b.index);
        });

        CreatedUnits = indexedUnits.Select(x => x.unit).ToList();


        for (int i = 0; i < CreatedUnits.Count; i++)
        {
            CreatedUnits[i].transform.DOLocalMoveX(InitialPositions[i], durationOfAnimationOfTransitionOfUnits);
        }
    }
    
    // public List<PeopleUnit> GetAllUnits()
    // {
    //     return _allUnits;
    // }
}
