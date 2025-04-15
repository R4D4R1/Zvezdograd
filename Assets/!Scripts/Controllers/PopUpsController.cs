using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PopUpsController : MonoInit
{
    public FoodTrucksPopUp FoodTrucksPopUp { get; private set; }
    public HospitalPopUp HospitalPopUp { get; private set; }
    public CityHallPopUp CityHallPopUp { get; private set; }
    public FactoryPopUp FactoryPopUp  { get; private set; }
    public CollectPopUp CollectPopUp  { get; private set; }
    public RepairPopUp RepairPopUp  { get; private set; }
    public EventPopUp EventPopUp  { get; private set; }
    public List<FullScreenPopUp> AllPopUps{ get; private set; }

    public override UniTask Init()
    {
        base.Init();
        FoodTrucksPopUp = FindFirstObjectByType<FoodTrucksPopUp>();
        HospitalPopUp = FindFirstObjectByType<HospitalPopUp>();
        CityHallPopUp = FindFirstObjectByType<CityHallPopUp>();
        FactoryPopUp = FindFirstObjectByType<FactoryPopUp>();
        CollectPopUp = FindFirstObjectByType<CollectPopUp>();
        RepairPopUp = FindFirstObjectByType<RepairPopUp>();
        EventPopUp = FindFirstObjectByType<EventPopUp>();

        AllPopUps = new List<FullScreenPopUp>(FindObjectsByType<FullScreenPopUp>(FindObjectsSortMode.None));

        FoodTrucksPopUp.Init();
        HospitalPopUp.Init();
        CityHallPopUp.Init();
        FactoryPopUp.Init();
        CollectPopUp.Init();
        RepairPopUp.Init();
        return UniTask.CompletedTask;
    }
}