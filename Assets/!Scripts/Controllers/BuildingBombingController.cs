using System.Collections.Generic;
using UnityEngine;

public class BuildingBombingController : MonoBehaviour
{
    public static BuildingBombingController Instance;

    List<RepairableBuilding> regularBuildings = new List<RepairableBuilding>();
    List<RepairableBuilding> specialBuildings = new List<RepairableBuilding>();

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
        var allBuilding = FindObjectsOfType<RepairableBuilding>();

        foreach (var building in allBuilding)
        {
            if (building.GetComponent<SpecialBuilding>() != null)
            {
                specialBuildings.Add(building);
                Debug.Log("Special Building = " + building.name);
            }
            else
            {
                regularBuildings.Add(building);
                Debug.Log("Regular Building = " + building.name);
            }
        }
    }


    public void BombRegularBuilding()
    {
        ChooseBuildingToBomb().BombBuilding();
    }

    private RepairableBuilding ChooseBuildingToBomb()
    {
        int randomBuildingIndex = UnityEngine.Random.Range(0, regularBuildings.Count);
        return regularBuildings[randomBuildingIndex];
    }
}