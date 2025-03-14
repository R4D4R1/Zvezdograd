using UnityEngine;

public class PopUpsController : MonoBehaviour
{
    public FoodTrucksPopUp FoodTrucksPopUp { get; private set; }
    public HospitalPopUp HospitalPopUp { get; private set; }
    public CityHallPopUp CityHallPopUp { get; private set; }
    public FactoryPopUp FactoryPopUp  { get; private set; }
    public CollectPopUp CollectPopUp  { get; private set; }
    public RepairPopUp RepairPopUp  { get; private set; }



    private void Start()
    {
        FoodTrucksPopUp = FindFirstObjectByType<FoodTrucksPopUp>();
        HospitalPopUp = FindFirstObjectByType<HospitalPopUp>();
        CityHallPopUp = FindFirstObjectByType<CityHallPopUp>();
        FactoryPopUp = FindFirstObjectByType<FactoryPopUp>();
        CollectPopUp = FindFirstObjectByType<CollectPopUp>();
        RepairPopUp = FindFirstObjectByType<RepairPopUp>();
    }
}
