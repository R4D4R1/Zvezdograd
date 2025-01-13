using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class CityHallPopUp : InfoPopUp
{
    private CityHallBuilding _buildingToUse;
    [SerializeField] private TextMeshProUGUI _errorText;

    [SerializeField] private int _daysLeftToSendArmyMaterialsOriginal = 3;
    [SerializeField] private TextMeshProUGUI _militaryTimerText;

    private int _daysLeftToSendArmyMaterials;

    [Range(0f, 10f)]
    [SerializeField] private int _relationWithGoverment;
    [SerializeField] private TextMeshProUGUI _relationWithGovermentText;



    // Здание совета

    // Делать поставки вооружения с завода - написан срок
    // За непоставк у в срок - минус 2 очка / за поставку плюс 2


    //передать для государства медикаменты
    //передать стройматериалы
    //передать провизию


    private void Start()
    {
        _daysLeftToSendArmyMaterials = _daysLeftToSendArmyMaterialsOriginal;
        ControllersManager.Instance.timeController.OnNextDayEvent += TimeController_OnNextDayEvent;
        UpdateMilitaryTimerText();
        UpdateRelationWithGovermentText();
    }

    private void TimeController_OnNextDayEvent()
    {
        if (_daysLeftToSendArmyMaterials > 0)
        {
            _daysLeftToSendArmyMaterials--;
            UpdateMilitaryTimerText();
        }

        if (_daysLeftToSendArmyMaterials == 0)
        {
            _daysLeftToSendArmyMaterials = _daysLeftToSendArmyMaterialsOriginal;
            if (_relationWithGoverment > 1)
            {
                _relationWithGoverment -= 2;
                UpdateRelationWithGovermentText();
                UpdateMilitaryTimerText();
            }
            else
            {
                // GAMEOVER
            }
        }
    }

    private void UpdateRelationWithGovermentText()
    {
        _relationWithGovermentText.text = "Отношение - " + _relationWithGoverment.ToString();
    }

    private void UpdateMilitaryTimerText()
    {
        _militaryTimerText.text = "Крайний срок отправки воен. помощи - " + _daysLeftToSendArmyMaterials.ToString() + "дн.";
    }



    public void ShowCityHallPopUp(CityHallBuilding cityHallBuilding)
    {
        _buildingToUse = cityHallBuilding;

        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            SetAlpha(1);
        });
    }

    public override void HidePopUp()
    {
        _bgImage.transform.DOScale(Vector3.zero, scaleDownDuration);

        ControllersManager.Instance.mainGameUIController.EnableEscapeMenuToggle();
        ControllersManager.Instance.mainGameUIController.TurnOnUI();
        ControllersManager.Instance.blurController.UnBlurBackGroundSmoothly();

        _errorText.enabled = false;

        SetAlpha(0);
    }
}
