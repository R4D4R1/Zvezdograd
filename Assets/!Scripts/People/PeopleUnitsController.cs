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

    public bool AreUnitsReady(int units)
    {
        return units <= readyUnits.Count;
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

    public void NextTurn()
    {
        foreach (var unit in allUnits)
        {
            unit.UpdateUnitState();
            Debug.ClearDeveloperConsole();
            Debug.Log(unit.GetCurrentState().ToString());
        }

        UpdateReadyUnits();
        AnimateUnitPositions();
    }

    private void AnimateUnitPositions()
    {
        allUnits.Sort((x, y) => x.busyTime.CompareTo(y.busyTime));

        for (int i = 0; i < allUnits.Count; i++)
        {
            allUnits[i].transform.DOLocalMoveX(initialPositions[i], 0.5f);
        }
    }
}
