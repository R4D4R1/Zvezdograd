using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;

public class PeopleUnitsController : MonoBehaviour
{
    [SerializeField] private List<PeopleUnit> _allUnits;
    [Range(1f, 18f), SerializeField] private int _startPeopleUnitAmount;
    [Range(0f, 1f), SerializeField] private float _durationOfAnimationOfTransitionOfUnits;

    public List<PeopleUnit> ReadyUnits { get; private set; } = new();
    public List<PeopleUnit> CreatedUnits { get; private set; } = new();
    public Queue<PeopleUnit> NotCreatedUnits { get; private set; } = new();
    public List<float> InitialPositions { get; private set; } = new();


    void Start()
    {
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

        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += NextTurn;
        ControllersManager.Instance.buildingController.GetCityHallBuilding().OnNewUnitCreated += CreateUnit;
    }

    private void OnDisable()
    {
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed -= NextTurn;
        ControllersManager.Instance.buildingController.GetCityHallBuilding().OnNewUnitCreated -= CreateUnit;

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

    // ����� ��� ���������� ��������� ������ � ���� ��� ������ ����� ��� ������
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

    // ����� ��� ��������� ������� ���-�� ������ � ��������� ������ � ������
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

    // ����� ��� ������� ������� � ������ ������
    public void InjureRandomReadyUnit()
    {
        if (ReadyUnits.Count > 0)
        {
            // �������� ��������� ���� �� ������ readyUnits
            int randomIndex = Random.Range(0, ReadyUnits.Count);
            PeopleUnit randomUnit = ReadyUnits[randomIndex];

            // ������������� ��������� ����� ��� Injured
            randomUnit.SetInjured();
            randomUnit.DisableUnit();

            // ��������� ������ ������� ������
            UpdateReadyUnits();
            AnimateUnitPositions();

            Debug.Log($"Unit {randomUnit.name} has been injured.");
        }
        else
        {
            Debug.Log("No ready units available to injure.");
        }
    }

    // ����� ��� �������� �������� ������
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

    // ����� ��� �������� �����
    public void CreateUnit()
    {
        Debug.Log(NotCreatedUnits.Count);

        if (NotCreatedUnits.Count > 0)
        {
            var unit = NotCreatedUnits.Dequeue();
            Debug.Log(NotCreatedUnits.Count);

            unit.gameObject.SetActive(true);
            unit.SetState(PeopleUnit.UnitState.Ready,0,0);

            CreatedUnits.Add(unit);

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
