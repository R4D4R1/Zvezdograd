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
                _buildingToUse.RawMaterialsToCreateReadyMaterials,
                _buildingToUse.PeopleToCreateReadyMaterials);

        _armyMaterialsDemandsText.text = FormatResourceDemand(
            _buildingToUse.RawMaterialsToCreateArmyMaterials,
            _buildingToUse.PeopleToCreateArmyMaterials);

        ShowPopUp();
    }

    public void CreateReadyMaterials()
    {
        if (CanCreateMaterials(_buildingToUse.PeopleToCreateReadyMaterials,
                               _buildingToUse.RawMaterialsToCreateReadyMaterials,
                               EnoughSpaceToStoreReadyMaterials()))
        {
            HidePopUp();
            _buildingToUse.CreateReadyMaterials();
        }
    }

    public void CreateArmySupplies()
    {
        if (CanCreateMaterials(_buildingToUse.PeopleToCreateArmyMaterials,
                               _buildingToUse.RawMaterialsToCreateArmyMaterials,
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

    private bool EnoughSpaceToStoreReadyMaterials()
    {
        if (_resourceViewModel.ReadyMaterials.Value + _buildingToUse.ReadyMaterialsGet
               < _resourceViewModel.Model.MaxReadyMaterials)
            return true;
        else
        {
            ShowError("Не достаточно места для стройматериалов");
            return false;
        }
    }

    private string FormatResourceDemand(int rawMaterials, int people)
    {
        return $"Необходимо \n {rawMaterials} сырья \n {people} подразделений";
    }
}
