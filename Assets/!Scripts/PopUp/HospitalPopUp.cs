using TMPro;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

public class HospitalPopUp : QuestPopUp
{
    [SerializeField] private TextMeshProUGUI medicineTimerText;
    [SerializeField] private GameObject giveMedicineBtnParent;
    [SerializeField] private Transform healGOParent;
    [SerializeField] private GameObject healInjuredUnitBtn;
    
    private HospitalBuilding _hospitalBuilding;
    private bool _isHealing;

    public readonly Subject<SelectableBuilding> OnBuildingHighlighted = new();

    public override void Init()
    {
        base.Init();

        _hospitalBuilding = _buildingsController.GetHospitalBuilding();
        
        SetButtonState(giveMedicineBtnParent, true);
        UpdateAllText();

        _timeController.OnNextDayEvent
            .Subscribe(_ => OnNextDayEvent())
            .AddTo(this);
        
        _peopleUnitsController.OnUnitInjuredByPeopleUnitController
            .Subscribe(_ => CreateHealGOPrefabIfNeeded())
            .AddTo(this);
        
        _peopleUnitsController.OnUnitHealedByPeopleUnitController
            .Subscribe(_ => UpdateHealUnitGOButtonState())
            .AddTo(this);

        _eventController.OnQuestTriggered
            .Subscribe(popupEvent =>
            {
                if (popupEvent.buildingType == _hospitalBuilding.Type.ToString())
                {
                    EnableQuest(
                        popupEvent.buildingType, popupEvent.questText, popupEvent.deadlineInDays, popupEvent.unitSize,
                        popupEvent.turnsToWork, popupEvent.turnsToRest, popupEvent.materialsToGet, popupEvent.materialsToLose,
                        popupEvent.stabilityToGet, popupEvent.stabilityToLose, popupEvent.relationshipWithGovToGet, popupEvent.relationshipWithGovToLose);

                    OnBuildingHighlighted.OnNext(_hospitalBuilding);
                }
            })
            .AddTo(this);

        _isHealing = false;
    }

    private void CreateHealGOPrefabIfNeeded()
    {
        if (!healInjuredUnitBtn.activeSelf)
        {
            healInjuredUnitBtn.SetActive(true);
            healInjuredUnitBtn.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(HealInjuredUnit);
            SetHealButtonState(healInjuredUnitBtn, true);
        }
    }

    private void OnNextDayEvent()
    {
        if (_buildingsController.GetHospitalBuilding().MedicineWasGiven())
        {
            SetButtonState(giveMedicineBtnParent, true);
        }

        UpdateAllText();
    }

    public void GiveAwayMedicine()
    {
        if (!CanGiveAwayMedicine()) return;
        SetButtonState(giveMedicineBtnParent, false);
        _buildingsController.GetHospitalBuilding().SendPeopleToGiveMedicine();
    }

    private bool CanGiveAwayMedicine()
    {
        return HasEnoughPeople(_buildingsController.GetHospitalBuilding().HospitalBuildingConfig.PeopleToGiveMedicine) &&
               HasEnoughResources(ResourceModel.ResourceType.Medicine,
                   _buildingsController.GetHospitalBuilding().HospitalBuildingConfig.MedicineToGive) &&
               CanUseActionPoint();
    }

    private void HealInjuredUnit()
    {
        if (!CanHealInjuredUnit()) return;
        _hospitalBuilding.InjuredUnitStartedHealing();
        SetHealButtonState(healInjuredUnitBtn, false);
    }

    private bool CanHealInjuredUnit()
    {
        return HasEnoughResources(ResourceModel.ResourceType.Medicine,
                   _hospitalBuilding.HospitalBuildingConfig.MedicineToHealInjuredUnit) &&
               CanUseActionPoint();
    }

    protected override void UpdateAllText()
    {
        medicineTimerText.text = "Осталось времени до раздачи - " +
                                 _buildingsController.GetHospitalBuilding().DaysToGiveMedicine + " дн.";
    }

    private void SetHealButtonState(GameObject btnsParent, bool activeState)
    {
        _isHealing = activeState;
        btnsParent.transform.GetChild(0).gameObject.SetActive(activeState);
        btnsParent.transform.GetChild(1).gameObject.SetActive(!activeState);
    }
    
    private void UpdateHealUnitGOButtonState()
    {
        if (_peopleUnitsController.InjuredUnits.Count == 0 && healInjuredUnitBtn)
        {
            healInjuredUnitBtn.SetActive(false);
            healInjuredUnitBtn = null;
        }
        else if (healInjuredUnitBtn)
        {
            SetButtonState(healInjuredUnitBtn, true);
        }
    }
    public override PopUpSaveData SaveData()
    {
        return new HospitalQuestPopUpSaveData()
        {
            popUpID = PopUpID,
            isBtnActive = IsBtnActive,
            IsHealing = _isHealing
        };
    }
    
    public override void LoadData(PopUpSaveData data)
    {
        var save = data as HospitalQuestPopUpSaveData;
        if (save == null) return;
        
        IsBtnActive = save.isBtnActive;
        _isHealing = save.IsHealing;
        
        SetButtonState(giveMedicineBtnParent,IsBtnActive);

        if (_peopleUnitsController.InjuredUnits.Count > 0)
        {
            healInjuredUnitBtn.SetActive(true);
            SetHealButtonState(healInjuredUnitBtn,_isHealing);
        }
        
        UpdateAllText();
    }
}
