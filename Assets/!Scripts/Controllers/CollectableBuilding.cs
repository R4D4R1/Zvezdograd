using UnityEngine;

public class CollectableBuilding : SelectableBuilding
{
    [field: SerializeField] public int RawMaterialsLeft { get; private set; }

    [field: SerializeField] public int RawMaterialsGet { get; private set; }

    [field: SerializeField] public int PeopleToCollect { get; private set; }

    [field: SerializeField] public int TurnsToCollect { get; private set; }


    public void CollectBuilding()
    {
            ControllersManager.Instance.peopleUnitsController.AssignUnitsToTask(PeopleToCollect, TurnsToCollect);
            ControllersManager.Instance.resourceController.AddOrRemoveRawMaterials(RawMaterialsGet);
    }
}
