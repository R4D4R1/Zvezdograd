using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class FactoryPopUp : EnoughPopUp
{
    private FactoryBuilding _buildingToUse;

    [SerializeField] private TextMeshProUGUI _createReadyMaterialButtonText;
    [SerializeField] private TextMeshProUGUI _creatArmyMaterialButtonText;
    [SerializeField] private TextMeshProUGUI _readyMaterialsDemandsText;
    [SerializeField] private TextMeshProUGUI _armyMaterialsDemandsText;

    [SerializeField] private UnityEvent OnArmyBtnEnabled;
    [SerializeField] private UnityEvent OnArmyBtnDisabled;

    public void UpdateCreateArmyButtonState()
    {
        OnArmyBtnEnabled?.Invoke();
    }

    public void ShowFactoryPopUp(FactoryBuilding factoryBuilding)
    {
        _buildingToUse = factoryBuilding;

        _readyMaterialsDemandsText.text = FormatResourceDemand(
                _buildingToUse.FactoryBuildingConfig.RawMaterialsToCreateReadyMaterials,
                _buildingToUse.FactoryBuildingConfig.PeopleToCreateReadyMaterials);

        _armyMaterialsDemandsText.text = FormatResourceDemand(
            _buildingToUse.FactoryBuildingConfig.RawMaterialsToCreateArmyMaterials,
            _buildingToUse.FactoryBuildingConfig.PeopleToCreateArmyMaterials);

        ShowPopUp();
    }

    public void CreateReadyMaterials()
    {
        if (CanCreateMaterials(_buildingToUse.FactoryBuildingConfig.PeopleToCreateReadyMaterials,
                               _buildingToUse.FactoryBuildingConfig.RawMaterialsToCreateReadyMaterials,
                               HasEnoughSpaceForResources(ResourceModel.ResourceType.ReadyMaterials,
                                   _buildingToUse.FactoryBuildingConfig.ReadyMaterialsGet)))
        {
            HidePopUp();
            _buildingToUse.CreateReadyMaterials();
        }
    }

    public void CreateArmySupplies()
    {
        if (CanCreateMaterials(_buildingToUse.FactoryBuildingConfig.PeopleToCreateArmyMaterials,
                               _buildingToUse.FactoryBuildingConfig.RawMaterialsToCreateArmyMaterials,
                               true))
        {
            OnArmyBtnDisabled?.Invoke();
            HidePopUp();
            _buildingToUse.CreateArmyMaterials();
        }
    }

    private bool CanCreateMaterials(int requiredPeople, int requiredRawMaterials, bool extraCondition)
    {
        return HasEnoughPeople(requiredPeople) &&
               HasEnoughResources(ResourceModel.ResourceType.RawMaterials, requiredRawMaterials) &&
               extraCondition &&
               CanUseActionPoint();
    }

    private string FormatResourceDemand(int rawMaterials, int people)
    {
        return $"Необходимо \n {rawMaterials} сырья \n {people} подразделений";
    }
}
