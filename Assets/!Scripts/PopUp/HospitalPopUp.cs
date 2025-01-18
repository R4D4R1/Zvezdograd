using DG.Tweening;
using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class HospitalPopUp : EnoughPeoplePopUp
{
    //[SerializeField] private TextMeshProUGUI _medicineTimerText;

    [SerializeField] private int _stabilityNegativeRemoveValue;
    [SerializeField] private int _originalDaysToGiveMedicine;

    [SerializeField] private GameObject activeBtn;
    [SerializeField] private GameObject inactiveBtn;

    private bool _foodWasGivenAwayInLastTwoDay = false;
    private int _daysToGiveMedicine;

    private void Start()
    {
        _daysToGiveMedicine = _originalDaysToGiveMedicine;
        _errorText.enabled = false;
        _isDestroyable = false;

        activeBtn.SetActive(true);
        inactiveBtn.SetActive(false);
        ControllersManager.Instance.timeController.OnNextDayEvent += NextDayStarted;
    }

    private void NextDayStarted()
    {
        _daysToGiveMedicine--;

        if (_daysToGiveMedicine == 0)
        {
            if (!_foodWasGivenAwayInLastTwoDay)
            {
                ControllersManager.Instance.resourceController.AddOrRemoveStability(_stabilityNegativeRemoveValue);
            }
            else
            {
                activeBtn.SetActive(true);
                inactiveBtn.SetActive(false);
            }

            _daysToGiveMedicine = _originalDaysToGiveMedicine;
        }
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
        if (EnoughPeopleTo(GetHospitalBuilding().PeopleToGiveMedicine) && EnoughMedicineToGiveAway())
        {
            //_foodWasGivenAwayToday = true;

            activeBtn.SetActive(false);
            inactiveBtn.SetActive(true);

            GetHospitalBuilding().SendPeopleToGiveMedicine();
        }
        else if (!EnoughMedicineToGiveAway())
        {
            _errorText.text = "ÍÅ ÄÎÑÒÀÒÎ×ÍÎ ÌÅÄÈÖÈÍÛ";
            _errorText.enabled = true;
        }
        else if (!EnoughPeopleTo(GetHospitalBuilding().PeopleToGiveMedicine))
        {
            _errorText.text = "ÍÅ ÄÎÑÒÀÒÎ×ÍÎ ËÞÄÅÉ";
            _errorText.enabled = true;
        }
    }

    private bool EnoughMedicineToGiveAway()
    {
        if (ControllersManager.Instance.resourceController.GetMedicine() > GetHospitalBuilding().MedicineToGive)
            return true;
        else
            return false;
    }

    public HospitalBuilding GetHospitalBuilding()
    {
        return ControllersManager.Instance.buildingController.SpecialBuildings.OfType<HospitalBuilding>().FirstOrDefault();
    }
}
