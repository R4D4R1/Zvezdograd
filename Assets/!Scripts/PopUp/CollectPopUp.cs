using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CollectPopUp : EnoughPopUp
{
    private CollectableBuilding _buildingToUse;

    [SerializeField] protected TextMeshProUGUI _demandsText;

    private void Start()
    {
        _errorText.enabled = false;
        _isDestroyable = false;
    }

    public void ShowCollectPopUp(CollectableBuilding collectableBuilding)
    {
        // ÏÎÊÀÇÛÂÀÅÒ ÑÊÎËÜÊÎ ĞÅÑÎÂ ÎÑÒÀËÎÑÜ 
        // ÑÊÎËÜÊÎ ÄÎÁÓÄÅÒÜÑß
        // ÑÊÎËÜÊÎ ËŞÄEÉ
        // ÑÊÎËÜÊÎ ÕÎÄÎÂ

        _buildingToUse = collectableBuilding;

        _demandsText.text = $" - Îñòàëîñü {_buildingToUse.RawMaterialsLeft} ñûğüÿ \n" +
            $" - Âû ïîëó÷èòå {_buildingToUse.RawMaterialsGet} ñûğüÿ ( ó âàñ {ControllersManager.Instance.resourceController.GetRawMaterials()} )\n" +
            $" - Íåîáõîäèìî {_buildingToUse.PeopleToCollect} ïîäğàçäåëåíèé ( ó âàñ {ControllersManager.Instance.peopleUnitsController.GetReadyUnits()} )\n" +
            $" - Çàéìåò {_buildingToUse.TurnsToCollect} õîäîâ\n" +
            $" - Ïîäğàçäåëåíèÿ áóäóò îòäûõàòü {_buildingToUse.TurnsToRest} õîäîâ";

        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            IsActive = true;

            SetAlpha(1);
        });
    }

    public void CollectBuilding()
    {

        if (EnoughPeopleTo(_buildingToUse.PeopleToCollect) && EnoughRawMaterialsToStore() && EnoughRawMaterialsInBuilding())
        {
            HidePopUp();
            _buildingToUse.CollectBuilding();
        }
        else if (!EnoughPeopleTo(_buildingToUse.PeopleToCollect))
        {
            _errorText.text = "ÍÅ ÄÎÑÒÀÒÎ×ÍÎ ËŞÄÅÉ";
            _errorText.enabled = true;
        }
        else if (!EnoughRawMaterialsToStore())
        {
            _errorText.text = "ÍÅ ÄÎÑÒÀÒÎ×ÍÎ ÌÅÑÒÀ ÄËß ĞÅÑÓĞÑÎÂ";
            _errorText.enabled = true;
        }
        else
        {
            _errorText.text = "ĞÅÑÓĞÑÛ ÇÀÊÎÍ×ÈËÈÑÜ";
            _errorText.enabled = true;
        }
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