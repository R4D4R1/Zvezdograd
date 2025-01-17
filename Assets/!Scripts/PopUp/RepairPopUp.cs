using UnityEngine;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class RepairPopUp : EnoughPeoplePopUp
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

        _demandsText.text = $" - {_buildingToUse.PeopleToRepair} ������ ( � ��� {ControllersManager.Instance.peopleUnitsController.GetReadyUnits()} )\n" +
            $" - {_buildingToUse.BuildingMaterialsToRepair} ��������������� ( � ��� {ControllersManager.Instance.resourceController.GetReadyMaterials()} )\n" +
            $" - ������ {_buildingToUse.TurnsToRepair} �����\n" +
            $" - ������������� ����� �������� {_buildingToUse.TurnsToRestFromRepair} �����";

        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            IsActive = true;

            SetAlpha(1.0f);
        });
    }

    public void RepairBuilding()
    {

        if (EnoughPeopleTo(_buildingToUse.PeopleToRepair) && EnoughMaterialsToRepair())
        {
            HidePopUp();
            _buildingToUse.RepairBuilding();
        }
        else if (!EnoughPeopleTo(_buildingToUse.PeopleToRepair))
        {
            _errorText.text = "�� ���������� �����";
            _errorText.enabled = true;
        }
        else
        {
            _errorText.text = "�� ���������� ��������";
            _errorText.enabled = true;
        }
    }

    public bool EnoughMaterialsToRepair()
    {
        if (ControllersManager.Instance.resourceController.GetReadyMaterials() >= _buildingToUse.BuildingMaterialsToRepair)
            return true;
        else
            return false;
    }
}