using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    // TIME TURN DATA
    public TimeController.PeriodOfDay periodOfDay;
    public List<TimeController.DelayedAction> delayedActions = new();
    public string dateString;
    public int localIncreaseMaxAPValue;
    public int localIncreaseAddAPValue;
    public int currentActionPoints;
    
    //RESOURCES DATA
    public int provision;
    public int medicine;
    public int rawMaterials;
    public int readyMaterials;
    public int stability;
    
    public List<PeopleUnitData> allUnitsData;

    public DateTime GetDate()
    {
        return DateTime.Parse(dateString);
    }

    public void SetDate(DateTime date)
    {
        dateString = date.ToString("yyyy-MM-dd");
    }
}

[Serializable]
public class PeopleUnitData
{
    public Vector2 position;
    public PeopleUnit.UnitState currentState;
    public int busyTime;
    public int restingTime;
}