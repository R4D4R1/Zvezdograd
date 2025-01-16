using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class CityHallPopUp : InfoPopUp
{
    [SerializeField] private CityHallBuilding _buildingToUse;
    [SerializeField] private TextMeshProUGUI _errorText;

    [SerializeField] private TextMeshProUGUI _relationWithGovermentText;

    [SerializeField] private TextMeshProUGUI _militaryTimerText;

    // ������ ������

    // ������ �������� ���������� � ������ - ������� ����
    // �� ��������� � � ���� - ����� 2 ���� / �� �������� ���� 2


    //�������� ��� ����������� �����������
    //�������� ��������������
    //�������� ��������


    private void Start()
    {
        ControllersManager.Instance.timeController.OnNextDayEvent += TimeController_OnNextDayEvent;
    }

    public void ShowCityHallPopUp()
    {
        UpdateMilitaryTimerText();
        UpdateRelationWithGovermentText();

        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            IsActive = true;

            ControllersManager.Instance.mainGameUIController.InPopUp(this);

            SetAlpha(1);
        });
    }

    public override void HidePopUp()
    {
        if (IsActive)
        {
            ControllersManager.Instance.mainGameUIController.Running();

            _bgImage.transform.DOScale(Vector3.zero, scaleDownDuration).OnComplete(() =>
            {
                IsActive = false;
                ControllersManager.Instance.mainGameUIController.InGame();
            });

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
        _relationWithGovermentText.text = "��������� - " + _buildingToUse._relationWithGoverment.ToString();
    }

    private void UpdateMilitaryTimerText()
    {
        _militaryTimerText.text = "������� ���� �������� ����. ������ - " + _buildingToUse._daysLeftToSendArmyMaterials.ToString() + "��.";
    }
}
