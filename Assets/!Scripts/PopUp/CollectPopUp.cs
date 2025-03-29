using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class CollectPopUp : EnoughPopUp
{
    private CollectableBuilding _buildingToUse;
    [FormerlySerializedAs("_demandsText")] [SerializeField] private TextMeshProUGUI demandsText;

    public void ShowCollectPopUp(CollectableBuilding collectableBuilding)
    {
        _buildingToUse = collectableBuilding;
        UpdateDemandsText();

        ShowPopUp();
    }

    private void UpdateDemandsText()
    {
        demandsText.text = $" - Осталось {_buildingToUse.RawMaterialsLeft} сырья\n" +
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
               HasEnoughSpaceForResources(ResourceModel.ResourceType.RawMaterials,_buildingToUse.RawMaterialsGet) &&
               //HasEnoughSpaceForRawMaterials() &&
               HasRawMaterialsInBuilding() &&
               CanUseActionPoint();
    }

    // private bool HasEnoughSpaceForRawMaterials()
    // {
    //     if (ResourceViewModel.RawMaterials.Value + _buildingToUse.RawMaterialsGet <= ResourceViewModel.Model.MaxRawMaterials)
    //         return true;
    //
    //     ShowError("�� ���������� ����� ��� ��������");
    //     return false;
    // }

    private bool HasRawMaterialsInBuilding()
    {
        if (_buildingToUse.RawMaterialsLeft > 0)
            return true;

        ShowError("Сырье кончилось");
        return false;
    }
}