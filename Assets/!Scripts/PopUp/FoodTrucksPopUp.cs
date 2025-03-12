using DG.Tweening;
using UnityEngine;

public class FoodTrucksPopUp : QuestPopUp
{
    protected override void Start()
    {
        base.Start();

        _errorText.enabled = false;
        SetButtonState(true);
        _controllersManager.TimeController.OnNextDayEvent += OnNextDayEvent;
    }

    public void ShowFoodTruckPopUp()
    {
        transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            IsActive = true;
            SetAlpha(1);
        });
    }

    private void OnNextDayEvent()
    {
        // включаем на следующий день если раздали еду
        if (_controllersManager.BuildingController.GetFoodTruckBuilding().FoodWasGivenAwayToday())
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
            _controllersManager.BuildingController.GetFoodTruckBuilding().SendPeopleToGiveProvision();
        }
        else
        {
            ShowErrorMessage();
        }
    }

    private bool CanGiveAwayProvision()
    {
        return CheckForEnoughPeople(_controllersManager.BuildingController.GetFoodTruckBuilding().PeopleToGiveProvision) &&
               EnoughProvisionToGiveAway();
    }

    private void ShowErrorMessage()
    {
        if (!EnoughProvisionToGiveAway())
        {
            _errorText.text = "НЕТ ПРОВИЗИИ";
        }
        else if (!CheckForEnoughPeople(_controllersManager.BuildingController.GetFoodTruckBuilding().PeopleToGiveProvision))
        {
            _errorText.text = "НЕ ДОСТАТОЧНО ЛЮДЕЙ";
        }
        _errorText.enabled = true;
    }

    public bool EnoughProvisionToGiveAway()
    {
        return ChechIfEnoughResourcesByType(ResourceModel.ResourceType.Provision, _controllersManager.BuildingController.GetFoodTruckBuilding().FoodToGive);
    }
}
