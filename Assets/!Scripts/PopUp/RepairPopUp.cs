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
    }

    public void ShowRepairPopUp(RepairableBuilding buildingToRepair)
    {
        _buildingToUse = buildingToRepair;

        _demandsText.text = $" - {_buildingToUse.PeopleToRepair} ������ ( � ��� {_controllersManager.PeopleUnitsController.ReadyUnits.Count} )\n" +
                            $" - {_buildingToUse.BuildingMaterialsToRepair} ��������������� ( � ��� {_resourceViewModel.ReadyMaterials.Value} )\n" +
                            $" - ������ {_buildingToUse.TurnsToRepair} �����\n" +
                            $" - ������������� ����� �������� {_buildingToUse.TurnsToRestFromRepair} �����";

        transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
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
            _errorText.text = "�� ���������� �����";
        }
        else if (!EnoughMaterialsToRepair())
        {
            _errorText.text = "�� ���������� ��������";
        }
        _errorText.enabled = true;
    }

    public bool EnoughMaterialsToRepair()
    {
        return ChechIfEnoughResourcesByType(ResourceModel.ResourceType.ReadyMaterials, _buildingToUse.BuildingMaterialsToRepair);
    }
}
