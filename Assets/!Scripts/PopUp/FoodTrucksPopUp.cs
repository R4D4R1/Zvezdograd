using DG.Tweening;
using UnityEngine;

public class FoodTrucksPopUp : QuestPopUp
{
    protected override void Start()
    {
        base.Start();

        _errorText.enabled = false;
        _isDestroyable = false;
        SetButtonState(true);
        ControllersManager.Instance.timeController.OnNextDayEvent += OnNextDayEvent;
    }

    public void ShowFoodTruckPopUp()
    {
        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            IsActive = true;
            SetAlpha(1);
        });
    }

    private void OnNextDayEvent()
    {
        // включаем на следующий день если раздали еду
        if (ControllersManager.Instance.buildingController.GetFoodTruckBuilding().FoodWasGivenAwayToday())
        {
            activeBtn.SetActive(true);
            inactiveBtn.SetActive(false);
        }
    }

    public void GiveAwayProvision()
    {
        if (CanGiveAwayProvision())
        {
            SetButtonState(false);
            ControllersManager.Instance.buildingController.GetFoodTruckBuilding().SendPeopleToGiveProvision();
        }
        else
        {
            ShowErrorMessage();
        }
    }

    private bool CanGiveAwayProvision()
    {
        return CheckForEnoughPeople(ControllersManager.Instance.buildingController.GetFoodTruckBuilding().PeopleToGiveProvision) &&
               EnoughProvisionToGiveAway();
    }

    private void ShowErrorMessage()
    {
        if (!EnoughProvisionToGiveAway())
        {
            _errorText.text = "НЕТ ПРОВИЗИИ";
        }
        else if (!CheckForEnoughPeople(ControllersManager.Instance.buildingController.GetFoodTruckBuilding().PeopleToGiveProvision))
        {
            _errorText.text = "НЕ ДОСТАТОЧНО ЛЮДЕЙ";
        }
        _errorText.enabled = true;
    }

    public bool EnoughProvisionToGiveAway()
    {
        return ChechIfEnoughResourcesByType(ResourceController.ResourceType.Provision, ControllersManager.Instance.buildingController.GetFoodTruckBuilding().FoodToGive);
    }
}
