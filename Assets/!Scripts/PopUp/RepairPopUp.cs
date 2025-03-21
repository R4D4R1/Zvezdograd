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

        _demandsText.text = $" - {_buildingToUse.PeopleToRepair} подразделений\n" +
                            $" - {_buildingToUse.BuildingMaterialsToRepair} стройматериалов \n";
        ShowPopUp();
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
