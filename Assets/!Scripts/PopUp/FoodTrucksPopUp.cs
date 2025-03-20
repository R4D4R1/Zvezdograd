using DG.Tweening;
using UnityEngine;
using UniRx;

public class FoodTrucksPopUp : QuestPopUp
{
    public override void Init()
    {
        base.Init();

        SetButtonState(true);

        _controllersManager.TimeController.OnNextDayEvent
            .Subscribe(_ => OnNextDayEvent())
            .AddTo(this);
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
    }

    private bool CanGiveAwayProvision()
    {
        return HasEnoughPeople(_controllersManager.BuildingController.GetFoodTruckBuilding().PeopleToGiveProvision) &&
               HasEnoughResources(ResourceModel.ResourceType.Provision,
               _controllersManager.BuildingController.GetFoodTruckBuilding().FoodToGive)
               && CanUseActionPoint();
    }
}
