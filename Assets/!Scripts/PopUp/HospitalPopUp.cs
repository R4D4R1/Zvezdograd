using DG.Tweening;
using TMPro;
using UnityEngine;

public class HospitalPopUp : QuestPopUp
{
    [SerializeField] private TextMeshProUGUI _medicineTimerText;

    protected override void Start()
    {
        base.Start();

        _errorText.enabled = false;
        SetButtonState(true);
        UpdateMedicineTimerText();
        _controllersManager.TimeController.OnNextDayEvent += OnNextDayEvent;
    }

    private void OnNextDayEvent()
    {
        if (_controllersManager.BuildingController.GetHospitalBuilding().MedicineWasGiven())
        {
            activeBtn.SetActive(true);
            inactiveBtn.SetActive(false);
        }

        UpdateMedicineTimerText();
    }

    public void ShowHospitalPopUp()
    {
        transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            IsActive = true;
            SetAlpha(1);
        });
    }

    public void GiveAwayMedicine()
    {
        if (CanGiveAwayMedicine())
        {
            if (!CanUseActionPoint())
                return;
            SetButtonState(false);
            _controllersManager.BuildingController.GetHospitalBuilding().SendPeopleToGiveMedicine();
        }
        else
        {
            ShowErrorMessage();
        }
    }

    private bool CanGiveAwayMedicine()
    {
        return CheckForEnoughPeople(_controllersManager.BuildingController.GetHospitalBuilding().PeopleToGiveMedicine) &&
               EnoughMedicineToGiveAway();
    }

    private void ShowErrorMessage()
    {
        if (!EnoughMedicineToGiveAway())
        {
            _errorText.text = "НЕ ДОСТАТОЧНО МЕДИЦИНЫ";
        }
        else if (!CheckForEnoughPeople(_controllersManager.BuildingController.GetHospitalBuilding().PeopleToGiveMedicine))
        {
            _errorText.text = "НЕ ДОСТАТОЧНО ЛЮДЕЙ";
        }
        _errorText.enabled = true;
    }

    private void UpdateMedicineTimerText()
    {
        _medicineTimerText.text = "Крайний срок отправки мед. помощи - " +
            _controllersManager.BuildingController.GetHospitalBuilding().DaysToGiveMedicine.ToString() + " дн.";
    }

    private bool EnoughMedicineToGiveAway()
    {
        return ChechIfEnoughResourcesByType(ResourceModel.ResourceType.Medicine, _controllersManager.BuildingController.GetHospitalBuilding().MedicineToGive);
    }

    
}
