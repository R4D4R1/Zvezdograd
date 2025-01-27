using UnityEngine;

public class PopUpsController : MonoBehaviour
{
    public FoodTrucksPopUp FoodTrucksPopUp { get; private set; }
    public HospitalPopUp HospitalPopUp { get; private set; }
    public CityHallPopUp CityHallPopUp { get; private set; }

    private void Start()
    {
        FoodTrucksPopUp = FindFirstObjectByType<FoodTrucksPopUp>();
        HospitalPopUp = FindFirstObjectByType<HospitalPopUp>();
        CityHallPopUp = FindFirstObjectByType<CityHallPopUp>();

        // Проверяем, все ли объекты найдены
        if (FoodTrucksPopUp == null)
            Debug.LogError("FoodTrucksPopUp not found on the scene!");
        if (HospitalPopUp == null)
            Debug.LogError("HospitalPopUp not found on the scene!");
        if (CityHallPopUp == null)
            Debug.LogError("CityHallPopUp not found on the scene!");
    }
}
