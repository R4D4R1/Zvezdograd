using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public TimeController.PeriodOfDay periodOfDay;
    public string dateString; // ������ ��� ����
    public List<PeopleUnitData> allUnitsData;

    // ����� ��� ��������� ���� ��� DateTime
    public DateTime GetDate()
    {
        return DateTime.Parse(dateString);
    }

    // ����� ��� ��������� ���� �� DateTime
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
