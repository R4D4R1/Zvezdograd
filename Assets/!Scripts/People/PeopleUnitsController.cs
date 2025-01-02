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

    // ¬озвращает количество готовых юнитов и выводит это значение в консоль
    public int GetReadyUnits()
    {
        //Debug.Log(readyUnits.Count);
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

    public bool AreUnitsReady(int units)
    {
        return units <= readyUnits.Count;
    }

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

            AnimateUnitPositions();
        }
        else
        {
            Debug.Log("Not enough units");
        }
    }

    public void NextTurn()
    {
        UpdateRestingTimeForAllUnits();
        RestAllBusyUnits();
        AnimateUnitPositions();
    }

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


    private void AnimateUnitPositions()
    {
        allUnits.Sort((x, y) => x.restingTime.CompareTo(y.restingTime));

        for (int i = 0; i < allUnits.Count; i++)
        {
            allUnits[i].transform.DOLocalMoveX(initialPositions[i], 0.5f);
        }
    }
}
