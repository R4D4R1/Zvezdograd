using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class GameData
{
    [JsonProperty]
    public TimeController.PeriodOfDay periodOfDay;
    [JsonProperty]
    public List<TimeController.DelayedAction> delayedActions = new();
    [JsonProperty]
    public string dateString;
    [JsonProperty]
    public int localIncreaseMaxAPValue;
    [JsonProperty]
    public int localIncreaseAddAPValue;
    [JsonProperty]
    public int currentActionPoints;

    [JsonProperty]
    public int provision;
    [JsonProperty]
    public int medicine;
    [JsonProperty]
    public int rawMaterials;
    [JsonProperty]
    public int readyMaterials;
    [JsonProperty]
    public int stability;

    [JsonProperty]
    public List<PeopleUnitData> allUnitsData;

    public DateTime GetDate() => DateTime.Parse(dateString);
    public void SetDate(DateTime date) => dateString = date.ToString("yyyy-MM-dd");
}

[JsonObject(MemberSerialization.OptIn)]
public class PeopleUnitData
{
    [JsonProperty]
    public Vector2 position;
    [JsonProperty]
    public PeopleUnit.UnitState currentState;
    [JsonProperty]
    public int busyTime;
    [JsonProperty]
    public int restingTime;
}