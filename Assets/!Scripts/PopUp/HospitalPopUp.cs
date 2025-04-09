using TMPro;
using UnityEngine;
using UniRx;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HospitalPopUp : QuestPopUp
{
    [FormerlySerializedAs("_medicineTimerText")] [SerializeField] private TextMeshProUGUI medicineTimerText;
    [SerializeField] private GameObject giveMedicineBtnParent;
    [SerializeField] private Transform healGOParent;
    [SerializeField] private GameObject _healUnitBtn;
    
    private HospitalBuilding _building;

    private bool _isHealing;
    
    public override void Init()
    {
        base.Init();

        _building = BuildingController.GetHospitalBuilding();
        
        SetButtonState(giveMedicineBtnParent, true);
        UpdateAllText();

        TimeController.OnNextDayEvent
            .Subscribe(_ => OnNextDayEvent())
            .AddTo(this);
        
        PeopleUnitsController.OnUnitInjuredByPeopleUnitController
            .Subscribe(_ => CreateHealGOPrefabIfNeeded())
            .AddTo(this);
        
        PeopleUnitsController.OnUnitHealedByPeopleUnitController
            .Subscribe(_ => UpdateHealUnitGOButtonState())
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

        _isHealing = false;
    }

    private void CreateHealGOPrefabIfNeeded()
    {
        if (!_healUnitBtn.activeSelf)
        {
            _healUnitBtn.SetActive(true);
            _healUnitBtn.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(HealInjuredUnit);
            SetHealButtonState(_healUnitBtn, true);
        }
    }

    private void OnNextDayEvent()
    {
        if (BuildingController.GetHospitalBuilding().MedicineWasGiven())
        {
            SetButtonState(giveMedicineBtnParent, true);
        }

        UpdateAllText();
    }

    public void GiveAwayMedicine()
    {
        if (!CanGiveAwayMedicine()) return;
        SetButtonState(giveMedicineBtnParent, false);
        BuildingController.GetHospitalBuilding().SendPeopleToGiveMedicine();
    }

    private bool CanGiveAwayMedicine()
    {
        return HasEnoughPeople(BuildingController.GetHospitalBuilding().HospitalBuildingConfig.PeopleToGiveMedicine) &&
               HasEnoughResources(ResourceModel.ResourceType.Medicine,
                   BuildingController.GetHospitalBuilding().HospitalBuildingConfig.MedicineToGive) &&
               CanUseActionPoint();
    }

    private void HealInjuredUnit()
    {
        if (!CanHealInjuredUnit()) return;
        _building.InjuredUnitStartedHealing();
        SetHealButtonState(_healUnitBtn, false);
    }

    private bool CanHealInjuredUnit()
    {
        return HasEnoughResources(ResourceModel.ResourceType.Medicine,
                   _building.HospitalBuildingConfig.MedicineToHealInjuredUnit) &&
               CanUseActionPoint();
    }

    protected override void UpdateAllText()
    {
        medicineTimerText.text = "Осталось времени до раздачи - " +
                                 BuildingController.GetHospitalBuilding().DaysToGiveMedicine + " дн.";
    }

    private void SetHealButtonState(GameObject btnsParent, bool activeState)
    {
        _isHealing = activeState;
        btnsParent.transform.GetChild(0).gameObject.SetActive(activeState);
        btnsParent.transform.GetChild(1).gameObject.SetActive(!activeState);
    }
    
    private void UpdateHealUnitGOButtonState()
    {
        if (PeopleUnitsController.InjuredUnits.Count == 0 && _healUnitBtn)
        {
            _healUnitBtn.SetActive(false);
            _healUnitBtn = null;
        }
        else if (_healUnitBtn)
        {
            SetButtonState(_healUnitBtn, true);
        }
    }
    public override PopUpSaveData GetSaveData()
    {
        return new HospitalQuestPopUpSaveData()
        {
            popUpId = PopUpId,
            isBtnActive = IsBtnActive,
            IsHealing = _isHealing
        };
    }
    
    public override void LoadFromSaveData(PopUpSaveData data)
    {
        var save = data as HospitalQuestPopUpSaveData;
        if (save == null) return;
        
        IsBtnActive = save.isBtnActive;
        _isHealing = save.IsHealing;
        
        SetButtonState(giveMedicineBtnParent,IsBtnActive);

        if (PeopleUnitsController.InjuredUnits.Count > 0)
        {
            _healUnitBtn.SetActive(true);
            SetHealButtonState(_healUnitBtn,_isHealing);
        }
        
        UpdateAllText();
    }
}
