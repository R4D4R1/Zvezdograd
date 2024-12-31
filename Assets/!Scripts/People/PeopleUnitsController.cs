using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;  // ��� �������� �����������

public class PeopleUnitsController : MonoBehaviour
{
    [SerializeField] private List<PeopleUnit> allUnits;  // ������ ���� ������
    private List<PeopleUnit> readyUnits = new();  // ������ ������� ������
    private Vector3[] initialPositions;  // ������ ��� �������� ��������� �������

    // ���������� ���������� ������� ������ � ������� ��� �������� � �������
    public int GetReadyUnits()
    {
        return readyUnits.Count;
    }

    // ���������� ��� ������������� �������
    private void Awake()
    {
        // �������� ��������� ������� ���� ������ � RectTransform
        initialPositions = new Vector3[allUnits.Count];
        for (int i = 0; i < allUnits.Count; i++)
        {
            initialPositions[i] = allUnits[i].GetComponent<RectTransform>().position;  // ��������� ������� �����
        }

        UpdateReadyUnits();
    }

    // �������� �� ������� �������� � ���������� ���� ��� ������ �������
    void Start()
    {
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += NextTurn;
    }

    // ������� �� ������� ��� ���������� �������
    private void OnDisable()
    {
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed -= NextTurn;
    }

    // ��������� ������ ������� ������
    void UpdateReadyUnits()
    {
        readyUnits.Clear();
        foreach (var unit in allUnits)
        {
            if (unit.GetCurrentState() == PeopleUnit.UnitState.Ready)
            {
                readyUnits.Add(unit);
            }
        }
    }

    // ���������, ���������� �� ������� ������ ��� ���������� �������
    public bool AreUnitsReady(int units)
    {
        return units <= readyUnits.Count;
    }

    // ��������� ������ �� �������, ���� �� ����������
    public void AssignUnitsToTask(int requiredUnits, int restingTurns)
    {
        if (AreUnitsReady(requiredUnits))
        {
            int assignedUnits = 0;
            UpdateReadyUnits();

            foreach (var unit in readyUnits)
            {
                if (assignedUnits < requiredUnits)
                {
                    unit.SetBusy();
                    unit.DisableUnit();
                    unit.SetRestingTime(restingTurns);
                    assignedUnits++;
                }
            }
        }
        else
        {
            Debug.Log("Not enough units");
        }

        SortUnitsByRestingTime();
    }

    // ����� ��� ���������� ������ �� ������� ������ �� �����������
    public void SortUnitsByRestingTime()
    {
        // ��������� ���� ������ �� ������� ������
        readyUnits.Sort((unit1, unit2) => unit1.restingTime.CompareTo(unit2.restingTime));

        // ������ ������������ ������ �� ��������� ��������
        for (int i = 0; i < readyUnits.Count; i++)
        {
            PeopleUnit unit = readyUnits[i];

            // ���������� ����� �� ��������� �������
            if (i < initialPositions.Length)
            {
                // ��������� �������� ����������� �� �������
                RectTransform rectTransform = unit.GetComponent<RectTransform>();
                rectTransform.DOMove(initialPositions[i], 0.5f).SetEase(Ease.InOutSine);
            }
        }
    }

    // �����, ���������� ��� �������� � ���������� ����
    public void NextTurn()
    {
        UpdateRestingTimeForAllUnits();
        RestAllBusyUnits();
        SortUnitsByRestingTime();  // ����� ���������� ������� ������, ��������� ������
    }

    // ��������� ��� ������� ����� � ��������� ������
    public void RestAllBusyUnits()
    {
        foreach (var unit in allUnits)
        {
            if (unit.GetCurrentState() == PeopleUnit.UnitState.Busy)
            {
                unit.UnitResting();
            }
        }
    }

    // ��������� ���������� ����� ������ ��� ���� ������, ����������� � ��������� ������
    public void UpdateRestingTimeForAllUnits()
    {
        foreach (var unit in allUnits)
        {
            if (unit.GetCurrentState() == PeopleUnit.UnitState.Resting)
            {
                unit.UpdateRestingTime();
            }
        }
    }
}
