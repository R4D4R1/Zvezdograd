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
        if (_controllersManager.BombBuildingController.GetFoodTruckBuilding().FoodWasGivenAwayToday())
        {
            SetButtonState(giveFoodBtnParent,true);
        }
    }

    public void GiveAwayProvision()
    {
        if (CanGiveAwayProvision())
        {

            SetButtonState(giveFoodBtnParent,false);
            _controllersManager.BombBuildingController.GetFoodTruckBuilding().SendPeopleToGiveProvision();
        }
    }

    private bool CanGiveAwayProvision()
    {
        return HasEnoughPeople(_controllersManager.BombBuildingController.GetFoodTruckBuilding().PeopleToGiveProvision) &&
               HasEnoughResources(ResourceModel.ResourceType.Provision,
               _controllersManager.BombBuildingController.GetFoodTruckBuilding().FoodToGive)
               && CanUseActionPoint();
    }
}
