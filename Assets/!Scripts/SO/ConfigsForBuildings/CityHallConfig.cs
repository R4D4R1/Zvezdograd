using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "CityHallBuildingConfig", menuName = "BuildingConfigs/CityHallBuildingConfig")]
public class CityHallBuildingConfig : ScriptableObject
{
    [Header("CITY HALL SETTINGS")]
    [SerializeField, Range(1, 5)] private int daysLeftToSendArmyMaterialsOriginal;
    [SerializeField, Range(1, 5)] private int daysLeftToReceiveGovHelpOriginal;
    [SerializeField, Range(1, 10)] private int amountOfHelpNeededToSend;
    [SerializeField, Range(1, 5)] private int readyMaterialsToCreateNewPeopleUnit;
    [SerializeField, Range(1, 10)] private int relationWithGovernmentOriginal;
    [SerializeField, Range(1, 5)] private int turnsToCreateNewUnitOriginal;
    [SerializeField, Range(1, 5)] private int turnsToCreateNewActionPoints;
    [SerializeField, Range(1, 3)] private int amountOfRelationAddForArmySent;

    public int DaysLeftToSendArmyMaterialsOriginal => daysLeftToSendArmyMaterialsOriginal;
    public int DaysLeftToReceiveGovHelpOriginal => daysLeftToReceiveGovHelpOriginal;
    public int AmountOfHelpNeededToSend => amountOfHelpNeededToSend;
    public int ReadyMaterialsToCreateNewPeopleUnit => readyMaterialsToCreateNewPeopleUnit;
    public int RelationWithGovernmentOriginal => relationWithGovernmentOriginal;
    public int TurnsToCreateNewUnitOriginal => turnsToCreateNewUnitOriginal;
    public int TurnsToCreateNewActionPoints => turnsToCreateNewActionPoints;
    public int AmountOfRelationAddForArmySent => amountOfRelationAddForArmySent;
}
