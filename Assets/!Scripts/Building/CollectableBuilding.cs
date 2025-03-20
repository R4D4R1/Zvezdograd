using UnityEngine;
using UniRx;

public class CollectableBuilding : ChangeMaterialsBuiliding
{
    [Header("COLLECTABLE CONFIG")]
    [SerializeField] private CollectableBuildingConfig _config;

    public int RawMaterialsLeft { get; private set; }
    public int TurnsToCollect { get; private set; }
    public int RawMaterialsGet { get; private set; }
    public int PeopleToCollect { get; private set; }
    public int TurnsToRest { get; private set; }

    private int _turnsToWork;


    public override void Init()
    {
        base.Init();

        _controllersManager.TimeController.OnNextTurnBtnClickBetween
            .Subscribe(_ => TryTurnOnBuilding())
            .AddTo(this);

        _controllersManager.TimeController.OnNextTurnBtnClickBetween
            .Subscribe(_ => UpdateTurnsDependingOnStability())
            .AddTo(this);

        UpdateTurnsDependingOnStability();

        RawMaterialsLeft = _config.RawMaterialsLeft;
        TurnsToCollect = _config.TurnsToCollectOriginal;
        RawMaterialsGet = _config.RawMaterialsGet;
        PeopleToCollect = _config.PeopleToCollect;
        TurnsToRest = _config.TurnsToRest;
    }

    private void UpdateTurnsDependingOnStability()
    {
        TurnsToCollect = UpdateAmountOfTurnsNeededToDoSMTH(_config.TurnsToCollectOriginal);
    }

    private void TryTurnOnBuilding()
    {
        if (!BuildingIsSelectable)
        {
            _turnsToWork--;

            if (_turnsToWork == 0)
            {
                RawMaterialsLeft -= _config.RawMaterialsGet;
                _resourceViewModel.ModifyResource(ResourceModel.ResourceType.RawMaterials, _config.RawMaterialsGet);

                BuildingIsSelectable = true;
                RestoreOriginalMaterials();
            }
        }
    }

    public void CollectBuilding()
    {
        UpdateTurnsDependingOnStability();

        _controllersManager.PeopleUnitsController.AssignUnitsToTask(
            _config.PeopleToCollect, TurnsToCollect, _config.TurnsToRest);

        _turnsToWork = TurnsToCollect;

        BuildingIsSelectable = false;

        SetGreyMaterials();
    }
}
