using DG.Tweening;
using UnityEngine;

public class FoodTrucksPopUp : QuestPopUp
{
    [Header("UI")]
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

    public void GiveAwayProvision()
    {
        if (CanGiveAwayProvision())
        {
            DisableActiveButton();
            ControllersManager.Instance.buildingController.GetFoodTruckBuilding().SendPeopleToGiveProvision();
        }
        else
        {
            ShowErrorMessage();
        }
    }

    private bool CanGiveAwayProvision()
    {
        return CheckForEnoughPeople(ControllersManager.Instance.buildingController.GetFoodTruckBuilding().PeopleToGiveProvision) &&
               EnoughProvisionToGiveAway();
    }

    private void DisableActiveButton()
    {
        activeBtn.SetActive(false);
        inactiveBtn.SetActive(true);
    }

    private void ShowErrorMessage()
    {
        if (!EnoughProvisionToGiveAway())
        {
            _errorText.text = "ÍÅÒ ÏÐÎÂÈÇÈÈ";
        }
        else if (!CheckForEnoughPeople(ControllersManager.Instance.buildingController.GetFoodTruckBuilding().PeopleToGiveProvision))
        {
            _errorText.text = "ÍÅ ÄÎÑÒÀÒÎ×ÍÎ ËÞÄÅÉ";
        }
        _errorText.enabled = true;
    }

    public bool EnoughProvisionToGiveAway()
    {
        return ControllersManager.Instance.resourceController.GetProvision() > ControllersManager.Instance.buildingController.GetFoodTruckBuilding().FoodToGive;
    }
}
