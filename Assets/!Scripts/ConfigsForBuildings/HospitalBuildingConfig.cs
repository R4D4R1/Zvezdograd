using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "HospitalBuildingConfig", menuName = "BuildingConfigs/HospitalBuildingConfig")]
public class HospitalBuildingConfig : ScriptableObject
{
    [Header("Hospital Settings")]
    [SerializeField, Range(1f, 10f)] private int peopleToGiveMedicine;
    [SerializeField, Range(1f, 10f)] private int medicineToGive;
    [SerializeField, Range(1f, 10f)] private int medicineToHealInjuredUnit;

    [Header("Turns Settings")]
    [SerializeField, Range(1f, 10f)] private int turnsToGiveMedicineOriginal;
    [SerializeField, Range(1f, 10f)] private int turnsToRestFromMedicine;
    [SerializeField, Range(1f, 10f)] private int turnsToHealInjuredUnit;

    [Header("Days Settings")]
    [SerializeField, Range(1f, 10f)] private int originalDaysToGiveMedicine;

    [Header("Stability Modifiers")]
    [SerializeField, Range(1f, 10f)] private int stabilityAddValue;
    [SerializeField, Range(1f, 10f)] private int stabilityRemoveValue;

    public int PeopleToGiveMedicine => peopleToGiveMedicine;
    public int MedicineToGive => medicineToGive;
    public int MedicineToHealInjuredUnit => medicineToHealInjuredUnit;
    public int TurnsToGiveMedicineOriginal => turnsToGiveMedicineOriginal;
    public int TurnsToRestFromMedicine => turnsToRestFromMedicine;
    public int TurnsToHealInjuredUnit => turnsToHealInjuredUnit;
    public int OriginalDaysToGiveMedicine => originalDaysToGiveMedicine;
    public int StabilityAddValue => stabilityAddValue;
    public int StabilityRemoveValue => stabilityRemoveValue;
}
