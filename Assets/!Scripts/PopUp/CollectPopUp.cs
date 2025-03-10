using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CollectPopUp : EnoughPopUp
{
    private CollectableBuilding buildingToUse;

    [SerializeField] private TextMeshProUGUI demandsText;

    private void Start()
    {
        _errorText.enabled = false;
        _isDestroyable = false;
    }

    public void ShowCollectPopUp(CollectableBuilding collectableBuilding)
    {
        buildingToUse = collectableBuilding;

        demandsText.text = $" - Осталось {buildingToUse.RawMaterialsLeft} сырья\n" +
                           $" - Вы получите {buildingToUse.RawMaterialsGet} сырья (у вас {ControllersManager.Instance.resourceController.GetRawMaterials()})\n" +
                           $" - Необходимо {buildingToUse.PeopleToCollect} подразделений (у вас {ControllersManager.Instance.peopleUnitsController.ReadyUnits.Count})\n" +
                           $" - Займет {buildingToUse.TurnsToCollect} ходов\n" +
                           $" - Подразделения будут отдыхать {buildingToUse.TurnsToRest} ходов";

        transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            IsActive = true;
            SetAlpha(1);
        });
    }

    public void CollectBuilding()
    {
        if (CheckForEnoughPeople(buildingToUse.PeopleToCollect) && EnoughRawMaterialsToStore() && EnoughRawMaterialsInBuilding())
        {
            HidePopUp();
            buildingToUse.CollectBuilding();
        }
        else
        {
            ShowErrorMessage();
        }
    }

    private void ShowErrorMessage()
    {
        if (!CheckForEnoughPeople(buildingToUse.PeopleToCollect))
        {
            _errorText.text = "НЕ ДОСТАТОЧНО ЛЮДЕЙ";
        }
        else if (!EnoughRawMaterialsToStore())
        {
            _errorText.text = "НЕ ДОСТАТОЧНО МЕСТА ДЛЯ РЕСУРСОВ";
        }
        else
        {
            _errorText.text = "РЕСУРСЫ ЗАКОНЧИЛИСЬ";
        }
        _errorText.enabled = true;
    }

    public bool EnoughRawMaterialsToStore()
    {
        return ControllersManager.Instance.resourceController.GetRawMaterials() + buildingToUse.RawMaterialsGet <= ControllersManager.Instance.resourceController.GetMaxRawMaterials();
    }

    public bool EnoughRawMaterialsInBuilding()
    {
        return buildingToUse.RawMaterialsLeft > 0;
    }
}
