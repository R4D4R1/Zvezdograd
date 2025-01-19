using DG.Tweening;
using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class HospitalPopUp : EnoughPeoplePopUp
{
    [SerializeField] private TextMeshProUGUI _medicineTimerText;

    [SerializeField] private GameObject activeBtn;
    [SerializeField] private GameObject inactiveBtn;

    private void Start()
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
        if (EnoughPeopleTo(ControllersManager.Instance.buildingController.GetHospitalBuilding().PeopleToGiveMedicine)
            && EnoughMedicineToGiveAway())
        {
            activeBtn.SetActive(false);
            inactiveBtn.SetActive(true);

            ControllersManager.Instance.buildingController.GetHospitalBuilding().SendPeopleToGiveMedicine();
        }
        else if (!EnoughMedicineToGiveAway())
        {
            _errorText.text = "ÍÅ ÄÎÑÒÀÒÎ×ÍÎ ÌÅÄÈÖÈÍÛ";
            _errorText.enabled = true;
        }
        else if (!EnoughPeopleTo(ControllersManager.Instance.buildingController.GetHospitalBuilding().PeopleToGiveMedicine))
        {
            _errorText.text = "ÍÅ ÄÎÑÒÀÒÎ×ÍÎ ËŞÄÅÉ";
            _errorText.enabled = true;
        }
    }

    private bool EnoughMedicineToGiveAway()
    {
        if (ControllersManager.Instance.resourceController.GetMedicine() > ControllersManager.Instance.buildingController.GetHospitalBuilding().MedicineToGive)
            return true;
        else
            return false;
    }

    private void UpdateMedicineTimerText()
    {
        _medicineTimerText.text = "Êğàéíèé ñğîê îòïğàâêè ìåä. ïîìîùè - " +
            ControllersManager.Instance.buildingController.GetHospitalBuilding().DaysToGiveMedicine.ToString() + " äí.";
    }
}
