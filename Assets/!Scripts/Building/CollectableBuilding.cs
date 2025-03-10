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

    private int _turnsToWork = 0;


    protected override void Start()
    {
        base.Start();
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += TryTurnOnBuilding;
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += UpdateAmountOfTurnsNeededToDoSMTH;

        UpdateAmountOfTurnsNeededToDoSMTH();

        _turnsToWork = TurnsToCollectOriginal;
    }

    private void UpdateAmountOfTurnsNeededToDoSMTH()
    {
        TurnsToCollect = UpdateAmountOfTurnsNeededToDoSMTH(TurnsToCollectOriginal);
    }

    private void TryTurnOnBuilding()
    {
        if (!BuildingIsSelectable)
        {
            _turnsToWork--;
            if (_turnsToWork == 0)
            {
                RawMaterialsLeft -= RawMaterialsGet;
                ControllersManager.Instance.resourceController.ModifyResource(ResourceController.ResourceType.RawMaterials, RawMaterialsGet);

                BuildingIsSelectable = true;
                GetComponent<MeshRenderer>().material = originalMaterial;
            }
        }
    }

    public void CollectBuilding()
    {
        ControllersManager.Instance.peopleUnitsController.AssignUnitsToTask(PeopleToCollect, TurnsToCollect, TurnsToRest);

        _turnsToWork = TurnsToCollect;

        BuildingIsSelectable = false;

        GetComponent<MeshRenderer>().material = greyMaterial;
    }
}
