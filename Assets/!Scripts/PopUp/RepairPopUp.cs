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

        _demandsText.text = $" - {_buildingToUse.RepairableBuildingConfig.PeopleToRepair} подразделений\n" +
                            $" - {_buildingToUse.RepairableBuildingConfig.BuildingMaterialsToRepair} стройматериалов \n";
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
        return HasEnoughPeople(_buildingToUse.RepairableBuildingConfig.PeopleToRepair) &&
               HasEnoughResources(ResourceModel.ResourceType.ReadyMaterials,
                   _buildingToUse.RepairableBuildingConfig.BuildingMaterialsToRepair) &&
               CanUseActionPoint();
    }
}
