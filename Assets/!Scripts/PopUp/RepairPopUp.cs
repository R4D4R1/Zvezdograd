using UnityEngine;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class RepairPopUp : EnoughPopUp
{
    private RepairableBuilding _buildingToUse;

    [SerializeField] protected TextMeshProUGUI _demandsText;

    public void ShowRepairPopUp(RepairableBuilding buildingToRepair)
    {
        _buildingToUse = buildingToRepair;

        _demandsText.text = $" - {_buildingToUse.PeopleToRepair} отряда ( у вас {_controllersManager.PeopleUnitsController.ReadyUnits.Count} )\n" +
                            $" - {_buildingToUse.BuildingMaterialsToRepair} стройматериалов ( у вас {_resourceViewModel.ReadyMaterials.Value} )\n" +
                            $" - Займет {_buildingToUse.TurnsToRepair} ходов\n" +
                            $" - Подразделения будут отдыхать {_buildingToUse.TurnsToRestFromRepair} ходов";

        transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            IsActive = true;
            SetAlpha(1.0f);
        });
    }

    public void RepairBuilding()
    {
        if (CanRepairBuilding())
        {
            HidePopUp();

            _buildingToUse.RepairBuilding();
        }
    }

    private bool CanRepairBuilding()
    {
        return HasEnoughPeople(_buildingToUse.PeopleToRepair) &&
               EnoughMaterialsToRepair() &&
               CanUseActionPoint();
    }

    public bool EnoughMaterialsToRepair()
    {
        return HasEnoughResources(ResourceModel.ResourceType.ReadyMaterials, _buildingToUse.BuildingMaterialsToRepair);
    }
}
