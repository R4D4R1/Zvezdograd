using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CollectPopUp : DemandPopUp
{
    private CollectableBuilding _buildingToUse;

    public void ShowCollectPopUp(CollectableBuilding collectableBuilding)
    {
        // ���������� ������� ����� �������� 
        // ������� ����������
        // ������� ���E�
        // ������� �����

        _buildingToUse = collectableBuilding;

        _demandsText.text = $" - �������� {_buildingToUse.RawMaterialsLeft} ����� \n" +
            $" - �� �������� {_buildingToUse.RawMaterialsGet} ����� ( � ��� {ControllersManager.Instance.resourceController.GetRawMaterials()} )\n" +
            $" - ���������� {_buildingToUse.PeopleToCollect} ������������� ( � ��� {ControllersManager.Instance.peopleUnitsController.GetReadyUnits()} )\n" +
            $" - ������ {_buildingToUse.TurnsToCollect} �����\n" +
            $" - ������������� ����� �������� {_buildingToUse.TurnsToRest} �����";

        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            IsActive = true;

            ControllersManager.Instance.mainGameUIController.InPopUp(this);

            SetAlpha(1);
        });
    }

    public void CollectBuilding()
    {

        if (EnoughPeopleToCollect() && EnoughRawMaterialsToStore() && EnoughRawMaterialsInBuilding())
        {
            HidePopUp();
            _buildingToUse.CollectBuilding();
        }
        else if (!EnoughPeopleToCollect())
        {
            _errorText.text = "�� ���������� �����";
            _errorText.enabled = true;
        }
        else if (!EnoughRawMaterialsToStore())
        {
            _errorText.text = "�� ���������� ����� ��� ��������";
            _errorText.enabled = true;
        }
        else
        {
            _errorText.text = "������� �����������";
            _errorText.enabled = true;
        }
    }

    public bool EnoughPeopleToCollect()
    {
        if (ControllersManager.Instance.peopleUnitsController.GetReadyUnits() >= _buildingToUse.PeopleToCollect)
            return true;
        else
            return false;
    }

    public bool EnoughRawMaterialsToStore()
    {
        if (ControllersManager.Instance.resourceController.GetRawMaterials() + _buildingToUse.RawMaterialsGet <= ControllersManager.Instance.resourceController.GetMaxRawMaterials())
            return true;
        else
            return false;
    }

    public bool EnoughRawMaterialsInBuilding()
    {
        if (_buildingToUse.RawMaterialsLeft > 0)
            return true;
        else
            return false;
    }
}