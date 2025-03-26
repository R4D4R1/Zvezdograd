using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;
using UniRx;

public class BuildingController : MonoInit
{
    public List<SelectableBuilding> AllBuildings { get; private set; } = new();
    private List<RepairableBuilding> RegularBuildings { get; } = new();
    private List<RepairableBuilding> SpecialBuildings { get; } = new();

    public readonly Subject<Unit> OnBuildingBombed = new();

    private TimeController _timeController;
    private BuildingControllerConfig _buildingControllerConfig;
    
    [Inject]
    public void Construct(TimeController timeController,BuildingControllerConfig buildingControllerConfig)
    {
        _timeController = timeController;
        _buildingControllerConfig = buildingControllerConfig;
    }
    
    public override void Init()
    {
        base.Init();
        _timeController.OnNextTurnBtnClickBetween
            .Subscribe(_ => TryBombBuilding())
            .AddTo(this);
        
        AllBuildings = FindObjectsByType<SelectableBuilding>(FindObjectsSortMode.None).ToList();

        foreach (var building in AllBuildings)
        {
            if (building.GetComponent<RepairableBuilding>() != null)
            {
                if (building.GetComponent<SpecialBuilding>() != null)
                {
                    SpecialBuildings.Add(building as RepairableBuilding);
                }
                else
                {
                    RegularBuildings.Add(building as RepairableBuilding);
                }
            }
        }
    }

    private void TryBombBuilding()
    {
        var randomValue = Random.Range(0, 100);
        if (randomValue <= _buildingControllerConfig.ChanceOfBombingBuilding)
        {
            ChooseBuildingToBomb()?.BombBuilding();
            OnBuildingBombed.OnNext(Unit.Default);
        }
    }

    private RepairableBuilding ChooseBuildingToBomb()
    {
        RepairableBuilding buildingToReturn = null;

        for (var i = 0; i < SpecialBuildings.Count + RegularBuildings.Count; i++)
        {
            if (Random.Range(0, 100) <= _buildingControllerConfig.ChanceOfBombingSpecialBuilding)
            {
                var randomBuildingIndex = Random.Range(0, SpecialBuildings.Count);

                if (SpecialBuildings[randomBuildingIndex].CurrentState == RepairableBuilding.State.Intact
                    && SpecialBuildings[randomBuildingIndex].buildingIsSelectable)
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
        if (buildingToReturn)
        {
            return buildingToReturn;
        }
        else
        {
            Debug.Log("NO BUILDING FOR BOMBING");
            return null;
        }
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
