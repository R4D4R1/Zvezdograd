using UnityEngine;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class RepairPopUp : DemandPopUp
{
    private RepairableBuilding _buildingToUse;

    public void ShowRepairPopUp(RepairableBuilding buildingToRepair)
    {
        _buildingToUse = buildingToRepair;

        _demandsText.text = $" - {_buildingToUse.PeopleToRepair} îòðÿäà ( ó âàñ {ControllersManager.Instance.peopleUnitsController.GetReadyUnits()} )\n" +
            $" - {_buildingToUse.BuildingMaterialsToRepair} ñòðîéìàòåðèàëîâ ( ó âàñ {ControllersManager.Instance.resourceController.GetReadyMaterials()} )\n" +
            $" - Çàéìåò {_buildingToUse.TurnsToRepair} õîäîâ\n" +
            $" - Ïîäðàçäåëåíèÿ áóäóò îòäûõàòü {_buildingToUse.TurnsToRestFromRepair} õîäîâ";

        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            IsActive = true;

            ControllersManager.Instance.mainGameUIController.InPopUp(this);

            SetAlpha(1.0f);
        });
    }

    public void RepairBuilding()
    {

        if (EnoughPeopleToReapir() && EnoughMaterialsToRepair())
        {
            HidePopUp();
            _buildingToUse.RepairBuilding();
        }
        else if (!EnoughPeopleToReapir())
        {
            _errorText.text = "ÍÅ ÄÎÑÒÀÒÎ×ÍÎ ËÞÄÅÉ";
            _errorText.enabled = true;
        }
        else
        {
            _errorText.text = "ÍÅ ÄÎÑÒÀÒÎ×ÍÎ ÐÅÑÓÐÑÎÂ";
            _errorText.enabled = true;
        }
    }

    public bool EnoughPeopleToReapir()
    {
        if (ControllersManager.Instance.peopleUnitsController.GetReadyUnits() >= _buildingToUse.PeopleToRepair)
            return true;
        else
            return false;
    }

    public bool EnoughMaterialsToRepair()
    {
        if (ControllersManager.Instance.resourceController.GetReadyMaterials() >= _buildingToUse.BuildingMaterialsToRepair)
            return true;
        else
            return false;
    }
}