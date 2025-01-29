using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class EnoughPopUp : InfoPopUp
{
    [SerializeField] protected TextMeshProUGUI _errorText;

    public bool CheckForEnoughPeople(int peopleToDoSmth)
    {
        if (ControllersManager.Instance.peopleUnitsController.GetReadyUnits() >= peopleToDoSmth)
            return true;
        else
            return false;
    }

    public override void HidePopUp()
    {
        base.HidePopUp();
        _errorText.enabled = false;
    }
}
