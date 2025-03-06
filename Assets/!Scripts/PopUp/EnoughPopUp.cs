using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class EnoughPopUp : InfoPopUp
{
    [SerializeField] protected TextMeshProUGUI _errorText;

    public bool CheckForEnoughPeople(int peopleToDoSmth)
    {
        if (ControllersManager.Instance.peopleUnitsController.ReadyUnits.Count >= peopleToDoSmth)
            return true;
        else
            return false;
    }

    public override void HidePopUp()
    {
        base.HidePopUp();
        _errorText.enabled = false;
    }
    public bool ChechIfEnoughResourcesByType(ResourceController.ResourceType resourceType, int resourceAmountToCompare)
    {
        if (resourceType == ResourceController.ResourceType.Provision)
        {
            return ControllersManager.Instance.resourceController.GetProvision() >= resourceAmountToCompare;
        }
        else if (resourceType == ResourceController.ResourceType.Medicine)
        {
            return ControllersManager.Instance.resourceController.GetMedicine() >= resourceAmountToCompare;
        }
        else if (resourceType == ResourceController.ResourceType.RawMaterials)
        {
            return ControllersManager.Instance.resourceController.GetRawMaterials() >= resourceAmountToCompare;
        }
        else
        {
            return ControllersManager.Instance.resourceController.GetReadyMaterials() >= resourceAmountToCompare;
        }
    }
}
