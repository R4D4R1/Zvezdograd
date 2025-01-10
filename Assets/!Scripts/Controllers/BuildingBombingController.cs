using System.Collections.Generic;
using UnityEngine;

public class BuildingBombingController : MonoBehaviour
{
    public List<RepairableBuilding> RegularBuildings { get; private set; } = new();
    public List<RepairableBuilding> SpecialBuildings { get; private set; } = new();
    [SerializeField] private int _specialBuildingBombChance;

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
}