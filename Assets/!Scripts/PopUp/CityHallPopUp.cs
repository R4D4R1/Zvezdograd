using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class CityHallPopUp : InfoPopUp
{
    private CityHallBuilding _buildingToUse;
    [SerializeField] private TextMeshProUGUI _errorText;

    [SerializeField] private TextMeshProUGUI _relationWithGovermentText;

    [SerializeField] private TextMeshProUGUI _militaryTimerText;

    // Здание совета

    // Делать поставки вооружения с завода - написан срок
    // За непоставк у в срок - минус 2 очка / за поставку плюс 2


    //передать для государства медикаменты
    //передать стройматериалы
    //передать провизию


    private void Start()
    {
        ControllersManager.Instance.timeController.OnNextDayEvent += TimeController_OnNextDayEvent;
    }

    public void ShowCityHallPopUp(CityHallBuilding cityHallBuilding)
    {
        _buildingToUse = cityHallBuilding;

        UpdateMilitaryTimerText();
        UpdateRelationWithGovermentText();

        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            IsActive = true;

            SetAlpha(1);
        });
    }

    public override void HidePopUp()
    {
        if (IsActive)
        {
            IsActive = false;

            _bgImage.transform.DOScale(Vector3.zero, scaleDownDuration);

            _errorText.enabled = false;

            ControllersManager.Instance.mainGameUIController.EnableEscapeMenuToggle();
            ControllersManager.Instance.mainGameUIController.TurnOnUI();

            SetAlpha(0);
        }
    }

    private void TimeController_OnNextDayEvent()
    {
        if (_buildingToUse.DayPassed())
        {
            UpdateRelationWithGovermentText();
            UpdateMilitaryTimerText();
        }
        else
        {
            //gameover in DayPassed()
        }
    }

    private void UpdateRelationWithGovermentText()
    {
        _relationWithGovermentText.text = "Отношение - " + _buildingToUse._relationWithGoverment.ToString();
    }

    private void UpdateMilitaryTimerText()
    {
        _militaryTimerText.text = "Крайний срок отправки воен. помощи - " + _buildingToUse._daysLeftToSendArmyMaterials.ToString() + "дн.";
    }
}
