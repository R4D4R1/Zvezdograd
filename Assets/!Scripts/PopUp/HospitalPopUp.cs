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
        _isDestroyable = false;
        SetButtonState(true);
        UpdateMedicineTimerText();
        ControllersManager.Instance.timeController.OnNextDayEvent += OnNextDayEvent;
    }

    private void OnNextDayEvent()
    {
        if (ControllersManager.Instance.buildingController.GetHospitalBuilding().MedicineWasGiven())
        {
            activeBtn.SetActive(true);
            inactiveBtn.SetActive(false);
        }

        UpdateMedicineTimerText();
    }

    public void ShowHospitalPopUp()
    {
        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            IsActive = true;
            SetAlpha(1);
        });
    }

    public void GiveAwayMedicine()
    {
        if (CanGiveAwayMedicine())
        {
            SetButtonState(false);
            ControllersManager.Instance.buildingController.GetHospitalBuilding().SendPeopleToGiveMedicine();
        }
        else
        {
            ShowErrorMessage();
        }
    }

    private bool CanGiveAwayMedicine()
    {
        return CheckForEnoughPeople(ControllersManager.Instance.buildingController.GetHospitalBuilding().PeopleToGiveMedicine) &&
               EnoughMedicineToGiveAway();
    }

    private void ShowErrorMessage()
    {
        if (!EnoughMedicineToGiveAway())
        {
            _errorText.text = "НЕ ДОСТАТОЧНО МЕДИЦИНЫ";
        }
        else if (!CheckForEnoughPeople(ControllersManager.Instance.buildingController.GetHospitalBuilding().PeopleToGiveMedicine))
        {
            _errorText.text = "НЕ ДОСТАТОЧНО ЛЮДЕЙ";
        }
        _errorText.enabled = true;
    }

    private void UpdateMedicineTimerText()
    {
        _medicineTimerText.text = "Крайний срок отправки мед. помощи - " +
            ControllersManager.Instance.buildingController.GetHospitalBuilding().DaysToGiveMedicine.ToString() + " дн.";
    }

    private bool EnoughMedicineToGiveAway()
    {
        return ChechIfEnoughResourcesByType(ResourceController.ResourceType.Medicine, ControllersManager.Instance.buildingController.GetHospitalBuilding().MedicineToGive);
    }

    
}
