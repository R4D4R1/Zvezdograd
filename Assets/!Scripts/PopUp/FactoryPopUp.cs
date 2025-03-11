using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FactoryPopUp : EnoughPopUp
{
    private FactoryBuilding _buildingToUse;

    [SerializeField] private TextMeshProUGUI _createReadyMaterialButtonText;
    [SerializeField] private TextMeshProUGUI _creatArmyMaterialButtonText;
    [SerializeField] private TextMeshProUGUI _readyMaterialsDemandsText;
    [SerializeField] private TextMeshProUGUI _armyMaterialsDemandsText;

    [SerializeField] private UnityEvent OnArmyBtnEnabled;
    [SerializeField] private UnityEvent OnArmyBtnDisabled;

    private void Start()
    {
        _errorText.enabled = false;
        _isDestroyable = false;
    }

    public void UpdateCreateArmyButtonState()
    {
        OnArmyBtnEnabled?.Invoke();
    }

    public void ShowFactoryPopUp(FactoryBuilding factoryBuilding)
    {
        _buildingToUse = factoryBuilding;

        transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            IsActive = true;

            _readyMaterialsDemandsText.text = $"ÍÅÎÁÕÎÄÈÌÎ \n {_buildingToUse.RawMaterialsToCreateReadyMaterials} ñûğüÿ \n" +
                                              $" {_buildingToUse.PeopleToCreateReadyMaterials} ïîäğàçäåëåíèÿ";

            _armyMaterialsDemandsText.text = $"ÍÅÎÁÕÎÄÈÌÎ \n {_buildingToUse.RawMaterialsToCreateArmyMaterials} ñûğüÿ \n" +
                                             $" {_buildingToUse.PeopleToCreateArmyMaterials} ïîäğàçäåëåíèÿ";

            SetAlpha(1);
        });
    }

    public void CreateReadyMaterials()
    {
        if (CheckForEnoughPeople(_buildingToUse.PeopleToCreateReadyMaterials) && EnoughRawMaterialsForReadyMaterials() && EnoughSpaceToStoreReadyMaterials())
        {
            HidePopUp();
            _buildingToUse.CreateReadyMaterials();
        }
        else
        {
            ShowErrorMessage();
        }
    }

    public void CreateArmySupplies()
    {
        if (CheckForEnoughPeople(_buildingToUse.PeopleToCreateArmyMaterials) && EnoughRawMaterialsForArmyMaterials())
        {
            OnArmyBtnDisabled?.Invoke();
            HidePopUp();
            _buildingToUse.CreateArmyMaterials();
        }
        else
        {
            ShowErrorMessage();
        }
    }

    private void ShowErrorMessage()
    {
        if (!EnoughRawMaterialsForReadyMaterials() || !EnoughRawMaterialsForArmyMaterials())
        {
            _errorText.text = "ÍÅ ÄÎÑÒÀÒÎ×ÍÎ ÑÛĞÜß";
        }
        else if (!EnoughSpaceToStoreReadyMaterials())
        {
            _errorText.text = "ÍÅ ÄÎÑÒÀÒÎ×ÍÎ ÌÅÑÒÀ ÄËß ÌÀÒÅĞÈÀËÎÂ";
        }
        else if (!CheckForEnoughPeople(_buildingToUse.PeopleToCreateReadyMaterials) || !CheckForEnoughPeople(_buildingToUse.PeopleToCreateArmyMaterials))
        {
            _errorText.text = "ÍÅ ÄÎÑÒÀÒÎ×ÍÎ ËŞÄÅÉ";
        }
        _errorText.enabled = true;
    }

    public bool EnoughSpaceToStoreReadyMaterials()
    {
        return _resourceViewModel.ReadyMaterials.Value + _buildingToUse.ReadyMaterialsGet
               < _resourceViewModel.Model.MaxReadyMaterials;
    }

    public bool EnoughRawMaterialsForReadyMaterials()
    {
        return ChechIfEnoughResourcesByType(ResourceModel.ResourceType.RawMaterials, _buildingToUse.RawMaterialsToCreateReadyMaterials);
    }

    public bool EnoughRawMaterialsForArmyMaterials()
    {
        return ChechIfEnoughResourcesByType(ResourceModel.ResourceType.RawMaterials, _buildingToUse.RawMaterialsToCreateArmyMaterials);
    }
}
