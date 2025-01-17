using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class FoodTrucksPopUp : EnoughPeoplePopUp
{
    [SerializeField] private TextMeshProUGUI _foodTimerText;

    [SerializeField] private int _stabilityNegativeRemoveValue;

    [SerializeField] private GameObject activeBtn;
    [SerializeField] private GameObject inactiveBtn;


    private bool _foodWasGivenAwayToday = false;

    private void Start()
    {
        _errorText.enabled = false;
        _isDestroyable = false;

        activeBtn.SetActive(true);
        inactiveBtn.SetActive(false);
        ControllersManager.Instance.timeController.OnNextDayEvent += NextDayStarted;
    }

    public void ShowFoodTruckPopUp()
    {
        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            IsActive = true;

            SetAlpha(1);
        });
    }

    public void GivaAwayFood()
    {
        if (EnoughPeopleTo(FoodTrucksBuilding.Instance.PeopleToGiveProvision) && EnoughProvisionToGiveAway())
        {
            _foodWasGivenAwayToday = true;

            activeBtn.SetActive(false);
            inactiveBtn.SetActive(true);

            FoodTrucksBuilding.Instance.SendPeopleToGiveProvision();
        }
        else if (!EnoughProvisionToGiveAway())
        {
            _errorText.text = "ÍÅÒ ÏÐÎÂÈÇÈÈ";
            _errorText.enabled = true;
        }
        else if (!EnoughPeopleTo(FoodTrucksBuilding.Instance.PeopleToGiveProvision))
        {
            _errorText.text = "ÍÅ ÄÎÑÒÀÒÎ×ÍÎ ËÞÄÅÉ";
            _errorText.enabled = true;
        }
    }

    private void NextDayStarted()
    {
        if (!_foodWasGivenAwayToday)
        {
            ControllersManager.Instance.resourceController.AddOrRemoveStability(_stabilityNegativeRemoveValue);
        }
        else
        {
            activeBtn.SetActive(true);
            inactiveBtn.SetActive(false);
        }
    }

    public bool EnoughProvisionToGiveAway()
    {
        if (ControllersManager.Instance.resourceController.GetProvision() > FoodTrucksBuilding.Instance.FoodToGive)
            return true;
        else
            return false;
    }
}
