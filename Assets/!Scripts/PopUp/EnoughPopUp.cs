using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class EnoughPopUp : InfoPopUp
{
    [FormerlySerializedAs("_errorText")] [SerializeField] protected TextMeshProUGUI errorText;

    public virtual void Init()
    {
        HideError();
    }

    protected bool HasEnoughPeople(int requiredPeople)
    {
        if (_controllersManager.PeopleUnitsController.ReadyUnits.Count >= requiredPeople)
            return true;

        ShowError("Недостаточно подразделений");
        return false;
    }

    protected bool HasEnoughResources(ResourceModel.ResourceType resourceType, int requiredAmount)
    {
        if (_resourceViewModel.GetResourceValue(resourceType) >= requiredAmount)
            return true;

        ShowError("Недостаточно ресурсов");
        return false;
    }

    protected bool CanUseActionPoint()
    {
        if (_controllersManager.TimeController.OnActionPointUsed())
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
