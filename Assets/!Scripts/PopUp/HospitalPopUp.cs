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

        _building = _controllersManager.BuildingController.GetHospitalBuilding();
        
        SetButtonState(giveMedicineBtnParent, true);
        UpdateMedicineTimerText();

        _controllersManager.TimeController.OnNextDayEvent
            .Subscribe(_ => OnNextDayEvent())
            .AddTo(this);
        
        _controllersManager.PeopleUnitsController.OnUnitInjuredByPeopleUnitController
            .Subscribe(_ => CreateHealGOPrefabIfNeeded())
            .AddTo(this);
        
        _controllersManager.PeopleUnitsController.OnUnitHealedByPeopleUnitController
            .Subscribe(_ => UpdateHealUnitGOButtonState())
            .AddTo(this);
    }

    private void CreateHealGOPrefabIfNeeded()
    {
        if (_healUnitBtn == null) 
        {
            _healUnitBtn = Instantiate(healGOPrefab, healGOParent);
            _healUnitBtn.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(HealInjuredUnit);
            SetButtonState(_healUnitBtn, true);
        }
    }

    private void OnNextDayEvent()
    {
        if (_controllersManager.BuildingController.GetHospitalBuilding().MedicineWasGiven())
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
            _controllersManager.BuildingController.GetHospitalBuilding().SendPeopleToGiveMedicine();
        }
    }

    private bool CanGiveAwayMedicine()
    {
        return HasEnoughPeople(_controllersManager.BuildingController.GetHospitalBuilding().PeopleToGiveMedicine) &&
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
            _controllersManager.BuildingController.GetHospitalBuilding().DaysToGiveMedicine.ToString() + " дн.";
    }

    private bool EnoughMedicineToGiveAway()
    {
        return HasEnoughResources(ResourceModel.ResourceType.Medicine,
            _controllersManager.BuildingController.GetHospitalBuilding().MedicineToGive);
    }
    
    private void UpdateHealUnitGOButtonState()
    {
        if (_controllersManager.PeopleUnitsController.InjuredUnits.Count == 0 && _healUnitBtn)
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
