using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class QuestPopUp : EnoughPopUp
{
    [SerializeField] private GameObject questObjectPrefab;
    [SerializeField] private Transform questGroupTransform;

     private readonly Dictionary<GameObject, QuestData> _questDeadlines = new();

     private struct QuestData
     {
         public readonly string QuestName;
         public int DeadlineInDays;
         public readonly int StabilityToLose;
         public readonly int RelationshipWithGovToLose;
         
         public QuestData(string questName, int deadlineInDays, int stabilityToLose, int relationshipWithGovToLose)
         {
             QuestName = questName;
             DeadlineInDays = deadlineInDays;
             StabilityToLose = stabilityToLose;
             RelationshipWithGovToLose = relationshipWithGovToLose;
         }
     }

     public override void Init()
     {
         TimeController.OnNextDayEvent
             .Subscribe(_ => OnNextDay())
             .AddTo(this);
     }

     private void OnNextDay()
     {
         foreach (var quest in _questDeadlines.Keys.ToList())
         {
             var data = _questDeadlines[quest];
             data.DeadlineInDays--;

             if (data.DeadlineInDays <= 0)
             {
                 LoseQuest(quest, data.StabilityToLose, data.RelationshipWithGovToLose);
             }
             else
             {
                 _questDeadlines[quest] = data;
                 quest.GetComponent<TextMeshProUGUI>().text = $"{data.QuestName}  {data.DeadlineInDays} дн.";
             }
         }
     }

     protected void EnableQuest(string buildingType, string questName, int deadlineInDays, int unitSize,
         int turnsToWork, int turnsToRest,
         int materialsToGet, int materialsToLose, int stabilityToGet, int stabilityToLose,
         int relationshipWithGovToGet, int relationshipWithGovToLose)
     {
         var quest = Instantiate(questObjectPrefab, questGroupTransform);
         
         if (!quest)
         {
             return;
         }

         quest.SetActive(true);
         quest.GetComponent<TextMeshProUGUI>().text = $"{questName}  {deadlineInDays} дн.";

         var completeButton = quest.GetComponentInChildren<Button>();
         if (completeButton)
         {
             completeButton.onClick.RemoveAllListeners();
             completeButton.onClick.AddListener(() =>
             {
                 if (!HasEnoughPeople(unitSize))
                 {
                     return;
                 }

                 if (!CanUseActionPoint())
                 {
                     return;
                 }

                 if (ProcessResources(buildingType, materialsToGet, materialsToLose))
                 {
                     PeopleUnitsController.AssignUnitsToTask(unitSize, turnsToWork, turnsToRest);
                     CompleteQuest(quest, stabilityToGet, relationshipWithGovToGet);
                 }
             });
         }

         _questDeadlines[quest] = new QuestData(questName,deadlineInDays, stabilityToLose, relationshipWithGovToLose);
     }


     private void CompleteQuest(GameObject quest, int stabilityToGet, int relationshipWithGovToGet)
     {
         ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Stability, stabilityToGet));
         BuildingController.GetCityHallBuilding().ModifyRelationWithGov(relationshipWithGovToGet);
         
         quest.SetActive(false);
         _questDeadlines.Remove(quest);
     }

     private void LoseQuest(GameObject quest, int stabilityToLose, int relationshipWithGovToLose)
     {
         ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Stability, -stabilityToLose));
         BuildingController.GetCityHallBuilding().ModifyRelationWithGov(-relationshipWithGovToLose);
         
         quest.SetActive(false);
         _questDeadlines.Remove(quest);
     }

     private bool ProcessResources(string buildingType, int materialsToGet, int materialsToLose)
     {
         ResourceModel.ResourceType resourceType;

         if (buildingType == "FoodTrucks")
             resourceType = ResourceModel.ResourceType.Provision;
         else if (buildingType == "Hospital")
             resourceType = ResourceModel.ResourceType.Medicine;
         else if (buildingType == "CityHall")
             resourceType = ResourceModel.ResourceType.ReadyMaterials;
         else
             return false;

         if (!HasEnoughResources(resourceType, materialsToLose)) return false;
         if (!HasEnoughSpaceForResources(resourceType, materialsToGet)) return false;
         
         ResourceViewModel.ModifyResourceCommand.Execute((resourceType, materialsToGet));
         ResourceViewModel.ModifyResourceCommand.Execute((resourceType, -materialsToLose));
         return true;
     }

    
    protected void SetButtonState(GameObject btnsParent, bool isActive)
    {
        btnsParent.transform.GetChild(0).gameObject.SetActive(isActive);
        btnsParent.transform.GetChild(1).gameObject.SetActive(!isActive);
    }
}
