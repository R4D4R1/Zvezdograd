using UnityEngine;

public class FactoryBuilding : RepairableBuilding
{
    [field: SerializeField] public int PeopleToCreateReadyMaterials { get; private set; }
    [field: SerializeField] public int PeopleToCreateArmyMaterials { get; private set; }
    [field: SerializeField] public int TurnsToCreateReadyMaterials { get; private set; }
    [field: SerializeField] public int TurnsToCreateArmyMaterials { get; private set; }
    [field: SerializeField] public int TurnsToRestFromReadyMaterialsJob { get; private set; }
    [field: SerializeField] public int TurnsToRestFromArmyMaterialsJob { get; private set; }
    [field: SerializeField] public int RawMaterialsToCreateReadyMaterials { get; private set; }
    [field: SerializeField] public int ReadyMaterialsGet { get; private set; }
    [field: SerializeField] public int RawMaterialsToCreateArmyMaterials { get; private set; }


    private void TryTurnOnBuilding()
    {
        _turnsToWork--;
        if (_turnsToWork == 0)
        {
            BuildingIsActive = true;
        }

        var meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.material = originalMaterial;
        }
    }

    public void CreateReadyMaterials()
    {
        ControllersManager.Instance.peopleUnitsController.AssignUnitsToTask(PeopleToCreateReadyMaterials, TurnsToCreateReadyMaterials, TurnsToRestFromReadyMaterialsJob);
        ControllersManager.Instance.resourceController.AddOrRemoveRawMaterials(ReadyMaterialsGet);

        _turnsToWork = TurnsToCreateReadyMaterials;

        BuildingIsActive = false;

        var meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.material = greyMaterial;
        }
    }

    public void CreateArmyMaterials()
    {
        Debug.Log("Army created");
    }
}
