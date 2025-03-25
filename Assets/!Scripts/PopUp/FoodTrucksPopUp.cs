using UnityEngine;
using UniRx;

public class FoodTrucksPopUp : QuestPopUp
{
    [SerializeField] private GameObject giveFoodBtnParent;
    public override void Init()
    {
        base.Init();

        SetButtonState(giveFoodBtnParent,true);

        TimeController.OnNextDayEvent
            .Subscribe(_ => OnNextDayEvent())
            .AddTo(this);
    }

    private void OnNextDayEvent()
    {
        if (BuildingController.GetFoodTruckBuilding().FoodWasGivenAwayToday())
        {
            SetButtonState(giveFoodBtnParent,true);
        }
    }

    public void GiveAwayProvision()
    {
        if (CanGiveAwayProvision())
        {

            SetButtonState(giveFoodBtnParent,false);
            BuildingController.GetFoodTruckBuilding().SendPeopleToGiveProvision();
        }
    }

    private bool CanGiveAwayProvision()
    {
        return HasEnoughPeople(BuildingController.GetFoodTruckBuilding().PeopleToGiveProvision) &&
               HasEnoughResources(ResourceModel.ResourceType.Provision,
               BuildingController.GetFoodTruckBuilding().FoodToGive)
               && CanUseActionPoint();
    }
}
