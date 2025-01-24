using DG.Tweening;
using TMPro;
using UnityEngine;

public class CityHallPopUp : InfoPopUp
{
    [SerializeField] private TextMeshProUGUI _errorText;

    [SerializeField] private TextMeshProUGUI _relationWithGovermentText;

    [SerializeField] private TextMeshProUGUI _militaryTimerText;

    [SerializeField] private TextMeshProUGUI _helpFromGovTimerText;

    private CityHallBuilding _building;

    private void Start()
    {
        _building = ControllersManager.Instance.buildingController.GetCityHallBuilding();

        _errorText.enabled = false;
        _isDestroyable = false;

        ControllersManager.Instance.timeController.OnNextDayEvent += OnNextDayEvent;
    }

    public void ShowCityHallPopUp()
    {
        UpdateAllText();

        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            IsActive = true;

            SetAlpha(1);
        });
    }

    private void OnNextDayEvent()
    {
        if (_building.DayPassed())
        {
            UpdateAllText();
        }
        else
        {
            //gameover in DayPassed()
        }
    }

    private void UpdateAllText()
    {
        UpdateRelationWithGovermentText();
        UpdateMilitaryTimerText();
        UpdateHelpFromGovTimerText();
    }

    private void UpdateRelationWithGovermentText()
    {
        _relationWithGovermentText.text = "Отношение - " +
            _building.RelationWithGoverment.ToString();
    }

    private void UpdateMilitaryTimerText()
    {
        _militaryTimerText.text = "Крайний срок отправки воен. помощи - " +
            _building.DaysLeftToSendArmyMaterials.ToString() + "дн.";
    }

    private void UpdateHelpFromGovTimerText()
    {
        _helpFromGovTimerText.text = "Помощь прибудет через - " +
            _building.DaysLeftToRecieveGovHelp.ToString() + "дн.";
    }
}
