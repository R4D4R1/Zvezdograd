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
        _relationWithGovermentText.text = "��������� - " +
            _building.RelationWithGoverment.ToString();
    }

    private void UpdateMilitaryTimerText()
    {
        _militaryTimerText.text = "������� ���� �������� ����. ������ - " +
            _building.DaysLeftToSendArmyMaterials.ToString() + "��.";
    }

    private void UpdateHelpFromGovTimerText()
    {
        _helpFromGovTimerText.text = "������ �������� ����� - " +
            _building.DaysLeftToRecieveGovHelp.ToString() + "��.";
    }
}
