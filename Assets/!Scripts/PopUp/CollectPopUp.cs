using TMPro;
using UniRx;
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
                            $" - Будет собрано {_buildingToUse.CollectableBuildingConfig.RawMaterialsGet} \n" +
                            $" - Необходимо {_buildingToUse.CollectableBuildingConfig.PeopleToCollect} подразделений \n";
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
        return HasEnoughPeople(_buildingToUse.CollectableBuildingConfig.PeopleToCollect) &&
               HasEnoughSpaceForResources(ResourceModel.ResourceType.RawMaterials,_buildingToUse.CollectableBuildingConfig.RawMaterialsGet) &&
               HasRawMaterialsInBuilding() &&
               CanUseActionPoint();
    }

    private bool HasRawMaterialsInBuilding()
    {
        if (_buildingToUse.RawMaterialsLeft > 0)
            return true;

        ShowError("Сырье кончилось");
        return false;
    }
}