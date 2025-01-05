using UnityEngine;

public class CollectableBuilding : SelectableBuilding
{
    [field: SerializeField] public int RawMaterialsLeft { get; private set; }

    [field: SerializeField] public int RawMaterialsGet { get; private set; }

    [field: SerializeField] public int PeopleToCollect { get; private set; }

    [field: SerializeField] public int TurnsToCollect { get; private set; }

    [SerializeField] private Material originalMaterial;
    [SerializeField] private Material greyMaterial;

    private void OnEnable()
    {
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += TryTurnOnBuilding;
    }

    private void TryTurnOnBuilding()
    {
        if (!BuildingIsActive)
        {
            GetComponent<MeshRenderer>().material = originalMaterial;

            BuildingIsActive = true;
        }
    }

    public void CollectBuilding()
    {
        ControllersManager.Instance.peopleUnitsController.AssignUnitsToTask(PeopleToCollect, TurnsToCollect);
        RawMaterialsLeft -= RawMaterialsGet;
        ControllersManager.Instance.resourceController.AddOrRemoveRawMaterials(RawMaterialsGet);

        BuildingIsActive = false;

        GetComponent<MeshRenderer>().material = greyMaterial;
    }
}
