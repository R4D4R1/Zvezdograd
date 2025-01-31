using DG.Tweening;
using TMPro;
using UnityEngine;

public class HospitalPopUp : QuestPopUp
{
    [SerializeField] private TextMeshProUGUI _medicineTimerText;
    [SerializeField] private GameObject activeBtn;
    [SerializeField] private GameObject inactiveBtn;

    protected override void Start()
    {
        base.Start();
        InitializePopUp();
    }

    private void InitializePopUp()
    {
        _errorText.enabled = false;
        _isDestroyable = false;
        activeBtn.SetActive(true);
        inactiveBtn.SetActive(false);
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
            DisableActiveButton();
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

    private void DisableActiveButton()
    {
        activeBtn.SetActive(false);
        inactiveBtn.SetActive(true);
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

    private bool EnoughMedicineToGiveAway()
    {
        return ControllersManager.Instance.resourceController.GetMedicine() > ControllersManager.Instance.buildingController.GetHospitalBuilding().MedicineToGive;
    }

    private void UpdateMedicineTimerText()
    {
        _medicineTimerText.text = "Крайний срок отправки мед. помощи - " +
            ControllersManager.Instance.buildingController.GetHospitalBuilding().DaysToGiveMedicine.ToString() + " дн.";
    }
}
