using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class FactoryPopUp : InfoPopUp
{
    private FactoryBuilding _buildingToUse;
    [SerializeField] protected TextMeshProUGUI _errorText;
    [SerializeField] protected TextMeshProUGUI _createReadyMaterialButtonText;
    [SerializeField] protected TextMeshProUGUI _creatArmyMaterialButtonText;
    [SerializeField] protected TextMeshProUGUI _denyButtonText;

    // �� ������ �� ����� ������ ��������� �� �����
    // �� ����� ��������� �� ���������� ������ �������� ����� ����������
    // ������ ������� ��������� ��� ��������� ������� ������

    // �������
    // 2 ����� 1 �������� �� 3 ���� + 3 �����
    // 3 ����� 2 �������� �� 4 ���� + 4 ������

    // ��������� ������ ������� � ��������� �������

    public void ShowFactoryPopUp(FactoryBuilding factoryBuilding)
    {
        _buildingToUse = factoryBuilding;

        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            SetAlpha(1);
        });
    }

    public override void HidePopUp()
    {
        _bgImage.transform.DOScale(Vector3.zero, scaleDownDuration);

        ControllersManager.Instance.mainGameUIController.EnableEscapeMenuToggle();
        ControllersManager.Instance.mainGameUIController.TurnOnUI();
        ControllersManager.Instance.blurController.UnBlurBackGroundSmoothly();

        _errorText.enabled = false;

        SetAlpha(0);
    }

    public void CreateReadyMaterials()
    {
        if (EnoughPeopleToCreateReadyMaterials() && EnoughRawMaterialsForReadyMaterials() && EnoughSpaceToStoreReadyMaterials())
        {
            HidePopUp();
            _buildingToUse.CreateReadyMaterials();
        }
        else if(!EnoughRawMaterialsForReadyMaterials())
        {
            _errorText.text = "���� �����";
            _errorText.enabled = true;
        }
        else if (!EnoughSpaceToStoreReadyMaterials())
        {
            _errorText.text = "���� ����� ��� ����������";
            _errorText.enabled = true;
        }
        else if (!EnoughPeopleToCreateReadyMaterials())
        {
            _errorText.text = "�� ���������� �����";
            _errorText.enabled = true;
        }
    }

    public void CreateArmySupplies()
    {
        if (EnoughPeopleToCreateArmyMaterials() && EnoughRawMaterialsForArmyMaterrials())
        {
            HidePopUp();
            _buildingToUse.CreateArmyMaterials();
        }
        else if (!EnoughRawMaterialsForArmyMaterrials())
        {
            _errorText.text = "���� �����";
            _errorText.enabled = true;
        }
        else if (!EnoughPeopleToCreateReadyMaterials())
        {
            _errorText.text = "�� ���������� �����";
            _errorText.enabled = true;
        }
    }

    public bool EnoughSpaceToStoreReadyMaterials()
    {
        if (ControllersManager.Instance.resourceController.GetReadyMaterials() + _buildingToUse.ReadyMaterialsGet < ControllersManager.Instance.resourceController.GetMaxReadyMaterials())
            return true;
        else
            return false;
    }

    public bool EnoughPeopleToCreateArmyMaterials()
    {
        if (ControllersManager.Instance.peopleUnitsController.GetReadyUnits() >= _buildingToUse.PeopleToCreateReadyMaterials)
            return true;
        else
            return false;
    }

    public bool EnoughPeopleToCreateReadyMaterials()
    {
        if (ControllersManager.Instance.peopleUnitsController.GetReadyUnits() >= _buildingToUse.PeopleToCreateArmyMaterials)
            return true;
        else
            return false;
    }

    public bool EnoughRawMaterialsForReadyMaterials()
    {
        if (ControllersManager.Instance.resourceController.GetRawMaterials() >= _buildingToUse.RawMaterialsToCreateReadyMaterials)
            return true;
        else
            return false;
    }

    public bool EnoughRawMaterialsForArmyMaterrials()
    {
        if (ControllersManager.Instance.resourceController.GetRawMaterials() >= _buildingToUse.RawMaterialsToCreateArmyMaterials)
            return true;
        else
            return false;
    }
}
