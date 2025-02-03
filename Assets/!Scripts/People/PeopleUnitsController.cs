using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class PeopleUnitsController : MonoBehaviour
{
    [SerializeField] private List<PeopleUnit> allUnits;
    private List<PeopleUnit> readyUnits = new();
    private List<float> initialPositions = new();

    private void Awake()
    {
        foreach (var unit in allUnits)
        {
            initialPositions.Add(unit.transform.localPosition.x);
        }

        UpdateReadyUnits();
    }

    void Start()
    {
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += NextTurn;
    }

    private void OnDisable()
    {
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed -= NextTurn;
    }

    public int GetReadyUnits()
    {
        return readyUnits.Count;
    }

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

    public void AssignUnitsToTask(int requiredUnits, int busyTurns, int restingTurns)
    {
        if (AreUnitsReady(requiredUnits))
        {
            int assignedUnits = 0;

            foreach (var unit in readyUnits)
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

    public void InjureRandomReadyUnit()
    {
        if (readyUnits.Count > 0)
        {
            // Выбираем случайный юнит из списка readyUnits
            int randomIndex = Random.Range(0, readyUnits.Count);
            PeopleUnit randomUnit = readyUnits[randomIndex];

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

    public void NextTurn()
    {
        foreach (var unit in allUnits)
        {
            unit.UpdateUnitState();
        }

        UpdateReadyUnits();
        AnimateUnitPositions();
    }

    private void AnimateUnitPositions()
    {
        //allUnits.Sort((x, y) => x.BusyTime.CompareTo(y.BusyTime))
        allUnits.Sort((x, y) => (x.BusyTime + x.RestingTime).CompareTo(y.BusyTime + y.RestingTime));

        for (int i = 0; i < allUnits.Count; i++)
        {
            allUnits[i].transform.DOLocalMoveX(initialPositions[i], 0.5f);
        }
    }

    public bool AreUnitsReady(int units)
    {
        return units <= readyUnits.Count;
    }
}
