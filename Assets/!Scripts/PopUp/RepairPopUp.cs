using UnityEngine;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class RepairPopUp : EnoughPopUp
{
    private RepairableBuilding _buildingToUse;

    [SerializeField] protected TextMeshProUGUI _demandsText;

    private void Start()
    {
        _errorText.enabled = false;
        _isDestroyable = false;
    }

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
            SetAlpha(1.0f);
        });
    }

    public void RepairBuilding()
    {
        if (CheckForEnoughPeople(_buildingToUse.PeopleToRepair) && EnoughMaterialsToRepair())
        {
            HidePopUp();
            _buildingToUse.RepairBuilding();
        }
        else
        {
            ShowErrorMessage();
        }
    }

    private void ShowErrorMessage()
    {
        if (!CheckForEnoughPeople(_buildingToUse.PeopleToRepair))
        {
            _errorText.text = "ÍÅ ÄÎÑÒÀÒÎ×ÍÎ ËÞÄÅÉ";
        }
        else if (!EnoughMaterialsToRepair())
        {
            _errorText.text = "ÍÅ ÄÎÑÒÀÒÎ×ÍÎ ÐÅÑÓÐÑÎÂ";
        }
        _errorText.enabled = true;
    }

    public bool EnoughMaterialsToRepair()
    {
        return ControllersManager.Instance.resourceController.GetReadyMaterials() >= _buildingToUse.BuildingMaterialsToRepair;
    }
}
