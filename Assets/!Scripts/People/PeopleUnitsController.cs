using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;
using Zenject;

public class PeopleUnitsController : MonoBehaviour
{
    private List<PeopleUnit> _allUnits;
    [Range(1f, 18f), SerializeField] private int _startPeopleUnitAmount;
    [Range(0f, 1f), SerializeField] private float _durationOfAnimationOfTransitionOfUnits;

    public List<PeopleUnit> ReadyUnits { get; private set; } = new();
    public List<PeopleUnit> CreatedUnits { get; private set; } = new();
    public Queue<PeopleUnit> NotCreatedUnits { get; private set; } = new();
    public List<float> InitialPositions { get; private set; } = new();

    public event System.Action OnUnitCreatedByPeopleUnitController;

    protected ControllersManager _controllersManager;

    [Inject]
    public void Construct(ControllersManager controllersManager)
    {
        _controllersManager = controllersManager;
    }

    void Start()
    {
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

            if (unitNum >= _startPeopleUnitAmount)
            {

                _allUnits[unitNum].gameObject.SetActive(false);
                _allUnits[unitNum].SetNotCreated();

                NotCreatedUnits.Enqueue(_allUnits[unitNum]);

            }
            else
            {
                CreatedUnits.Add(_allUnits[unitNum]);
                CreatedUnits[unitNum].SetState(PeopleUnit.UnitState.Ready,0,0);
            }

        }

        UpdateReadyUnits();
    }

    private void OnEnable()
    {
        _controllersManager.TimeController.OnNextTurnBtnPressed += NextTurn;
        _controllersManager.BuildingController.GetCityHallBuilding().OnCityHallUnitCreated += CreateUnit;
    }

    private void OnDisable()
    {
        _controllersManager.TimeController.OnNextTurnBtnPressed -= NextTurn;
        _controllersManager.BuildingController.GetCityHallBuilding().OnCityHallUnitCreated -= CreateUnit;
    }

    public void NextTurn()
    {
        foreach (var unit in _allUnits)
        {
            unit.UpdateUnitState();
        }

        UpdateReadyUnits();
        AnimateUnitPositions();
    }

    // МЕТОД ДЛЯ ДОБАВЛЕНИЯ СВОБОДНЫХ ЮНИТОВ В ЛИСТ ДЛЯ ВЫБОРА ЮНИТА ДЛЯ РАБОЫТ
    public void UpdateReadyUnits()
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
                    unit.SetBusyForTurns(busyTurns, restingTurns);
                    unit.DisableUnit();
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
    public void InjureRandomReadyUnit()
    {
        if (ReadyUnits.Count > 0)
        {
            // Выбираем случайный юнит из списка readyUnits
            int randomIndex = Random.Range(0, ReadyUnits.Count);
            PeopleUnit randomUnit = ReadyUnits[randomIndex];

            // Устанавливаем состояние юнита как Injured
            randomUnit.SetInjured();
            randomUnit.DisableUnit();

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

    // МЕТОД ДЛЯ АНИМАЦИИ СОЗДАНЫХ ЮНИТОВ
    private void AnimateUnitPositions()
    {
        var indexedUnits = CreatedUnits
        .Select((unit, index) => (unit, index))
        .ToList();

        indexedUnits.Sort((a, b) =>
        {
            int result = (a.unit.BusyTime + a.unit.RestingTime).CompareTo(b.unit.BusyTime + b.unit.RestingTime);
            return result != 0 ? result : a.index.CompareTo(b.index);
        });

        CreatedUnits = indexedUnits.Select(x => x.unit).ToList();


        for (int i = 0; i < CreatedUnits.Count; i++)
        {
            CreatedUnits[i].transform.DOLocalMoveX(InitialPositions[i], _durationOfAnimationOfTransitionOfUnits);
        }
    }

    // МЕТОД ДЛЯ СОЗДАНИЯ ЮНИТА
    public void CreateUnit()
    {
        if (NotCreatedUnits.Count > 0)
        {
            var unit = NotCreatedUnits.Dequeue();

            unit.gameObject.SetActive(true);
            unit.SetState(PeopleUnit.UnitState.Ready,0,0);

            CreatedUnits.Add(unit);
            OnUnitCreatedByPeopleUnitController?.Invoke();
            AnimateUnitPositions();
        }
    }

    public bool AreUnitsReady(int units)
    {
        return units <= ReadyUnits.Count;
    }

    public List<PeopleUnit> GetAllUnits()
    {
        return _allUnits;
    }
}
