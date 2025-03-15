using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CityHallBuildingConfig", menuName = "Configs/CityHallBuildingConfig")]
public class CityHallBuildingConfig : ScriptableObject
{
    [Header("CITY HALL SETTINGS")]
    [SerializeField, Range(1, 5)] private int _daysLeftToSendArmyMaterialsOriginal;
    [SerializeField, Range(1, 5)] private int _daysLeftToRecieveGovHelpOriginal;
    [SerializeField, Range(1, 10)] private int _amountOfHelpNeededToSend;
    [SerializeField, Range(1, 5)] private int _readyMaterialsToCreateNewPeopleUnit;
    [SerializeField, Range(1, 10)] private int _relationWithGoverment;
    [SerializeField, Range(1, 5)] private int _turnsToCreateNewUnitOriginal;

    public int DaysLeftToSendArmyMaterialsOriginal => _daysLeftToSendArmyMaterialsOriginal;
    public int DaysLeftToRecieveGovHelpOriginal => _daysLeftToRecieveGovHelpOriginal;
    public int AmountOfHelpNeededToSend => _amountOfHelpNeededToSend;
    public int ReadyMaterialsToCreateNewPeopleUnit => _readyMaterialsToCreateNewPeopleUnit;
    public int RelationWithGoverment => _relationWithGoverment;
    public int TurnsToCreateNewUnitOriginal => _turnsToCreateNewUnitOriginal;
}
