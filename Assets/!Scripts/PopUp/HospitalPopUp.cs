using TMPro;
using UnityEngine;
using UniRx;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HospitalPopUp : QuestPopUp
{
    [FormerlySerializedAs("_medicineTimerText")] [SerializeField] private TextMeshProUGUI medicineTimerText;
    [SerializeField] private GameObject giveMedicineBtnParent;
    [SerializeField] private GameObject healGOPrefab;
    [SerializeField] private Transform healGOParent;

    private GameObject _healUnitBtn;
    private HospitalBuilding _building;
    
    public override void Init()
    {
        base.Init();

        _building = BuildingController.GetHospitalBuilding();
        
        SetButtonState(giveMedicineBtnParent, true);
        UpdateMedicineTimerText();

        TimeController.OnNextDayEvent
            .Subscribe(_ => OnNextDayEvent())
            .AddTo(this);
        
        PeopleUnitsController.OnUnitInjuredByPeopleUnitController
            .Subscribe(_ => CreateHealGOPrefabIfNeeded())
            .AddTo(this);
        
        PeopleUnitsController.OnUnitHealedByPeopleUnitController
            .Subscribe(_ => UpdateHealUnitGOButtonState())
            .AddTo(this);
    }

    private void CreateHealGOPrefabIfNeeded()
    {
        if (!_healUnitBtn) 
        {
            _healUnitBtn = Instantiate(healGOPrefab, healGOParent);
            _healUnitBtn.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(HealInjuredUnit);
            SetButtonState(_healUnitBtn, true);
        }
    }

    private void OnNextDayEvent()
    {
        if (BuildingController.GetHospitalBuilding().MedicineWasGiven())
        {
            SetButtonState(giveMedicineBtnParent, true);
        }

        UpdateMedicineTimerText();
    }

    public void GiveAwayMedicine()
    {
        if (CanGiveAwayMedicine())
        {
            SetButtonState(giveMedicineBtnParent, false);
            BuildingController.GetHospitalBuilding().SendPeopleToGiveMedicine();
        }
    }

    private bool CanGiveAwayMedicine()
    {
        return HasEnoughPeople(BuildingController.GetHospitalBuilding().PeopleToGiveMedicine) &&
               EnoughMedicineToGiveAway() &&
               CanUseActionPoint();
    }

    private void HealInjuredUnit()
    {
        if (CanHealInjuredUnit())
        {
            _building.InjuredUnitStartedHealing();
            SetButtonState(_healUnitBtn, false);
        }
    }

    private bool CanHealInjuredUnit()
    {
        return HasEnoughResources(ResourceModel.ResourceType.Medicine,
                   _building.MedicineToHealInjuredUnit) &&
               CanUseActionPoint();
    }

    private void UpdateMedicineTimerText()
    {
        medicineTimerText.text = "Осталось времени до раздачи - " +
            BuildingController.GetHospitalBuilding().DaysToGiveMedicine.ToString() + " дн.";
    }

    private bool EnoughMedicineToGiveAway()
    {
        return HasEnoughResources(ResourceModel.ResourceType.Medicine,
            BuildingController.GetHospitalBuilding().MedicineToGive);
    }
    
    private void UpdateHealUnitGOButtonState()
    {
        if (PeopleUnitsController.InjuredUnits.Count == 0 && _healUnitBtn)
        {
            Destroy(_healUnitBtn);
            _healUnitBtn = null;
        }
        else if (_healUnitBtn)
        {
            SetButtonState(_healUnitBtn, true);
        }
    }
}
