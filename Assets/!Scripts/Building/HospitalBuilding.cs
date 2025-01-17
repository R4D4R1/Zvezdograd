using UnityEngine;

public class HospitalBuilding : RepairableBuilding
{
    public static HospitalBuilding Instance;

    [field: SerializeField] public int PeopleToGiveMedicine { get; private set; }
    [field: SerializeField] public int TurnsToToGiveMedicine { get; private set; }
    [field: SerializeField] public int TurnsToRestFromMedicineJob { get; private set; }
    [field: SerializeField] public int MedicineToGive { get; private set; }

    private void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SendPeopleToGiveMedicine()
    {

    }
}
