using UnityEngine;
using UniRx;

public class FoodTrucksPopUp : QuestPopUp
{
    [SerializeField] private GameObject giveFoodBtnParent;
    public override void Init()
    {
        base.Init();

        SetButtonState(giveFoodBtnParent,true);

        _controllersManager.TimeController.OnNextDayEvent
            .Subscribe(_ => OnNextDayEvent())
            .AddTo(this);
    }

    private void OnNextDayEvent()
    {
        if (_controllersManager.BuildingController.GetFoodTruckBuilding().FoodWasGivenAwayToday())
        {
            SetButtonState(giveFoodBtnParent,true);
        }
    }

    public void GiveAwayProvision()
    {
        if (CanGiveAwayProvision())
        {

            SetButtonState(giveFoodBtnParent,false);
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
