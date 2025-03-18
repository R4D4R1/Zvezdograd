using UnityEngine;
using UniRx;

public class CollectableBuilding : ChangeMaterialsBuiliding
{
    [SerializeField] private CollectableBuildingConfig config;

    public int RawMaterialsLeft { get; private set; }
    public int TurnsToCollect { get; private set; }
    public int RawMaterialsGet { get; private set; }
    public int PeopleToCollect { get; private set; }
    public int TurnsToRest { get; private set; }


    public override void Init()
    {
        base.Init();

        // Инициализация значений из конфига
        RawMaterialsLeft = config.RawMaterialsLeft;
        TurnsToCollect = config.TurnsToCollectOriginal;
        RawMaterialsGet = config.RawMaterialsGet;
        PeopleToCollect = config.PeopleToCollect;
        TurnsToRest = config.TurnsToRest;

        _controllersManager.TimeController.OnNextTurnBtnPressed
            .Subscribe(_ => TryTurnOnBuilding())
            .AddTo(this);

        _controllersManager.TimeController.OnNextTurnBtnPressed
            .Subscribe(_ => UpdateTurnsDependingOnStability())
            .AddTo(this);

        UpdateTurnsDependingOnStability();
    }

    private void UpdateTurnsDependingOnStability()
    {
        TurnsToCollect = UpdateAmountOfTurnsNeededToDoSMTH(config.TurnsToCollectOriginal);
    }

    private void TryTurnOnBuilding()
    {
        if (!BuildingIsSelectable)
        {
            TurnsToCollect--;
            if (TurnsToCollect == 0)
            {
                RawMaterialsLeft -= config.RawMaterialsGet;
                _resourceViewModel.ModifyResource(ResourceModel.ResourceType.RawMaterials, config.RawMaterialsGet);

                BuildingIsSelectable = true;
                RestoreOriginalMaterials();
            }
        }
    }

    public void CollectBuilding()
    {
        UpdateTurnsDependingOnStability();

        _controllersManager.PeopleUnitsController.AssignUnitsToTask(
            config.PeopleToCollect, TurnsToCollect, config.TurnsToRest);

        BuildingIsSelectable = false;

        SetGreyMaterials();
    }
}
