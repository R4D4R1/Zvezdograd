using TMPro;
using UnityEngine;

public class EnoughPopUp : InfoPopUp
{
    [SerializeField] protected TextMeshProUGUI _errorText;

    public virtual void Init()
    {
        HideError();
    }

    protected bool HasEnoughPeople(int requiredPeople)
    {
        if (_controllersManager.PeopleUnitsController.ReadyUnits.Count >= requiredPeople)
            return true;

        ShowError("Не достаточно подразделений");
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
        _errorText.text = message;
        _errorText.enabled = true;
    }

    private void HideError()
    {
        _errorText.enabled = false;
    }

    public override void HidePopUp()
    {
        base.HidePopUp();
        HideError();
    }
}
