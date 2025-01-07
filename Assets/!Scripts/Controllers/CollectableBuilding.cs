using UnityEngine;

public class CollectableBuilding : SelectableBuilding
{
    [field: SerializeField] public int RawMaterialsLeft { get; private set; }

    [field: SerializeField] public int RawMaterialsGet { get; private set; }

    [field: SerializeField] public int PeopleToCollect { get; private set; }

    [field: SerializeField] public int TurnsToCollect { get; private set; }

    [field: SerializeField] public int TurnsToRest { get; private set; }

    [SerializeField] private Material originalMaterial;
    [SerializeField] private Material greyMaterial;

    private int _turnsToWork = 0;

    private void OnEnable()
    {
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += TryTurnOnBuilding;
    }

    private void Start()
    {
        _turnsToWork = TurnsToCollect;
    }

    private void TryTurnOnBuilding()
    {
        _turnsToWork--;
        if (_turnsToWork == 0)
        {
            BuildingIsActive = true;
        }

        if (BuildingIsActive)
        {
            GetComponent<MeshRenderer>().material = originalMaterial;
        }
    }

    public void CollectBuilding()
    {
        ControllersManager.Instance.peopleUnitsController.AssignUnitsToTask(PeopleToCollect, TurnsToCollect, TurnsToRest);
        RawMaterialsLeft -= RawMaterialsGet;
        ControllersManager.Instance.resourceController.AddOrRemoveRawMaterials(RawMaterialsGet);

        _turnsToWork = TurnsToCollect;

        BuildingIsActive = false;

        GetComponent<MeshRenderer>().material = greyMaterial;
    }
}
