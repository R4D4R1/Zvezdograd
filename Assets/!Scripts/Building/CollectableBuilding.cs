using UnityEngine;

public class CollectableBuilding : BuildingDependingOnStability
{
    [field: SerializeField] public int RawMaterialsLeft { get; private set; }

    [field: SerializeField] public int RawMaterialsGet { get; private set; }

    [field: SerializeField] public int PeopleToCollect { get; private set; }

    [SerializeField] private int TurnsToCollectOriginal;
    public int TurnsToCollect { get; private set; }

    [field: SerializeField] public int TurnsToRest { get; private set; }

    [SerializeField] private Material originalMaterial;
    [SerializeField] private Material greyMaterial;

    protected override void Start()
    {
        base.Start();
        _controllersManager.TimeController.OnNextTurnBtnPressed += TryTurnOnBuilding;
        _controllersManager.TimeController.OnNextTurnBtnPressed += UpdateTurnsDependingOnStability;

        UpdateTurnsDependingOnStability();
    }

    private void UpdateTurnsDependingOnStability()
    {
        TurnsToCollect = UpdateAmountOfTurnsNeededToDoSMTH(TurnsToCollectOriginal);
    }

    private void TryTurnOnBuilding()
    {
        if (!BuildingIsSelectable)
        {
            TurnsToCollect--;
            if (TurnsToCollect == 0)
            {
                RawMaterialsLeft -= RawMaterialsGet;
                _resourceViewModel.ModifyResource(ResourceModel.ResourceType.RawMaterials, RawMaterialsGet);

                BuildingIsSelectable = true;
                GetComponent<MeshRenderer>().material = originalMaterial;
            }
        }
    }

    public void CollectBuilding()
    {
        UpdateTurnsDependingOnStability();

        _controllersManager.PeopleUnitsController.AssignUnitsToTask(PeopleToCollect, TurnsToCollect, TurnsToRest);

        BuildingIsSelectable = false;

        GetComponent<MeshRenderer>().material = greyMaterial;
    }
}
