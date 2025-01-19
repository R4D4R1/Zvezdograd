using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    public List<RepairableBuilding> RegularBuildings { get; private set; } = new();
    public List<RepairableBuilding> SpecialBuildings { get; private set; } = new();

    [Range(0f, 100f)]
    [SerializeField] private int _specialBuildingBombChance;

    private void Awake()
    {
        var allBuilding = FindObjectsByType<RepairableBuilding>(FindObjectsSortMode.None);

        foreach (var building in allBuilding)
        {
            if (building.GetComponent<SpecialBuilding>() != null)
            {
                SpecialBuildings.Add(building);
            }
            else
            {
                RegularBuildings.Add(building);
            }
        }
    }


    public void BombRegularBuilding()
    {
        ChooseBuildingToBomb().BombBuilding();
    }

    private RepairableBuilding ChooseBuildingToBomb()
    {
        RepairableBuilding buildingToReturn = null;
        while(buildingToReturn == null)
        {
            if (Random.Range(0, 100) <= _specialBuildingBombChance)
            {
                int randomBuildingIndex = Random.Range(0, SpecialBuildings.Count);
                if (SpecialBuildings[randomBuildingIndex].CurrentState == RepairableBuilding.State.Intact)
                {
                    buildingToReturn = SpecialBuildings[randomBuildingIndex];
                }
            }
            else
            {
                int randomBuildingIndex = Random.Range(0, RegularBuildings.Count);
                if (RegularBuildings[randomBuildingIndex].CurrentState == RepairableBuilding.State.Intact)
                {
                    buildingToReturn = RegularBuildings[randomBuildingIndex];
                }
            }
        }

        return buildingToReturn;

    }

    public CityHallBuilding GetCityHallBuilding()
    {
        return SpecialBuildings.OfType<CityHallBuilding>().FirstOrDefault();
    }

    public FoodTrucksBuilding GetFoodTruckBuilding()
    {
        return SpecialBuildings.OfType<FoodTrucksBuilding>().FirstOrDefault();
    }

    public HospitalBuilding GetHospitalBuilding()
    {
        return SpecialBuildings.OfType<HospitalBuilding>().FirstOrDefault();
    }
}