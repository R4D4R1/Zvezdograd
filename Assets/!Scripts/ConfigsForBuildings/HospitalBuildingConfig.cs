using UnityEngine;

[CreateAssetMenu(fileName = "HospitalBuildingConfig", menuName = "BuildingConfigs/HospitalBuildingConfig")]
public class HospitalBuildingConfig : ScriptableObject
{
    [Header("Hospital Settings")]
    [SerializeField] private int peopleToGiveMedicine;
    [SerializeField] private int medicineToGive;

    [Header("Turns Settings")]
    [SerializeField] private int turnsToGiveMedicineOriginal;
    [SerializeField] private int turnsToRestFromMedicineJob;

    [Header("Days Settings")]
    [SerializeField] private int originalDaysToGiveMedicine;

    [Header("Stability Modifiers")]
    [SerializeField] private int stabilityAddValue;
    [SerializeField] private int stabilityRemoveValue;

    public int PeopleToGiveMedicine => peopleToGiveMedicine;
    public int MedicineToGive => medicineToGive;
    public int TurnsToGiveMedicineOriginal => turnsToGiveMedicineOriginal;
    public int TurnsToRestFromMedicineJob => turnsToRestFromMedicineJob;
    public int OriginalDaysToGiveMedicine => originalDaysToGiveMedicine;
    public int StabilityAddValue => stabilityAddValue;
    public int StabilityRemoveValue => stabilityRemoveValue;
}
