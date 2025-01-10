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


    protected override void TryTurnOnBuilding()
    {
        if (!BuildingIsActive)
        {
            var meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();

            _turnsToWork--;
            if (_turnsToWork == 0)
            {
                BuildingIsActive = true;

                ControllersManager.Instance.resourceController.AddOrRemoveReadyMaterials(ReadyMaterialsGet);

                if (meshRenderer != null)
                {
                    RestoreOriginalMaterials();
                }
            }
        }
    }

    public void CreateReadyMaterials()
    {
        ControllersManager.Instance.peopleUnitsController.AssignUnitsToTask(PeopleToCreateReadyMaterials, TurnsToCreateReadyMaterials, TurnsToRestFromReadyMaterialsJob);
        ControllersManager.Instance.resourceController.AddOrRemoveRawMaterials(-RawMaterialsToCreateReadyMaterials);

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
