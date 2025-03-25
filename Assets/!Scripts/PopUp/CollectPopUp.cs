using DG.Tweening;
using TMPro;
using UnityEngine;

public class CollectPopUp : EnoughPopUp
{
    private CollectableBuilding _buildingToUse;
    [SerializeField] private TextMeshProUGUI _demandsText;

    public void ShowCollectPopUp(CollectableBuilding collectableBuilding)
    {
        _buildingToUse = collectableBuilding;
        UpdateDemandsText();

        ShowPopUp();
    }

    private void UpdateDemandsText()
    {
        _demandsText.text = $" - Осталось {_buildingToUse.RawMaterialsLeft} сырья\n" +
                            $" - Будет собрано {_buildingToUse.RawMaterialsGet} \n" +
                            $" - Необходимо {_buildingToUse.PeopleToCollect} подразделений \n";
    }

    public void CollectBuilding()
    {
        if (CanCollectBuilding())
        {
            HidePopUp();
            _buildingToUse.CollectBuilding();
        }
    }

    private bool CanCollectBuilding()
    {
        return HasEnoughPeople(_buildingToUse.PeopleToCollect) &&
               HasEnoughSpaceForRawMaterials() &&
               HasRawMaterialsInBuilding() &&
               CanUseActionPoint();
    }

    private bool HasEnoughSpaceForRawMaterials()
    {
        if (ResourceViewModel.RawMaterials.Value + _buildingToUse.RawMaterialsGet <= ResourceViewModel.Model.MaxRawMaterials)
            return true;

        ShowError("�� ���������� ����� ��� ��������");
        return false;
    }

    private bool HasRawMaterialsInBuilding()
    {
        if (_buildingToUse.RawMaterialsLeft > 0)
            return true;

        ShowError("������� �����������");
        return false;
    }
}