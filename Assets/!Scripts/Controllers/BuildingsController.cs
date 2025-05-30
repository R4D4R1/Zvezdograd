using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using UniRx;

public class BuildingsController : MonoInit
{
    public List<SelectableBuilding> AllBuildings { get; private set; } = new();
    private List<RepairableBuilding> RegularBuildings { get; } = new();
    private List<RepairableBuilding> SpecialBuildings { get; } = new();
    
    public readonly Subject<Unit> OnBuildingBombed = new();

    private TimeController _timeController;
    private BuildingControllerConfig _buildingControllerConfig;
    private ResourceViewModel _resourceViewModel;

    public CityHallBuilding GetCityHallBuilding() => GetSpecialBuilding<CityHallBuilding>();
    public FoodTrucksBuilding GetFoodTruckBuilding() => GetSpecialBuilding<FoodTrucksBuilding>();
    public HospitalBuilding GetHospitalBuilding() => GetSpecialBuilding<HospitalBuilding>();

    [Inject]
    public void Construct(TimeController timeController, BuildingControllerConfig buildingControllerConfig,
        ResourceViewModel resourceViewModel)
    {
        _timeController = timeController;
        _buildingControllerConfig = buildingControllerConfig;
        _resourceViewModel = resourceViewModel;
    }

    public override UniTask Init()
    {
        base.Init();

        _timeController.OnNextTurnBtnClickBetween
            .Subscribe(_ =>
            {
                TryBombBuilding();
                RemoveStabilityForEachBombedBuilding();
            })
            .AddTo(this);

        AllBuildings = FindObjectsByType<SelectableBuilding>(FindObjectsSortMode.None).ToList();

        foreach (var building in AllBuildings)
        {
            if (building.TryGetComponent(out RepairableBuilding repairable))
            {
                if (building.TryGetComponent(out SpecialBuilding _))
                    SpecialBuildings.Add(repairable);
                else
                    RegularBuildings.Add(repairable);
            }
            
            building.Init();
        }
        return UniTask.CompletedTask;
    }

    private void RemoveStabilityForEachBombedBuilding()
    {
        RemoveStability(RegularBuildings, _buildingControllerConfig.StabilityRemoveValueForRegularBombedBuilding);
        RemoveStability(SpecialBuildings, _buildingControllerConfig.StabilityRemoveValueForSpecialBombedBuilding);
    }

    private void RemoveStability(IEnumerable<RepairableBuilding> buildings, int value)
    {
        foreach (var building in buildings.Where(b => b.CurrentState == RepairableBuilding.State.Damaged))
        {
            _resourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Stability, -value));
        }
    }

    private void TryBombBuilding()
    {
        var randomValue = Random.Range(0, 100);
        if (randomValue <= _buildingControllerConfig.ChanceOfBombingBuilding)
        {
            var building = ChooseBuildingToBomb();
            if (building)
            {
                building.BombBuilding();
                OnBuildingBombed.OnNext(Unit.Default);
            }
        }
    }

    private RepairableBuilding ChooseBuildingToBomb()
    {
        var useSpecial = Random.Range(0, 100) < _buildingControllerConfig.ChanceOfBombingSpecialBuilding;

        List<RepairableBuilding> candidates;

        if (useSpecial)
        {
            candidates = SpecialBuildings
                .Where(b => b.CurrentState == RepairableBuilding.State.Intact && b.BuildingIsSelectable)
                .ToList();
        }
        else
        {
            candidates = RegularBuildings
                .Where(b => b.CurrentState == RepairableBuilding.State.Intact)
                .ToList();
        }

        if (!candidates.Any())
        {
            Debug.Log($"NO {(useSpecial ? "SPECIAL" : "REGULAR")} BUILDING FOR BOMBING");
            return null;
        }

        var factoryBuildings = GetAllSpecialBuildings<FactoryBuilding>();
        var damagedFactoriesCount = factoryBuildings.Count(f => f.CurrentState == RepairableBuilding.State.Damaged);

        // Перемешиваем кандидатов для случайного выбора
        var shuffled = candidates.OrderBy(_ => Random.value).ToList();

        foreach (var candidate in shuffled)
        {
            // если это завод и уже есть один разбомбленный — пропускаем
            if (candidate is FactoryBuilding && damagedFactoriesCount >= 1)
                continue;

            return candidate;
        }

        Debug.Log("NO VALID BUILDING TO BOMB");
        return null;
    }

    private T GetSpecialBuilding<T>() where T : RepairableBuilding
    {
        return SpecialBuildings.OfType<T>().FirstOrDefault();
    }

    private List<T> GetAllSpecialBuildings<T>() where T : RepairableBuilding
    {
        return SpecialBuildings.OfType<T>().ToList();
    }
    
}
