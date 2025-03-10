using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public TimeController.PeriodOfDay periodOfDay;
    public string dateString;
    public List<PeopleUnitData> allUnitsData;
    public List<BuildingData> allBuildingsData;

    public int provision;
    public int medicine;
    public int rawMaterials;
    public int readyMaterials;
    public int stability;

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

[Serializable]
public class BuildingData
{
    public int BuildingId;
    //public RepairableBuilding.State currentState;
    //public int turnsToRepair;
}