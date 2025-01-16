using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CollectPopUp : DemandPopUp
{
    private CollectableBuilding _buildingToUse;

    public void ShowCollectPopUp(CollectableBuilding collectableBuilding)
    {
        // ÏÎÊÀÇÛÂÀÅÒ ÑÊÎËÜÊÎ ÐÅÑÎÂ ÎÑÒÀËÎÑÜ 
        // ÑÊÎËÜÊÎ ÄÎÁÓÄÅÒÜÑß
        // ÑÊÎËÜÊÎ ËÞÄEÉ
        // ÑÊÎËÜÊÎ ÕÎÄÎÂ

        _buildingToUse = collectableBuilding;

        _demandsText.text = $" - Îñòàëîñü {_buildingToUse.RawMaterialsLeft} ñûðüÿ \n" +
            $" - Âû ïîëó÷èòå {_buildingToUse.RawMaterialsGet} ñûðüÿ ( ó âàñ {ControllersManager.Instance.resourceController.GetRawMaterials()} )\n" +
            $" - Íåîáõîäèìî {_buildingToUse.PeopleToCollect} ïîäðàçäåëåíèé ( ó âàñ {ControllersManager.Instance.peopleUnitsController.GetReadyUnits()} )\n" +
            $" - Çàéìåò {_buildingToUse.TurnsToCollect} õîäîâ\n" +
            $" - Ïîäðàçäåëåíèÿ áóäóò îòäûõàòü {_buildingToUse.TurnsToRest} õîäîâ";

        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            IsActive = true;

            ControllersManager.Instance.mainGameUIController.InPopUp(this);

            SetAlpha(1);
        });
    }

    public void CollectBuilding()
    {

        if (EnoughPeopleToCollect() && EnoughRawMaterialsToStore() && EnoughRawMaterialsInBuilding())
        {
            HidePopUp();
            _buildingToUse.CollectBuilding();
        }
        else if (!EnoughPeopleToCollect())
        {
            _errorText.text = "ÍÅ ÄÎÑÒÀÒÎ×ÍÎ ËÞÄÅÉ";
            _errorText.enabled = true;
        }
        else if (!EnoughRawMaterialsToStore())
        {
            _errorText.text = "ÍÅ ÄÎÑÒÀÒÎ×ÍÎ ÌÅÑÒÀ ÄËß ÐÅÑÓÐÑÎÂ";
            _errorText.enabled = true;
        }
        else
        {
            _errorText.text = "ÐÅÑÓÐÑÛ ÇÀÊÎÍ×ÈËÈÑÜ";
            _errorText.enabled = true;
        }
    }

    public bool EnoughPeopleToCollect()
    {
        if (ControllersManager.Instance.peopleUnitsController.GetReadyUnits() >= _buildingToUse.PeopleToCollect)
            return true;
        else
            return false;
    }

    public bool EnoughRawMaterialsToStore()
    {
        if (ControllersManager.Instance.resourceController.GetRawMaterials() + _buildingToUse.RawMaterialsGet <= ControllersManager.Instance.resourceController.GetMaxRawMaterials())
            return true;
        else
            return false;
    }

    public bool EnoughRawMaterialsInBuilding()
    {
        if (_buildingToUse.RawMaterialsLeft > 0)
            return true;
        else
            return false;
    }
}