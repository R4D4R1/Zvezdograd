using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class FactoryPopUp : EnoughPeoplePopUp
{
    private FactoryBuilding _buildingToUse;
    [SerializeField] protected TextMeshProUGUI _createReadyMaterialButtonText;
    [SerializeField] protected TextMeshProUGUI _creatArmyMaterialButtonText;

    // �� ������ �� ����� ������ ��������� �� �����
    // �� ����� ��������� �� ���������� ������ �������� ����� ����������
    // ������ ������� ��������� ��� ��������� ������� ������

    // �������
    // 2 ����� 1 �������� �� 3 ���� + 3 �����
    // 3 ����� 2 �������� �� 4 ���� + 4 ������

    // ��������� ������ ������� � ��������� �������

    private void Start()
    {
        _errorText.enabled = false;
        _isDestroyable = false;
    }
    public void ShowFactoryPopUp(FactoryBuilding factoryBuilding)
    {
        _buildingToUse = factoryBuilding;

        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            IsActive = true;

            SetAlpha(1);
        });
    }

    public void CreateReadyMaterials()
    {
        if (EnoughPeopleTo(_buildingToUse.PeopleToCreateReadyMaterials) && EnoughRawMaterialsForReadyMaterials() && EnoughSpaceToStoreReadyMaterials())
        {
            HidePopUp();
            _buildingToUse.CreateReadyMaterials();
        }
        else if (!EnoughRawMaterialsForReadyMaterials())
        {
            _errorText.text = "���� �����";
            _errorText.enabled = true;
        }
        else if (!EnoughSpaceToStoreReadyMaterials())
        {
            _errorText.text = "���� ����� ��� ����������";
            _errorText.enabled = true;
        }
        else if (!EnoughPeopleTo(_buildingToUse.PeopleToCreateReadyMaterials))
        {
            _errorText.text = "�� ���������� �����";
            _errorText.enabled = true;
        }
    }

    public void CreateArmySupplies()
    {
        if (EnoughPeopleTo(_buildingToUse.PeopleToCreateArmyMaterials) && EnoughRawMaterialsForArmyMaterrials())
        {
            HidePopUp();
            _buildingToUse.CreateArmyMaterials();
        }
        else if (!EnoughRawMaterialsForArmyMaterrials())
        {
            _errorText.text = "���� �����";
            _errorText.enabled = true;
        }
        else if (!EnoughPeopleTo(_buildingToUse.PeopleToCreateArmyMaterials))
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
