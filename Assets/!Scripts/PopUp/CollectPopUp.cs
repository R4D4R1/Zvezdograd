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
                            $" - Вы получите {_buildingToUse.RawMaterialsGet} сырья (у вас {_resourceViewModel.RawMaterials.Value})\n" +
                            $" - Необходимо {_buildingToUse.PeopleToCollect} подразделений (у вас {_controllersManager.PeopleUnitsController.ReadyUnits.Count})\n" +
                            $" - Займет {_buildingToUse.TurnsToCollect} ходов\n" +
                            $" - Подразделения будут отдыхать {_buildingToUse.TurnsToRest} ходов";
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
        if (_resourceViewModel.RawMaterials.Value + _buildingToUse.RawMaterialsGet <= _resourceViewModel.Model.MaxRawMaterials)
            return true;

        ShowError("НЕ ДОСТАТОЧНО МЕСТА ДЛЯ РЕСУРСОВ");
        return false;
    }

    private bool HasRawMaterialsInBuilding()
    {
        if (_buildingToUse.RawMaterialsLeft > 0)
            return true;

        ShowError("РЕСУРСЫ ЗАКОНЧИЛИСЬ");
        return false;
    }
}

