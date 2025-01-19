using DG.Tweening;
using TMPro;
using UnityEngine;

public class CityHallPopUp : InfoPopUp
{
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
        _errorText.enabled = false;
        _isDestroyable = false;

        ControllersManager.Instance.timeController.OnNextDayEvent += OnNextDayEvent;
    }

    public void ShowCityHallPopUp()
    {
        UpdateMilitaryTimerText();
        UpdateRelationWithGovermentText();

        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            IsActive = true;

            SetAlpha(1);
        });
    }

    private void OnNextDayEvent()
    {
        if (ControllersManager.Instance.buildingController.GetCityHallBuilding().DayPassed())
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
        _relationWithGovermentText.text = "Отношение - " +
            ControllersManager.Instance.buildingController.GetCityHallBuilding()._relationWithGoverment.ToString();
    }

    private void UpdateMilitaryTimerText()
    {
        _militaryTimerText.text = "Крайний срок отправки воен. помощи - " +
            ControllersManager.Instance.buildingController.GetCityHallBuilding()._daysLeftToSendArmyMaterials.ToString() + "дн.";
    }
}
