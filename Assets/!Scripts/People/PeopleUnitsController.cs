using UnityEngine;
using System.Collections.Generic;

public class PeopleUnitsController : MonoBehaviour
{
    public static PeopleUnitsController Instance;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [SerializeField] private List<PeopleUnit> allUnits;
    [SerializeField] private int _maxRestingTime = 6;
    private List<PeopleUnit> readyUnits;

    public int GetMaxRestingTime()
    {
        return _maxRestingTime;
    }

    void Start()
    {
        readyUnits = new List<PeopleUnit>();
        UpdateReadyUnits();

        //AssignUnitsToTask(2);
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
        if(units < readyUnits.Count)
        {
            return true;
        }
        else
            return false;
    }

    public void AssignUnitsToTask(int requiredUnits)
    {
        int assignedUnits = 0;

        UpdateReadyUnits();

        foreach (var unit in readyUnits)
        {
            if (assignedUnits < requiredUnits)
            {
                unit.SetBusy();
                unit.DisableUnit();
                assignedUnits++;
            }
        }
    }

    public void NextTurn()
    {
        UpdateRestingTimeForAllUnits();
        RestAllBusyUnits();
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
}
