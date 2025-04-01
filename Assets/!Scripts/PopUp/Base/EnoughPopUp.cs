using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class EnoughPopUp : FullScreenPopUp
{
    [FormerlySerializedAs("_errorText")] [SerializeField] protected TextMeshProUGUI errorText;

    public virtual void Init()
    {
        HideError();
    }

    protected bool HasEnoughPeople(int requiredPeople)
    {
        if (PeopleUnitsController.ReadyUnits.Count >= requiredPeople)
            return true;

        ShowError("Недостаточно подразделений");
        return false;
    }

    protected bool HasEnoughResources(ResourceModel.ResourceType resourceType, int requiredAmount)
    {
        if (ResourceViewModel.GetResourceValue(resourceType) >= requiredAmount)
            return true;

        ShowError("Недостаточно ресурсов");
        return false;
    }
    
    protected bool HasEnoughSpaceForResources(ResourceModel.ResourceType resourceType, int materialsToGet)
    {
        if (ResourceViewModel.GetResourceValue(resourceType) + materialsToGet <= ResourceViewModel.GetMaxResourceValue(resourceType))
            return true;
        
        ShowError("Недостаточно места для ресурсов");
        return false;
    }

    protected bool CanUseActionPoint()
    {
        if (TimeController.OnActionPointUsed())
            return true;

        ShowError("Недостаточно очков действий");
        return false;
    }

    protected void ShowError(string message)
    {
        errorText.text = message;
        errorText.enabled = true;
    }

    private void HideError()
    {
        errorText.enabled = false;
    }

    public override void HidePopUp()
    {
        base.HidePopUp();
        HideError();
    }
}
