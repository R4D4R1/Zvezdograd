using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class EnoughPeoplePopUp : InfoPopUp
{
    [SerializeField] protected TextMeshProUGUI _errorText;

    public bool EnoughPeopleTo(int peopleToDoSmth)
    {
        if (ControllersManager.Instance.peopleUnitsController.GetReadyUnits() >= peopleToDoSmth)
            return true;
        else
            return false;
    }
}
