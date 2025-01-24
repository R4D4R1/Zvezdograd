using UnityEngine;

public class CollectableBuilding : SelectableBuilding
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


    private void Start()
    {
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += TryTurnOnBuilding;
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += UpdateAmountOfTurnsNeededToDoSMTH;

        UpdateAmountOfTurnsNeededToDoSMTH();

        _turnsToWork = TurnsToCollectOriginal;
    }

    private void UpdateAmountOfTurnsNeededToDoSMTH()
    {
        if (ControllersManager.Instance.resourceController.GetStability() > 75)
        {
            TurnsToCollect = TurnsToCollectOriginal - 1;
        }
        else if(ControllersManager.Instance.resourceController.GetStability() <= 75)
        {
            TurnsToCollect = TurnsToCollectOriginal;
        }
        else if(ControllersManager.Instance.resourceController.GetStability() <= 50)
        {
            TurnsToCollect = TurnsToCollectOriginal + 1;
        }
        else if (ControllersManager.Instance.resourceController.GetStability() <= 25)
        {
            TurnsToCollect = TurnsToCollectOriginal + 2;
        }
    }

    private void TryTurnOnBuilding()
    {
        if (!BuildingIsSelactable)
        {
            _turnsToWork--;
            if (_turnsToWork == 0)
            {
                RawMaterialsLeft -= RawMaterialsGet;
                ControllersManager.Instance.resourceController.AddOrRemoveRawMaterials(RawMaterialsGet);

                BuildingIsSelactable = true;
                GetComponent<MeshRenderer>().material = originalMaterial;
            }
        }
    }

    public void CollectBuilding()
    {
        ControllersManager.Instance.peopleUnitsController.AssignUnitsToTask(PeopleToCollect, TurnsToCollect, TurnsToRest);

        _turnsToWork = TurnsToCollect;

        BuildingIsSelactable = false;

        GetComponent<MeshRenderer>().material = greyMaterial;
    }
}
