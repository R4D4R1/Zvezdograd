using UnityEngine;
using UniRx;

public class FoodTrucksPopUp : QuestPopUp
{
    [SerializeField] private GameObject giveFoodBtnParent;
    
    private FoodTrucksBuilding _foodTrucksBuilding;

    public readonly Subject<SelectableBuilding> OnBuildingHighlighted = new();

    public override void Init()
    {
        base.Init();

        _foodTrucksBuilding = _buildingsController.GetFoodTruckBuilding();
        
        SetButtonState(giveFoodBtnParent,true);

        _timeController.OnNextDayEvent
            .Subscribe(_ => OnNextDayEvent())
            .AddTo(this);
        
        _eventController.OnQuestTriggered
            .Subscribe(popupEvent =>
            {
                if (popupEvent.buildingType == _foodTrucksBuilding.Type.ToString())
                {
                    EnableQuest(
                        popupEvent.buildingType, popupEvent.questText, popupEvent.deadlineInDays, popupEvent.unitSize,
                        popupEvent.turnsToWork, popupEvent.turnsToRest, popupEvent.materialsToGet, popupEvent.materialsToLose,
                        popupEvent.stabilityToGet, popupEvent.stabilityToLose, popupEvent.relationshipWithGovToGet, popupEvent.relationshipWithGovToLose);

                    OnBuildingHighlighted.OnNext(_foodTrucksBuilding);
                }
            })
            .AddTo(this);
    }

    private void OnNextDayEvent()
    {
        if (_buildingsController.GetFoodTruckBuilding().FoodWasGivenAwayToday())
        {
            SetButtonState(giveFoodBtnParent,true);
        }
    }

    public void GiveAwayProvision()
    {
        if (CanGiveAwayProvision())
        {
            SetButtonState(giveFoodBtnParent,false);
            _buildingsController.GetFoodTruckBuilding().SendPeopleToGiveProvision();
        }
    }

    private bool CanGiveAwayProvision()
    {
        return 
                HasEnoughPeople(_buildingsController.GetFoodTruckBuilding().FoodTrucksBuildingConfig.PeopleToGiveProvision) &&
                HasEnoughResources(ResourceModel.ResourceType.Provision, 
                    _buildingsController.GetFoodTruckBuilding().FoodTrucksBuildingConfig.FoodToGive) &&
                CanUseActionPoint();
    }

    public override void LoadData(PopUpSaveData data)
    {
        base.LoadData(data);

        SetButtonState(giveFoodBtnParent, IsBtnActive);
    }
}
