using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public TimeController.PeriodOfDay periodOfDay;
    public string dateString; // строка для даты
    public List<PeopleUnitData> allUnitsData;

    // Метод для получения даты как DateTime
    public DateTime GetDate()
    {
        return DateTime.Parse(dateString);
    }

    // Метод для установки даты из DateTime
    public void SetDate(DateTime date)
    {
        dateString = date.ToString("yyyy-MM-dd");
    }
}

[System.Serializable]
public class PeopleUnitData
{
    public Vector2 position;
    public PeopleUnit.UnitState currentState;
    public int busyTime;
    public int restingTime;
}
