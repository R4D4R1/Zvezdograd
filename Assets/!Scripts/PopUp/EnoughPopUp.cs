using DG.Tweening;
using TMPro;
using UnityEngine;

public class EnoughPopUp : InfoPopUp
{
    [SerializeField] protected TextMeshProUGUI _errorText;

    public virtual void Init()
    {
        HideError();
    }

    public bool HasEnoughPeople(int requiredPeople)
    {
        if (_controllersManager.PeopleUnitsController.ReadyUnits.Count >= requiredPeople)
            return true;

        ShowError("ме днярюрнвмн кчдеи");
        return false;
    }

    public bool HasEnoughResources(ResourceModel.ResourceType resourceType, int requiredAmount)
    {
        if (_resourceViewModel.GetResourceValue(resourceType) >= requiredAmount)
            return true;

        ShowError("меднярюрнвмн пеяспянб");
        return false;
    }

    protected bool CanUseActionPoint()
    {
        if (_controllersManager.TimeController.OnActionPointUsed())
            return true;

        ShowError("ме днярюрнвмн нвйнб деиярбхъ");
        return false;
    }

    public void ShowError(string message)
    {
        _errorText.text = message;
        _errorText.enabled = true;
    }

    public void HideError()
    {
        _errorText.enabled = false;
    }

    public override void HidePopUp()
    {
        base.HidePopUp();
        HideError();
    }
}
