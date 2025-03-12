using TMPro;
using UnityEngine;

public class EnoughPopUp : InfoPopUp
{
    [SerializeField] protected TextMeshProUGUI _errorText;

    public bool CheckForEnoughPeople(int peopleToDoSmth)
    {
        if (_controllersManager.PeopleUnitsController.ReadyUnits.Count >= peopleToDoSmth)
            return true;
        else
            return false;
    }

    public override void HidePopUp()
    {
        base.HidePopUp();
        _errorText.enabled = false;
    }

    public bool ChechIfEnoughResourcesByType(ResourceModel.ResourceType resourceType, int resourceAmountToCompare)
    {
        if (resourceType == ResourceModel.ResourceType.Provision)
        {
            return _resourceViewModel.Provision.Value >= resourceAmountToCompare;
        }
        else if (resourceType == ResourceModel.ResourceType.Medicine)
        {
            return _resourceViewModel.Medicine.Value >= resourceAmountToCompare;
        }
        else if (resourceType == ResourceModel.ResourceType.RawMaterials)
        {
            return _resourceViewModel.RawMaterials.Value >= resourceAmountToCompare;
        }
        else
        {
            return _resourceViewModel.ReadyMaterials.Value >= resourceAmountToCompare;
        }
    }
}
