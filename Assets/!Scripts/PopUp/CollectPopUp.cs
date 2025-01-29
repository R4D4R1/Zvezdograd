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
        // ПОКАЗЫВАЕТ СКОЛЬКО РЕСОВ ОСТАЛОСЬ 
        // СКОЛЬКО ДОБУДЕТЬСЯ
        // СКОЛЬКО ЛЮДEЙ
        // СКОЛЬКО ХОДОВ

        _buildingToUse = collectableBuilding;

        _demandsText.text = $" - Осталось {_buildingToUse.RawMaterialsLeft} сырья \n" +
            $" - Вы получите {_buildingToUse.RawMaterialsGet} сырья ( у вас {ControllersManager.Instance.resourceController.GetRawMaterials()} )\n" +
            $" - Необходимо {_buildingToUse.PeopleToCollect} подразделений ( у вас {ControllersManager.Instance.peopleUnitsController.GetReadyUnits()} )\n" +
            $" - Займет {_buildingToUse.TurnsToCollect} ходов\n" +
            $" - Подразделения будут отдыхать {_buildingToUse.TurnsToRest} ходов";

        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            IsActive = true;

            SetAlpha(1);
        });
    }

    public void CollectBuilding()
    {

        if (CheckForEnoughPeople(_buildingToUse.PeopleToCollect) && EnoughRawMaterialsToStore() && EnoughRawMaterialsInBuilding())
        {
            HidePopUp();
            _buildingToUse.CollectBuilding();
        }
        else if (!CheckForEnoughPeople(_buildingToUse.PeopleToCollect))
        {
            _errorText.text = "НЕ ДОСТАТОЧНО ЛЮДЕЙ";
            _errorText.enabled = true;
        }
        else if (!EnoughRawMaterialsToStore())
        {
            _errorText.text = "НЕ ДОСТАТОЧНО МЕСТА ДЛЯ РЕСУРСОВ";
            _errorText.enabled = true;
        }
        else
        {
            _errorText.text = "РЕСУРСЫ ЗАКОНЧИЛИСЬ";
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