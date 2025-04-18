using UnityEngine;
using UniRx;

public class FoodTrucksPopUp : QuestPopUp
{
    [SerializeField] private GameObject giveFoodBtnParent;
    
    private FoodTrucksBuilding _building;
    
    public override void Init()
    {
        base.Init();

        _building = BuildingsController.GetFoodTruckBuilding();
        
        SetButtonState(giveFoodBtnParent,true);

        TimeController.OnNextDayEvent
            .Subscribe(_ => OnNextDayEvent())
            .AddTo(this);
        
        EventController.OnQuestTriggered
            .Subscribe(popupEvent =>
            {
                if (popupEvent.buildingType == _building.Type.ToString())
                {
                    EnableQuest(
                        popupEvent.buildingType, popupEvent.questText, popupEvent.deadlineInDays, popupEvent.unitSize,
                        popupEvent.turnsToWork, popupEvent.turnsToRest, popupEvent.materialsToGet, popupEvent.materialsToLose,
                        popupEvent.stabilityToGet, popupEvent.stabilityToLose, popupEvent.relationshipWithGovToGet, popupEvent.relationshipWithGovToLose);
                }
            })
            .AddTo(this);
    }

    private void OnNextDayEvent()
    {
        if (BuildingsController.GetFoodTruckBuilding().FoodWasGivenAwayToday())
        {
            SetButtonState(giveFoodBtnParent,true);
        }
    }

    public void GiveAwayProvision()
    {
        if (CanGiveAwayProvision())
        {
            SetButtonState(giveFoodBtnParent,false);
            BuildingsController.GetFoodTruckBuilding().SendPeopleToGiveProvision();
        }
    }

    private bool CanGiveAwayProvision()
    {
        return 
                HasEnoughPeople(BuildingsController.GetFoodTruckBuilding().FoodTrucksBuildingConfig.PeopleToGiveProvision) &&
                HasEnoughResources(ResourceModel.ResourceType.Provision, 
                    BuildingsController.GetFoodTruckBuilding().FoodTrucksBuildingConfig.FoodToGive) &&
                CanUseActionPoint();
    }

    public override void LoadData(PopUpSaveData data)
    {
        base.LoadData(data);

        SetButtonState(giveFoodBtnParent, IsBtnActive);
    }
}
