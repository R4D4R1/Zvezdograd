using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

public class FactoryPopUp : EnoughPopUp,ISaveablePopUp
{
    private FactoryBuilding _buildingToUse;

    [SerializeField] private TextMeshProUGUI createReadyMaterialButtonText;
    [SerializeField] private TextMeshProUGUI creatArmyMaterialButtonText;
    [SerializeField] private TextMeshProUGUI readyMaterialsDemandsText;
    [SerializeField] private TextMeshProUGUI armyMaterialsDemandsText;
    [SerializeField] private GameObject mainArmyBtn;
    [SerializeField] private GameObject backArmyBtn;

    private bool _isCreatingArmyMaterials;
    
    public override void Init()
    {
        base.Init();
        BuildingsController.GetCityHallBuilding().OnMilitaryHelpSent
            .Subscribe(_ => SetCreateArmyMaterialsBtnState(true))
            .AddTo(this);

        _isCreatingArmyMaterials = false;
    }

    public void ShowFactoryPopUp(FactoryBuilding factoryBuilding)
    {
        _buildingToUse = factoryBuilding;

        readyMaterialsDemandsText.text = FormatResourceDemand(
                _buildingToUse.FactoryBuildingConfig.RawMaterialsToCreateReadyMaterials,
                _buildingToUse.FactoryBuildingConfig.PeopleToCreateReadyMaterials);

        armyMaterialsDemandsText.text = FormatResourceDemand(
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
            SetCreateArmyMaterialsBtnState(false);
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

    private void SetCreateArmyMaterialsBtnState(bool activeState)
    {
        _isCreatingArmyMaterials = activeState;
        mainArmyBtn.SetActive(activeState);
        backArmyBtn.SetActive(!activeState);
    }

    public new int PopUpID => base.PopUpId;

    public PopUpSaveData GetSaveData()
    {
        return new FactoryPopUpSaveData()
        {
            popUpID = PopUpID,
            isCreatingArmyMaterials = _isCreatingArmyMaterials
        };
    }

    public void LoadFromSaveData(PopUpSaveData data)
    {
        var save = data as FactoryPopUpSaveData;
        if (save == null) return;
        
        SetCreateArmyMaterialsBtnState(!save.isCreatingArmyMaterials);
    }
}
