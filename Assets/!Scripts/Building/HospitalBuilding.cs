using UnityEngine;

public class HospitalBuilding : RepairableBuilding
{
    [field: SerializeField] public int PeopleToGiveMedicine { get; private set; }
    [field: SerializeField] public int TurnsToToGiveMedicine { get; private set; }
    [field: SerializeField] public int TurnsToRestFromMedicineJob { get; private set; }
    [field: SerializeField] public int MedicineToGive { get; private set; }

    public void SendPeopleToGiveMedicine()
    {

    }
}
