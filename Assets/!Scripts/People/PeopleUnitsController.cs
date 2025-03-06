using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;

public class PeopleUnitsController : MonoBehaviour
{
    [SerializeField] private List<PeopleUnit> _allUnits;
    [Range(1f, 18f), SerializeField] private int _startPeopleUnitAmount;
    [Range(0f, 1f), SerializeField] private float _durationOfAnimationOfTransitionOfUnits;

    private List<PeopleUnit> _createdUnits = new();
    private Queue<PeopleUnit> _notCreatedUnits = new();
    private List<PeopleUnit> _readyUnits = new();
    private List<float> _initialPositions = new();


    void Start()
    {
        for (int unitNum = 0; unitNum < _allUnits.Count; unitNum++)
        {
            _initialPositions.Add(_allUnits[unitNum].transform.localPosition.x);

            if (unitNum >= _startPeopleUnitAmount)
            {

                _allUnits[unitNum].gameObject.SetActive(false);
                _allUnits[unitNum].SetNotCreated();

                _notCreatedUnits.Enqueue(_allUnits[unitNum]);

            }
            else
            {
                _createdUnits.Add(_allUnits[unitNum]);
                _createdUnits[unitNum].SetState(PeopleUnit.UnitState.Ready,0,0);
            }

        }

        UpdateReadyUnits();

        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += NextTurn;
    }

    private void OnDisable()
    {
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed -= NextTurn;
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
        _readyUnits.Clear();
        foreach (var unit in _allUnits)
        {
            if (unit.GetCurrentState() == PeopleUnit.UnitState.Ready)
            {
                _readyUnits.Add(unit);
            }
        }
    }

    // ����� ��� ��������� ������� ���-�� ������ � ��������� ������ � ������
    public void AssignUnitsToTask(int requiredUnits, int busyTurns, int restingTurns)
    {
        if (AreUnitsReady(requiredUnits))
        {
            int assignedUnits = 0;

            foreach (var unit in _readyUnits)
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
        if (_readyUnits.Count > 0)
        {
            // �������� ��������� ���� �� ������ readyUnits
            int randomIndex = Random.Range(0, _readyUnits.Count);
            PeopleUnit randomUnit = _readyUnits[randomIndex];

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
        var indexedUnits = _createdUnits
        .Select((unit, index) => (unit, index))
        .ToList();

        indexedUnits.Sort((a, b) =>
        {
            int result = (a.unit.BusyTime + a.unit.RestingTime).CompareTo(b.unit.BusyTime + b.unit.RestingTime);
            return result != 0 ? result : a.index.CompareTo(b.index);
        });

        _createdUnits = indexedUnits.Select(x => x.unit).ToList();


        for (int i = 0; i < _createdUnits.Count; i++)
        {
            _createdUnits[i].transform.DOLocalMoveX(_initialPositions[i], _durationOfAnimationOfTransitionOfUnits);
        }
    }

    // ����� ��� �������� �����
    public void CreateUnit()
    {
        if (_notCreatedUnits.Count > 0)
        {
            var unit = _notCreatedUnits.Dequeue();
            unit.gameObject.SetActive(true);
            unit.SetState(PeopleUnit.UnitState.Ready,0,0);

            _createdUnits.Add(unit);

            AnimateUnitPositions();
        }
    }

    public bool AreUnitsReady(int units)
    {
        return units <= _readyUnits.Count;
    }

    public int GetReadyUnits()
    {
        return _readyUnits.Count;
    }

    public List<PeopleUnit> GetAllUnits()
    {
        return _allUnits;
    }
}
