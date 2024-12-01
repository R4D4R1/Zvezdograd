using System.Collections.Generic;
using UnityEngine;

public class BuildingBombingController : MonoBehaviour
{
    public static BuildingBombingController Instance;

    public List<RepairableBuilding> RegularBuildings { get; private set; } = new();
    public List<RepairableBuilding> SpecialBuildings { get; private set; } = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        var allBuilding = FindObjectsByType<RepairableBuilding>(FindObjectsSortMode.None);

        foreach (var building in allBuilding)
        {
            if (building.GetComponent<SpecialBuilding>() != null)
            {
                SpecialBuildings.Add(building);
                //Debug.Log("Special Building = " + building.name);
            }
            else
            {
                RegularBuildings.Add(building);
                //Debug.Log("Regular Building = " + building.name);
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
            int randomBuildingIndex = Random.Range(0, RegularBuildings.Count);
            if(RegularBuildings[randomBuildingIndex].CurrentState == RepairableBuilding.State.Intact)
            {
                buildingToReturn = RegularBuildings[randomBuildingIndex];
            }
        }
        return buildingToReturn;

    }
}