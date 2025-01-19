using DG.Tweening;
using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class FoodTrucksPopUp : EnoughPeoplePopUp
{
    [SerializeField] private GameObject activeBtn;
    [SerializeField] private GameObject inactiveBtn;

    //private bool _foodWasGivenAwayToday = false;

    private void Start()
    {
        _errorText.enabled = false;
        _isDestroyable = false;

        activeBtn.SetActive(true);
        inactiveBtn.SetActive(false);
        ControllersManager.Instance.timeController.OnNextDayEvent += OnNextDayEvent;
    }

    public void ShowFoodTruckPopUp()
    {
        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            IsActive = true;

            SetAlpha(1);
        });
    }

    private void OnNextDayEvent()
    {
        if (ControllersManager.Instance.buildingController.GetFoodTruckBuilding().FoodWasGivenAwayToday())
        {
            activeBtn.SetActive(true);
            inactiveBtn.SetActive(false);
        }

    }

    public void GivaAwayFood()
    {
        if (EnoughPeopleTo(ControllersManager.Instance.buildingController.GetFoodTruckBuilding().PeopleToGiveProvision) && EnoughProvisionToGiveAway())
        {
            activeBtn.SetActive(false);
            inactiveBtn.SetActive(true);

            ControllersManager.Instance.buildingController.GetFoodTruckBuilding().SendPeopleToGiveProvision();
        }
        else if (!EnoughProvisionToGiveAway())
        {
            _errorText.text = "ÍÅÒ ÏÐÎÂÈÇÈÈ";
            _errorText.enabled = true;
        }
        else if (!EnoughPeopleTo(ControllersManager.Instance.buildingController.GetFoodTruckBuilding().PeopleToGiveProvision))
        {
            _errorText.text = "ÍÅ ÄÎÑÒÀÒÎ×ÍÎ ËÞÄÅÉ";
            _errorText.enabled = true;
        }
    }

    public bool EnoughProvisionToGiveAway()
    {
        if (ControllersManager.Instance.resourceController.GetProvision() > ControllersManager.Instance.buildingController.GetFoodTruckBuilding().FoodToGive)
            return true;
        else
            return false;
    }
}