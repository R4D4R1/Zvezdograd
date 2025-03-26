using UnityEngine;
using UniRx;
using UnityEngine.Serialization;

public class CollectableBuilding : ChangeMaterialsBuiliding
{
    [FormerlySerializedAs("_config")]
    [Header("COLLECTABLE CONFIG")]
    [SerializeField] private CollectableBuildingConfig config;

    public int RawMaterialsLeft { get; private set; }
    private int TurnsToCollect { get; set; }
    public int RawMaterialsGet { get; private set; }
    public int PeopleToCollect { get; private set; }
    public int TurnsToRest { get; private set; }

    private int _turnsToWork;


    public override void Init()
    {
        base.Init();

        TimeController.OnNextTurnBtnClickBetween
            .Subscribe(_ => TryTurnOnBuilding())
            .AddTo(this);

        TimeController.OnNextTurnBtnClickBetween
            .Subscribe(_ => UpdateTurnsDependingOnStability())
            .AddTo(this);

        UpdateTurnsDependingOnStability();

        RawMaterialsLeft = config.RawMaterialsLeft;
        TurnsToCollect = config.TurnsToCollectOriginal;
        RawMaterialsGet = config.RawMaterialsGet;
        PeopleToCollect = config.PeopleToCollect;
        TurnsToRest = config.TurnsToRest;
    }

    private void UpdateTurnsDependingOnStability()
    {
        TurnsToCollect = UpdateAmountOfTurnsNeededToDoSmth(config.TurnsToCollectOriginal);
    }

    private void TryTurnOnBuilding()
    {
        if (!buildingIsSelectable)
        {
            _turnsToWork--;

            if (_turnsToWork == 0)
            {
                RawMaterialsLeft -= config.RawMaterialsGet;
                ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.RawMaterials, config.RawMaterialsGet));

                buildingIsSelectable = true;
                RestoreOriginalMaterials();
            }
        }
    }

    public void CollectBuilding()
    {
        UpdateTurnsDependingOnStability();

        PeopleUnitsController.AssignUnitsToTask(
            config.PeopleToCollect, TurnsToCollect, config.TurnsToRest);

        _turnsToWork = TurnsToCollect;

        buildingIsSelectable = false;

        SetGreyMaterials();
    }
}
