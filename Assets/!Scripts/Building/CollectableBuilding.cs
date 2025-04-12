using System.Threading.Tasks;
using UnityEngine;
using UniRx;
using UnityEngine.Serialization;

public class CollectableBuilding : ChangeMaterialsBuilding, ISaveableBuilding
{
    [Header("COLLECTABLE CONFIG")]
    [SerializeField] private CollectableBuildingConfig config;
    public CollectableBuildingConfig CollectableBuildingConfig => config;

    public int RawMaterialsLeft { get; private set; }
    private int TurnsToCollect { get; set; }
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
    }

    private void UpdateTurnsDependingOnStability()
    {
        TurnsToCollect = UpdateAmountOfTurnsNeededToDoSmth(config.TurnsToCollectOriginal);
    }

    private void TryTurnOnBuilding()
    {
        if (!BuildingIsSelectable)
        {
            _turnsToWork--;

            if (_turnsToWork == 0)
            {
                RawMaterialsLeft -= config.RawMaterialsGet;
                ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.RawMaterials, config.RawMaterialsGet));

                BuildingIsSelectable = true;
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

        BuildingIsSelectable = false;

        SetGreyMaterials();
    }

    public new int BuildingID => base.BuildingId;

    public BuildingSaveData GetSaveData()
    {
        return new CollectableBuildingSaveData
        {
            buildingID = BuildingID,
            buildingIsSelectable = BuildingIsSelectable,
            rawMaterialsLeft = RawMaterialsLeft,
            turnsToCollect = TurnsToCollect,
            turnsToWork = _turnsToWork,
        };
    }

    public void LoadFromSaveData(BuildingSaveData data)
    {
        var save = data as CollectableBuildingSaveData;
        if (save == null) return;
        
        BuildingIsSelectable = save.buildingIsSelectable;
        RawMaterialsLeft = save.rawMaterialsLeft;
        TurnsToCollect = save.turnsToCollect;
        _turnsToWork = save.turnsToWork;
        
        if (save.buildingIsSelectable)
            RestoreOriginalMaterials();
        else
            SetGreyMaterials();
    }
}
